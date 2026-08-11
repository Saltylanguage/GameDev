using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace SaltyGame
{
    public static class CellularSimDataFingerprint
    {
        public const string Version = "cellular-sim-data-v2";

        public static string Create(CellularSimData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            var canonical = new StringBuilder(2048);
            canonical.Append(Version).Append('|');
            Append(canonical, data.Width);
            Append(canonical, data.Height);
            Append(canonical, data.RunDurationSeconds);
            Append(canonical, data.StepInterval);
            Append(canonical, data.MaxPopulation);
            Append(canonical, data.MinPopulation);

            canonical.Append("probabilities[");
            foreach (var entry in Sort(data.StartingProbabilities))
            {
                Append(canonical, entry.Key.Value);
                Append(canonical, entry.Value);
            }

            canonical.Append("]rules[");
            foreach (var entry in Sort(data.SpeciesRules))
            {
                Append(canonical, entry.Key.Value);
                AppendRules(canonical, entry.Value);
            }

            canonical.Append("]terrain[");
            foreach (var entry in Sort(data.TerrainDefinitions))
            {
                Append(canonical, entry.Key.Value);
                AppendTerrain(canonical, entry.Value);
            }

            canonical.Append("]alphaOffspring[");
            foreach (var entry in Sort(data.AlphaOffspringRules))
            {
                Append(canonical, entry.Key.Value);
                Append(canonical, entry.Value.Chance);
                Append(canonical, entry.Value.HealthBonus);
                Append(canonical, entry.Value.EnergyBonus);
            }

            canonical.Append(']');
            return Hash(canonical.ToString());
        }

        static void AppendRules(StringBuilder builder, SpeciesRules rules)
        {
            AppendPattern(builder, rules.MovementPattern);
            AppendPattern(builder, rules.AttackPattern);
            Append(builder, rules.AttackAmount);
            AppendPattern(builder, rules.BlockPattern);
            Append(builder, rules.BlockAmount);
            AppendPattern(builder, rules.DietPattern);
            Append(builder, rules.DietTargetId.HasValue ? rules.DietTargetId.Value.Value : string.Empty);
            AppendPattern(builder, rules.ReproductionPattern);
            Append(builder, rules.ReproductionNeighborCount);
            Append(builder, rules.ReproductionChance);
            Append(builder, rules.ReproductionFoodRequired);
            Append(builder, rules.MaxReproductionGroupSize);
            Append(builder, rules.StartingEnergy);
            Append(builder, rules.WiltChance);
            Append(builder, rules.CrowdingEnergyPenalty);
            Append(builder, rules.StartingFoodReserve);
            Append(builder, rules.SeedDropChance);
            Append(builder, rules.EnergyValue);
            Append(builder, rules.Metabolism);
        }

        static void AppendTerrain(StringBuilder builder, TerrainDefinition terrain)
        {
            Append(builder, terrain.IsPassable);
            Append(builder, terrain.MovementCost);
            Append(builder, terrain.ProvidesResource);
            Append(builder, terrain.PresentationColor.r);
            Append(builder, terrain.PresentationColor.g);
            Append(builder, terrain.PresentationColor.b);
            Append(builder, terrain.PresentationColor.a);
            Append(builder, terrain.RegrowthPerTick);
        }

        static void AppendPattern(StringBuilder builder, GridPattern pattern)
        {
            builder.Append('(').Append(pattern.Count).Append(':');
            foreach (var offset in pattern.Offsets)
            {
                Append(builder, offset.x);
                Append(builder, offset.y);
            }

            builder.Append(')');
        }

        static void Append(StringBuilder builder, string value)
        {
            builder.Append(value?.Length ?? 0).Append(':').Append(value).Append(';');
        }

        static void Append(StringBuilder builder, int value)
        {
            Append(builder, value.ToString(CultureInfo.InvariantCulture));
        }

        static void Append(StringBuilder builder, float value)
        {
            Append(builder, value.ToString("R", CultureInfo.InvariantCulture));
        }

        static void Append(StringBuilder builder, bool value)
        {
            Append(builder, value ? "1" : "0");
        }

        static List<KeyValuePair<TKey, TValue>> Sort<TKey, TValue>(
            IReadOnlyDictionary<TKey, TValue> source)
        {
            var entries = new List<KeyValuePair<TKey, TValue>>(source);
            entries.Sort((left, right) => string.CompareOrdinal(left.Key.ToString(), right.Key.ToString()));
            return entries;
        }

        static string Hash(string value)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(value));
                var result = new StringBuilder(bytes.Length * 2);
                foreach (var b in bytes)
                {
                    result.Append(b.ToString("x2", CultureInfo.InvariantCulture));
                }

                return result.ToString();
            }
        }
    }
}
