using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SaltyGame
{
    internal sealed class HareBurrows
    {
        internal static readonly SpeciesId Hare = new SpeciesId("hare");
        static readonly SpeciesId Fox = new SpeciesId("fox");
        static readonly GridPattern Neighbors = GridPatternTemplates.CreateMooreRange(1);
        const int DigEnergy = 24;
        const int Lifetime = 200;

        sealed class Burrow
        {
            public long Owner;
            public int X;
            public int Y;
            public int Expires;
            public readonly List<SpeciesCell> Occupants = new List<SpeciesCell>();
        }

        readonly List<Burrow> burrows = new List<Burrow>();
        readonly HashSet<long> diggers = new HashSet<long>();

        public int HiddenCount
        {
            get
            {
                var count = 0;
                foreach (var burrow in burrows) count += burrow.Occupants.Count;
                return count;
            }
        }

        public HareBurrows Copy()
        {
            var copy = new HareBurrows();
            copy.diggers.UnionWith(diggers);
            foreach (var burrow in burrows)
            {
                var entry = new Burrow { Owner = burrow.Owner, X = burrow.X, Y = burrow.Y, Expires = burrow.Expires };
                entry.Occupants.AddRange(burrow.Occupants);
                copy.burrows.Add(entry);
            }
            return copy;
        }

        public int[] CopyCellIndices(int width)
        {
            var result = new int[burrows.Count];
            for (var i = 0; i < burrows.Count; i++)
                result[i] = burrows[i].X + burrows[i].Y * width;
            return result;
        }

        // Entity IDs are process-wide. Their order within a run is stable, including
        // identities that remain relevant after a Hare dies or leaves the surface.
        public static string Fingerprint(Grid<SpeciesCell> cells, HareLineage lineage, HareBurrows state)
        {
            var diggers = state?.diggers;
            IReadOnlyList<Burrow> burrows = state == null ? Array.Empty<Burrow>() : state.burrows;
            var ids = diggers == null ? new SortedSet<long>() : new SortedSet<long>(diggers);
            foreach (var entry in lineage.ParentRecords)
            {
                ids.Add(entry.Key);
                ids.Add(entry.Value.Parent);
                ids.Add(entry.Value.Mate);
            }
            foreach (var burrow in burrows)
                foreach (var occupant in burrow.Occupants) AddCellIds(occupant);
            for (var y = 0; y < cells.Height; y++)
                for (var x = 0; x < cells.Width; x++)
                    if (cells.GetCell(x, y).IsCreature) AddCellIds(cells.GetCell(x, y));

            ids.Remove(0);
            var labels = new Dictionary<long, int>();
            foreach (var id in ids) labels.Add(id, labels.Count + 1);
            int Label(long id) => id == 0 ? 0 : labels[id];
            var key = new StringBuilder();
            for (var y = 0; y < cells.Height; y++)
                for (var x = 0; x < cells.Width; x++)
                {
                    var cell = cells.GetCell(x, y);
                    if (!cell.IsCreature) continue;
                    key.Append(FormattableString.Invariant($"surface:{x},{y},{Label(cell.EntityId)},{Label(cell.Gestation.MateEntityId)}|"));
                }
            foreach (var id in ids)
            {
                if (diggers?.Contains(id) == true) key.Append(FormattableString.Invariant($"dug:{Label(id)}|"));
                if (lineage.ParentRecords.TryGetValue(id, out var parents))
                    key.Append(FormattableString.Invariant($"birth:{Label(id)},{Label(parents.Parent)},{Label(parents.Mate)}|"));
            }
            foreach (var burrow in burrows)
            {
                key.Append(FormattableString.Invariant($"burrow:{Label(burrow.Owner)},{burrow.X},{burrow.Y},{burrow.Expires}|"));
                foreach (var cell in burrow.Occupants)
                {
                    var memory = cell.ScentMemory;
                    key.Append(FormattableString.Invariant($"hidden:{Label(cell.EntityId)},{cell.Health},{cell.Energy},{FloatBits(cell.EnergyRemainder)},{cell.CrowdingEnergyRemainder},{cell.Age},{cell.FoodEaten},{FloatBits(cell.FoodReserve)},{cell.IsAlpha},{cell.BehaviorState},{cell.BehaviorStateTicks},{cell.AttackCooldownTicksRemaining},{cell.ReproductionCooldownTicksRemaining},{cell.ForagePhase}|"));
                    key.Append(FormattableString.Invariant($"gestation:{cell.GestationTicksRemaining},{cell.PendingLitterSize},{Label(cell.Gestation.MateEntityId)},{cell.Gestation.OffspringEnergyPool}|"));
                    key.Append(FormattableString.Invariant($"tracking:{Label(cell.TrackingTargetEntityId)},{cell.TrackingTargetX},{cell.TrackingTargetY},{cell.TrackingTicksRemaining},{Label(memory.TargetEntityId)},{memory.X},{memory.Y},{memory.TicksRemaining},{memory.SniffedDropping}|"));
                }
            }
            using (var sha = SHA256.Create())
                return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(key.ToString()))).Replace("-", "").ToLowerInvariant();

            void AddCellIds(SpeciesCell cell)
            {
                ids.Add(cell.EntityId);
                ids.Add(cell.Gestation.MateEntityId);
                ids.Add(cell.TrackingTargetEntityId);
                ids.Add(cell.ScentMemory.TargetEntityId);
            }
        }

        static int FloatBits(float value) => BitConverter.ToInt32(BitConverter.GetBytes(value), 0);

        public Grid<SpeciesCell> PrepareStep(Grid<SpeciesCell> source, IReadOnlyDictionary<SpeciesId, SpeciesRules> rules,
            HareLineage lineage, int tick, bool canDig, bool shared, int capacity)
        {
            var cells = source.Copy();
            var threatened = FindFoxAttackTiles(source, rules);
            var neighbors = cells.GetPattern(Neighbors).Offsets;
            var sites = new HashSet<int>();
            foreach (var burrow in burrows) sites.Add(burrow.X + burrow.Y * cells.Width);

            var emerged = new HashSet<long>();
            for (var i = 0; i < burrows.Count; i++)
            {
                var burrow = burrows[i];
                var expired = tick >= burrow.Expires;
                if (expired || !threatened[burrow.X + burrow.Y * cells.Width])
                {
                    TryEmerge(burrow, cells, threatened, expired, burrow.X, burrow.Y, emerged);
                    foreach (var offset in neighbors)
                        TryEmerge(burrow, cells, threatened, expired, burrow.X + offset.x, burrow.Y + offset.y, emerged);
                }

                if (expired && burrow.Occupants.Count == 0)
                {
                    sites.Remove(burrow.X + burrow.Y * cells.Width);
                    burrows.RemoveAt(i--);
                }
            }

            if (canDig && neighbors.Count == 8)
            {
                for (var y = 0; y < cells.Height; y++)
                    for (var x = 0; x < cells.Width; x++)
                    {
                        var hare = cells.GetCell(x, y);
                        if (!IsHare(hare) || hare.Energy < DigEnergy || diggers.Contains(hare.EntityId)
                            || sites.Contains(x + y * cells.Width)) continue;
                        var clear = true;
                        foreach (var offset in neighbors)
                            if (!cells.TryGetCell(x + offset.x, y + offset.y, out var neighbor) || neighbor.IsOccupied)
                            {
                                clear = false;
                                break;
                            }
                        if (!clear) continue;

                        cells.SetCell(x, y, hare.WithEntity(Hare, hare.Health, hare.Energy - DigEnergy,
                            hare.Age, hare.FoodEaten, hare.FoodReserve, hare.IsAlpha));
                        diggers.Add(hare.EntityId);
                        sites.Add(x + y * cells.Width);
                        burrows.Add(new Burrow { Owner = hare.EntityId, X = x, Y = y, Expires = tick + Lifetime });
                    }
            }

            foreach (var burrow in burrows)
            {
                if (!canDig || tick >= burrow.Expires) continue;
                TryHide(burrow, cells, threatened, lineage, shared, capacity, burrow.X, burrow.Y, emerged);
                foreach (var offset in neighbors)
                    TryHide(burrow, cells, threatened, lineage, shared, capacity,
                        burrow.X + offset.x, burrow.Y + offset.y, emerged);
            }
            return cells;
        }

        static bool IsHare(SpeciesCell cell) => cell.IsCreature && cell.SpeciesId == Hare;

        static bool[] FindFoxAttackTiles(Grid<SpeciesCell> cells, IReadOnlyDictionary<SpeciesId, SpeciesRules> rules)
        {
            var threatened = new bool[cells.Count];
            if (!rules.TryGetValue(Fox, out var fox)) return threatened;
            var offsets = cells.GetPattern(fox.AttackPattern).Offsets;
            for (var y = 0; y < cells.Height; y++)
                for (var x = 0; x < cells.Width; x++)
                {
                    var cell = cells.GetCell(x, y);
                    if (!cell.IsCreature || cell.SpeciesId != Fox) continue;
                    foreach (var offset in offsets)
                    {
                        var targetX = x + offset.x;
                        var targetY = y + offset.y;
                        if (cells.TryResolveCoordinates(ref targetX, ref targetY))
                            threatened[targetX + targetY * cells.Width] = true;
                    }
                }
            return threatened;
        }

        static void TryHide(Burrow burrow, Grid<SpeciesCell> cells, bool[] threatened, HareLineage lineage,
            bool shared, int capacity, int x, int y, HashSet<long> emerged)
        {
            if (burrow.Occupants.Count >= capacity || !cells.TryResolveCoordinates(ref x, ref y)
                || !threatened[x + y * cells.Width]) return;
            var hare = cells.GetCell(x, y);
            if (!IsHare(hare) || emerged.Contains(hare.EntityId)) return;
            if (hare.EntityId != burrow.Owner && !shared && !(lineage?.IsDescendant(hare.EntityId, burrow.Owner) ?? false))
                return;
            burrow.Occupants.Add(hare);
            cells.SetCell(x, y, hare.WithoutEntity());
        }

        static void TryEmerge(Burrow burrow, Grid<SpeciesCell> cells, bool[] threatened, bool expired,
            int x, int y, HashSet<long> emerged)
        {
            if (burrow.Occupants.Count == 0 || !cells.TryResolveCoordinates(ref x, ref y)
                || (!expired && threatened[x + y * cells.Width])) return;
            var destination = cells.GetCell(x, y);
            if (!destination.IsPassable || destination.IsOccupied) return;

            var hare = burrow.Occupants[0];
            cells.SetCell(x, y, hare.PlaceCreatureOn(destination));
            burrow.Occupants.RemoveAt(0);
            emerged.Add(hare.EntityId);
        }
    }
}
