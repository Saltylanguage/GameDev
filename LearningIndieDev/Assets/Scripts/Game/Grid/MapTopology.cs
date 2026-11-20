using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace SaltyGame
{
    [Serializable]
    public struct MapBoundaryConnection
    {
        public Vector2Int source, direction, target;
        public MapBoundaryConnection(Vector2Int source, Vector2Int direction, Vector2Int target)
        {
            this.source = source;
            this.direction = direction;
            this.target = target;
        }
    }

    [Serializable]
    public sealed class MapTopologyDefinition
    {
        private const int MaximumRoughness = 100;
        private const int MinimumHoleSize = 16;
        private const int MinimumIslandMapWidth = 24;
        private const int MinimumIslandMapHeight = 16;
        private const double HoleRadius = 0.15;
        private static readonly Vector2Int[] CardinalDirections =
            { Vector2Int.left, Vector2Int.right, Vector2Int.down, Vector2Int.up };

        public enum FixedShape
        {
            AuthoredLayout = 0,
            Square = 1,
            Circle = 2,
            Oval = 3,
            Diamond = 4,
            Cross = 5,
            ShortRectangle = 6,
            LongRectangle = 7,
            IrregularLand = 8,
        }

        [SerializeField]
        [Tooltip("Fixed shapes use Height as their size. Irregular Land and Authored Layout use Width and Height.")]
        private FixedShape shape;
        [SerializeField]
        [Tooltip("Irregular Land only. This seed controls the map outline independently of the simulation seed.")]
        private int mapSeed = 1;
        [SerializeField, Range(0, MaximumRoughness)]
        [Tooltip("Irregular Land only. Higher values make the bays and lobes more pronounced.")]
        private int edgeRoughness = 35;
        [SerializeField]
        [Tooltip("Irregular Land only. Adds an impassable interior gap when Width and Height are at least 16.")]
        private bool interiorHole;
        [SerializeField]
        [Tooltip("Irregular Land only. Adds a separate island when Width is at least 24 and Height is at least 16.")]
        private bool detachedIsland;
        [SerializeField] bool explicitLayout, rectangle;
        [SerializeField] Vector2Int[] cells = Array.Empty<Vector2Int>();
        [SerializeField] MapBoundaryConnection[] connections = Array.Empty<MapBoundaryConnection>();
        [SerializeField] string generationInputs = "authored-map-v1";

        public MapTopologyDefinition() { }
        public MapTopologyDefinition(MapTopology topology)
        {
            if (topology == null) throw new ArgumentNullException(nameof(topology));
            explicitLayout = true;
            rectangle = topology.IsRectangle;
            cells = topology.CopyCells();
            connections = topology.CopyConnections();
            generationInputs = topology.GenerationInputs;
        }

        public IGridTopology Create(int width, int height)
        {
            if (shape == FixedShape.IrregularLand)
            {
                return CreateIrregularLand(width, height);
            }

            if (shape != FixedShape.AuthoredLayout)
            {
                return CreateFixedShape(shape, height);
            }

            if (!explicitLayout) return new RectangularGridTopology(width, height, true);
            if (rectangle) return new MapTopology(width, height, generationInputs: generationInputs);
            if (cells == null || cells.Length == 0) throw new ArgumentException("An explicit authored layout needs playable cells.");
            return new MapTopology(width, height, cells, connections, generationInputs);
        }

        private MapTopology CreateIrregularLand(int width, int height)
        {
            if (width <= 0 || height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width), "Map dimensions must be positive.");
            }

            // Validate the storage area before building any cells.
            _ = checked(width * height);
            var roughness = Math.Clamp(edgeRoughness, 0, MaximumRoughness);
            var variation = Math.Sqrt((double)roughness / MaximumRoughness);
            var hasIsland = detachedIsland && width >= MinimumIslandMapWidth && height >= MinimumIslandMapHeight;
            var mainlandWidth = hasIsland ? width - width / 4 : width;
            var hasHole = interiorHole && mainlandWidth >= MinimumHoleSize && height >= MinimumHoleSize;
            var random = new System.Random(mapSeed);
            var land = BuildLandmass(mainlandWidth, height, random, variation, hasHole);
            if (hasIsland)
            {
                var islandWidth = width - mainlandWidth - 1;
                var islandHeight = height / 3;
                var islandStartY = random.Next(height - islandHeight + 1);
                // Reserve one empty column so even a diagonal step cannot cross to the island.
                foreach (var cell in BuildLandmass(islandWidth, islandHeight, random, variation, hasHole: false))
                {
                    land.Add(new Vector2Int(mainlandWidth + 1 + cell.x, islandStartY + cell.y));
                }
            }

            var inputs = string.Format(CultureInfo.InvariantCulture,
                "irregular-land-v2:seed={0};width={1};height={2};roughness={3};hole={4}",
                mapSeed, width, height, roughness, hasHole ? 1 : 0);
            if (hasIsland) inputs += ";island=1";
            return new MapTopology(width, height, land, generationInputs: inputs);
        }

        private static List<Vector2Int> BuildLandmass(int width, int height, System.Random random, double variation, bool hasHole)
        {
            var broadPhase = random.NextDouble() * 2 * Math.PI;
            var mediumPhase = random.NextDouble() * 2 * Math.PI;
            var smallPhase = random.NextDouble() * 2 * Math.PI;
            var centerX = width / 2;
            var centerY = height / 2;
            var centerOffsetX = (2.0 * centerX + 1 - width) / width;
            var centerOffsetY = (2.0 * centerY + 1 - height) / height;
            var minimumRadiusSquared = Math.Max(0.36 * 0.36,
                centerOffsetX * centerOffsetX + centerOffsetY * centerOffsetY);
            var candidates = new HashSet<Vector2Int>();
            for (var y = 0; y < height; y++)
            {
                var dy = (2.0 * y + 1 - height) / height;
                for (var x = 0; x < width; x++)
                {
                    var dx = (2.0 * x + 1 - width) / width;
                    var distanceSquared = dx * dx + dy * dy;
                    if (hasHole && distanceSquared <= HoleRadius * HoleRadius) continue;

                    // Smooth waves around the whole outline keep bays and lobes coherent across rows.
                    var angle = Math.Atan2(dy, dx);
                    var waves = 0.22 * Math.Cos(3 * angle + broadPhase)
                        + 0.14 * Math.Cos(5 * angle + mediumPhase)
                        + 0.065 * Math.Cos(8 * angle + smallPhase);
                    var radius = Math.Min(0.70 + variation * waves, 1.0);
                    if (distanceSquared <= Math.Max(radius * radius, minimumRadiusSquared))
                    {
                        candidates.Add(new Vector2Int(x, y));
                    }
                }
            }

            // Keep only the cells connected to the core after sampling the outline.
            var startX = hasHole ? centerX + (int)Math.Ceiling(width * HoleRadius / 2) : centerX;
            var start = new Vector2Int(startX, centerY);
            var land = new List<Vector2Int> { start };
            candidates.Remove(start);
            for (var index = 0; index < land.Count; index++)
            {
                var current = land[index];
                foreach (var direction in CardinalDirections)
                {
                    var next = new Vector2Int(current.x + direction.x, current.y + direction.y);
                    if (candidates.Remove(next)) land.Add(next);
                }
            }

            return land;
        }

        private static MapTopology CreateFixedShape(FixedShape shape, int height)
        {
            if (height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height));
            }

            var width = shape switch
            {
                FixedShape.Oval or FixedShape.ShortRectangle => checked(height + Math.Max(1, height / 2)),
                FixedShape.LongRectangle => checked(height * 2),
                FixedShape.Square or FixedShape.Circle or FixedShape.Diamond or FixedShape.Cross => height,
                _ => throw new ArgumentOutOfRangeException(nameof(shape)),
            };
            var generationInputs = "fixed-shape-v1:" + shape;
            if (shape is FixedShape.Square or FixedShape.ShortRectangle or FixedShape.LongRectangle)
            {
                return new MapTopology(width, height, generationInputs: generationInputs);
            }

            var area = checked(width * height);
            // Use doubled, centered coordinates so even and odd sizes have the same symmetry.
            var cells = new List<Vector2Int>();
            for (var y = 0; y < height; y++)
            {
                var dy = Math.Abs(2L * y - (height - 1));
                var scaledY = dy * width;
                for (var x = 0; x < width; x++)
                {
                    var dx = Math.Abs(2L * x - (width - 1));
                    var scaledX = dx * height;
                    var playable = shape switch
                    {
                        FixedShape.Circle or FixedShape.Oval => scaledX * scaledX + scaledY * scaledY <= (long)area * area,
                        FixedShape.Diamond => scaledX + scaledY <= area,
                        FixedShape.Cross => dx < Math.Max(2, width / 3) || dy < Math.Max(2, height / 3),
                        _ => false,
                    };
                    if (playable)
                    {
                        cells.Add(new Vector2Int(x, y));
                    }
                }
            }

            return new MapTopology(width, height, cells, generationInputs: generationInputs);
        }
    }

    // Layout authors supply cells and reciprocal boundary pairings; gaps have no edges.
    public sealed class MapTopology : IGridTopology
    {
        private readonly RectangularGridTopology rectangularGrid;
        readonly HashSet<Vector2Int> validCells;
        readonly Dictionary<(Vector2Int, Vector2Int), Vector2Int> boundaries = new Dictionary<(Vector2Int, Vector2Int), Vector2Int>();
        readonly Dictionary<Vector2Int, Dictionary<Vector2Int, int>> distances = new Dictionary<Vector2Int, Dictionary<Vector2Int, int>>();
        readonly Vector2Int[] cells;
        readonly MapBoundaryConnection[] connections;

        public MapTopology(int width, int height, IEnumerable<Vector2Int> cells = null,
            IEnumerable<MapBoundaryConnection> connections = null, string generationInputs = "rectangle-v1")
        {
            if (width <= 0 || height <= 0) throw new ArgumentOutOfRangeException(nameof(width));
            Width = width;
            Height = height;
            GenerationInputs = generationInputs ?? throw new ArgumentNullException(nameof(generationInputs));
            IsRectangle = cells == null;
            if (IsRectangle)
            {
                if (connections != null) throw new ArgumentException("Custom connections require an explicit cell layout.");
                rectangularGrid = new RectangularGridTopology(width, height, true);
                this.cells = Array.Empty<Vector2Int>();
                this.connections = Array.Empty<MapBoundaryConnection>();
                CellCount = checked(width * height);
                return;
            }
            validCells = new HashSet<Vector2Int>();
            foreach (var cell in cells)
            {
                if (cell.x < 0 || cell.x >= width || cell.y < 0 || cell.y >= height || !validCells.Add(cell))
                    throw new ArgumentException("Map cells must be unique and within the layout dimensions.");
            }
            if (validCells.Count == 0) throw new ArgumentException("A map needs at least one cell.");
            var sorted = new List<Vector2Int>(validCells);
            sorted.Sort((a, b) => a.y != b.y ? a.y.CompareTo(b.y) : a.x.CompareTo(b.x));
            this.cells = sorted.ToArray();
            CellCount = sorted.Count;
            var landmasses = FindLandmasses();
            var pairs = new List<MapBoundaryConnection>(connections ?? Array.Empty<MapBoundaryConnection>());
            foreach (var pair in pairs)
            {
                if (!IsValidCell(pair.source.x, pair.source.y) || !IsValidCell(pair.target.x, pair.target.y)
                    || pair.direction == Vector2Int.zero || Math.Abs(pair.direction.x) > 1 || Math.Abs(pair.direction.y) > 1
                    || validCells.Contains(new Vector2Int(pair.source.x + pair.direction.x, pair.source.y + pair.direction.y)) || validCells.Contains(new Vector2Int(pair.target.x - pair.direction.x, pair.target.y - pair.direction.y)))
                    throw new ArgumentException("Boundary pairings must join valid cells at opposite missing neighbors.");
                if (landmasses[pair.source] != landmasses[pair.target])
                    throw new ArgumentException("Boundary pairings must stay on the same landmass.");
                AddBoundary(pair.source, pair.direction, pair.target);
                AddBoundary(pair.target, new Vector2Int(-pair.direction.x, -pair.direction.y), pair.source);
            }
            AddOuterSeams(landmasses, horizontal: true);
            AddOuterSeams(landmasses, horizontal: false);
            pairs.Clear();
            foreach (var edge in boundaries)
                pairs.Add(new MapBoundaryConnection(edge.Key.Item1, edge.Key.Item2, edge.Value));
            pairs.Sort((a, b) => string.CompareOrdinal(Key(a), Key(b)));
            this.connections = pairs.ToArray();
        }

        private Dictionary<Vector2Int, int> FindLandmasses()
        {
            var landmasses = new Dictionary<Vector2Int, int>(CellCount);
            var frontier = new Queue<Vector2Int>();
            var landmass = 0;
            foreach (var cell in cells)
            {
                if (!landmasses.TryAdd(cell, landmass)) continue;
                frontier.Enqueue(cell);
                while (frontier.Count > 0)
                {
                    var current = frontier.Dequeue();
                    // Direct diagonal steps are legal, so use the same adjacency to define an island.
                    for (var dy = -1; dy <= 1; dy++)
                        for (var dx = -1; dx <= 1; dx++)
                        {
                            var next = new Vector2Int(current.x + dx, current.y + dy);
                            if (validCells.Contains(next) && landmasses.TryAdd(next, landmass)) frontier.Enqueue(next);
                        }
                }
                landmass++;
            }
            return landmasses;
        }

        private void AddOuterSeams(Dictionary<Vector2Int, int> landmasses, bool horizontal)
        {
            var extents = new Dictionary<(int Landmass, int Line), (Vector2Int First, Vector2Int Last)>();
            // Cells are sorted by row then column, so each line's endpoints arrive in order.
            foreach (var cell in cells)
            {
                var key = (landmasses[cell], horizontal ? cell.y : cell.x);
                var first = extents.TryGetValue(key, out var existing) ? existing.First : cell;
                extents[key] = (first, cell);
            }
            var direction = horizontal ? Vector2Int.left : Vector2Int.down;
            foreach (var endpoints in extents.Values)
                ConnectOuterEndpoints(endpoints.First, direction, endpoints.Last);
        }

        public int Width { get; }
        public int Height { get; }
        public int CellCount { get; }
        public bool IsRectangle { get; }
        public string GenerationInputs { get; }
        public Vector2Int[] CopyCells() => (Vector2Int[])cells.Clone();
        public MapBoundaryConnection[] CopyConnections() => (MapBoundaryConnection[])connections.Clone();
        public bool IsValidCell(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height
            && (IsRectangle || validCells.Contains(new Vector2Int(x, y)));

        public bool TryResolve(int x, int y, Vector2Int offset, out Vector2Int target)
        {
            target = new Vector2Int(x, y);
            if (!IsValidCell(x, y)) return false;
            if (IsRectangle)
            {
                target = new Vector2Int(Modulo((long)x + offset.x, Width), Modulo((long)y + offset.y, Height));
                return true;
            }
            var remainingX = offset.x;
            var remainingY = offset.y;
            while (remainingX != 0 || remainingY != 0)
            {
                var direction = new Vector2Int(Math.Sign(remainingX), Math.Sign(remainingY));
                var adjacent = new Vector2Int(target.x + direction.x, target.y + direction.y);
                if (validCells.Contains(adjacent)) target = adjacent;
                else if (boundaries.TryGetValue((target, direction), out var wrapped)) target = wrapped;
                else return false;
                remainingX -= direction.x;
                remainingY -= direction.y;
            }
            return true;
        }

        public int GetDistance(int x, int y, int targetX, int targetY)
        {
            if (!IsValidCell(x, y) || !IsValidCell(targetX, targetY)) return int.MaxValue;
            if (IsRectangle)
            {
                var dx = Math.Abs(targetX - x);
                var dy = Math.Abs(targetY - y);
                return Math.Max(Math.Min(dx, Width - dx), Math.Min(dy, Height - dy));
            }
            var origin = new Vector2Int(x, y);
            if (!distances.TryGetValue(origin, out var found))
            {
                found = new Dictionary<Vector2Int, int> { [origin] = 0 };
                var queue = new Queue<Vector2Int>();
                queue.Enqueue(origin);
                while (queue.Count > 0)
                {
                    var current = queue.Dequeue();
                    for (var dy = -1; dy <= 1; dy++)
                        for (var dx = -1; dx <= 1; dx++)
                            if ((dx != 0 || dy != 0) && TryResolve(current.x, current.y, new Vector2Int(dx, dy), out var next)
                                && !found.ContainsKey(next))
                            {
                                found.Add(next, found[current] + 1);
                                queue.Enqueue(next);
                            }
                }
                distances.Add(origin, found);
            }
            return found.TryGetValue(new Vector2Int(targetX, targetY), out var distance) ? distance : int.MaxValue;
        }

        public bool WrapEdges => true;
        public string Fingerprint => Identity;
        public bool IsPlayableCell(int x, int y) => IsValidCell(x, y);
        public IEnumerable<Vector2Int> PlayableCoordinates
        {
            get
            {
                if (!IsRectangle)
                {
                    foreach (var cell in cells) yield return cell;
                    yield break;
                }
                for (var y = 0; y < Height; y++)
                    for (var x = 0; x < Width; x++) yield return new Vector2Int(x, y);
            }
        }
        public bool TryResolveCoordinates(ref int x, ref int y)
        {
            if (IsRectangle)
            {
                x = Modulo(x, Width);
                y = Modulo(y, Height);
            }
            return IsValidCell(x, y);
        }
        public bool TryResolveOffset(int x, int y, Vector2Int offset, out Vector2Int target) => TryResolve(x, y, offset, out target);
        public GridPattern GetPattern(GridPattern pattern)
        {
            if (!IsRectangle) throw new InvalidOperationException("Shape patterns require their origin.");
            return rectangularGrid.GetPattern(pattern);
        }
        public GridPattern GetPattern(int x, int y, GridPattern pattern)
        {
            if (IsRectangle) return GetPattern(pattern);
            var offsets = new List<Vector2Int>();
            var visited = new HashSet<Vector2Int> { new Vector2Int(x, y) };
            foreach (var offset in pattern.Offsets)
                if (TryResolve(x, y, offset, out var target) && visited.Add(target)) offsets.Add(offset);
            return new GridPattern(offsets);
        }

        public string Identity
        {
            get
            {
                var result = new StringBuilder("map-topology-v1|");
                result.Append(Width).Append('|').Append(Height).Append('|').Append(GenerationInputs.Length).Append(':').Append(GenerationInputs);
                result.Append(IsRectangle ? "|rectangle" : "|cells");
                foreach (var cell in cells) result.Append('|').Append(cell.x).Append(',').Append(cell.y);
                foreach (var pair in connections) result.Append('|').Append(Key(pair));
                return result.ToString();
            }
        }

        void ConnectOuterEndpoints(Vector2Int first, Vector2Int direction, Vector2Int last)
        {
            var reverse = new Vector2Int(-direction.x, -direction.y);
            if (boundaries.ContainsKey((first, direction)))
            {
                if (!boundaries.ContainsKey((last, reverse)))
                    throw new ArgumentException("Custom seams must pair every outer boundary endpoint.");
                return;
            }
            AddBoundary(first, direction, last);
            AddBoundary(last, reverse, first);
        }

        void AddBoundary(Vector2Int source, Vector2Int direction, Vector2Int target)
        {
            if (boundaries.TryGetValue((source, direction), out var existing))
            {
                if (existing != target) throw new ArgumentException("Conflicting boundary pairings.");
                return;
            }
            boundaries.Add((source, direction), target);
        }

        static string Key(MapBoundaryConnection pair) => string.Format(CultureInfo.InvariantCulture,
            "{0},{1}:{2},{3}:{4},{5}", pair.source.x, pair.source.y, pair.direction.x, pair.direction.y, pair.target.x, pair.target.y);
        static int Modulo(long value, int size) => (int)((value % size + size) % size);
    }
}
