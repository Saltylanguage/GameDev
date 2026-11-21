using System;
using System.Collections.Generic;
using UnityEngine;

namespace SaltyGame
{
    /// <summary>Defines playable grid cells, seam resolution, topology-aware distance, and pattern mapping.</summary>
    public interface IGridTopology
    {
        int Width { get; }
        int Height { get; }
        int CellCount { get; }
        bool WrapEdges { get; }
        string Fingerprint { get; }
        bool IsPlayableCell(int x, int y);
        bool TryResolveCoordinates(ref int x, ref int y);
        /// <summary>Resolves a pattern offset relative to its origin, including any shape-specific seam crossing.</summary>
        bool TryResolveOffset(int originX, int originY, Vector2Int offset, out Vector2Int target);
        int GetDistance(int x, int y, int targetX, int targetY);
        GridPattern GetPattern(GridPattern pattern);
        GridPattern GetPattern(int originX, int originY, GridPattern pattern);
        IEnumerable<Vector2Int> PlayableCoordinates { get; }
    }

    public sealed class RectangularGridTopology : IGridTopology
    {
        readonly Dictionary<GridPattern, GridPattern> wrappedPatterns = new Dictionary<GridPattern, GridPattern>();

        public RectangularGridTopology(int width, int height, bool wrapEdges = false)
        {
            if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width), width, "Grid width must be greater than zero.");
            if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height), height, "Grid height must be greater than zero.");
            Width = width;
            Height = height;
            WrapEdges = wrapEdges;
            Fingerprint = $"rectangular-grid-v1:{width}x{height}:wrap={wrapEdges}";
        }

        public int Width { get; }
        public int Height { get; }
        public int CellCount => checked(Width * Height);
        public bool WrapEdges { get; }
        public string Fingerprint { get; }
        public IEnumerable<Vector2Int> PlayableCoordinates
        {
            get
            {
                for (var y = 0; y < Height; y++)
                    for (var x = 0; x < Width; x++)
                        yield return new Vector2Int(x, y);
            }
        }

        public bool IsPlayableCell(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;

        public bool TryResolveCoordinates(ref int x, ref int y)
        {
            if (!WrapEdges) return IsPlayableCell(x, y);
            x = Modulo(x, Width);
            y = Modulo(y, Height);
            return true;
        }

        public bool TryResolveOffset(int originX, int originY, Vector2Int offset, out Vector2Int target)
        {
            var x = originX + offset.x;
            var y = originY + offset.y;
            if (!TryResolveCoordinates(ref x, ref y) || !IsPlayableCell(x, y))
            {
                target = default;
                return false;
            }

            target = new Vector2Int(x, y);
            return true;
        }

        public int GetDistance(int x, int y, int targetX, int targetY)
        {
            return Math.Max(Math.Abs(GetAxisOffset(targetX - x, Width)), Math.Abs(GetAxisOffset(targetY - y, Height)));
        }

        public GridPattern GetPattern(GridPattern pattern)
        {
            if (pattern == null) throw new ArgumentNullException(nameof(pattern));
            if (!WrapEdges) return pattern;
            if (wrappedPatterns.TryGetValue(pattern, out var cached)) return cached;
            var offsets = new List<Vector2Int>();
            var visited = new HashSet<Vector2Int>();
            foreach (var offset in pattern.Offsets)
            {
                var key = new Vector2Int(Modulo(offset.x, Width), Modulo(offset.y, Height));
                if (key == Vector2Int.zero || !visited.Add(key)) continue;
                offsets.Add(new Vector2Int(GetAxisOffset(offset.x, Width), GetAxisOffset(offset.y, Height)));
            }
            cached = new GridPattern(offsets);
            wrappedPatterns.Add(pattern, cached);
            wrappedPatterns.Add(cached, cached);
            return cached;
        }

        public GridPattern GetPattern(int originX, int originY, GridPattern pattern) => GetPattern(pattern);

        int GetAxisOffset(int offset, int size)
        {
            if (!WrapEdges) return offset;
            offset %= size;
            if (offset > size / 2) offset -= size;
            if (offset < -size / 2) offset += size;
            return offset;
        }

        static int Modulo(int value, int size) => (value % size + size) % size;
    }

    public sealed class Grid<T>
    {
        readonly T[] cells;
        readonly IGridTopology topology;

        public Grid(int width, int height, bool wrapEdges = false)
            : this(width, height, new RectangularGridTopology(width, height, wrapEdges))
        {
        }

        public Grid(int width, int height, IGridTopology topology)
        {
            if (width <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width), width, "Grid width must be greater than zero.");
            }

            if (height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height), height, "Grid height must be greater than zero.");
            }

            if (topology == null) throw new ArgumentNullException(nameof(topology));
            if (topology.Width != width || topology.Height != height)
                throw new ArgumentException("Topology dimensions must match the grid dimensions.", nameof(topology));

            Width = width;
            Height = height;
            this.topology = topology;
            cells = new T[checked(width * height)];
        }

        public Grid(int width, int height, Func<int, int, T> createCell, bool wrapEdges = false)
            : this(width, height, wrapEdges)
        {
            if (createCell == null)
            {
                throw new ArgumentNullException(nameof(createCell));
            }

            foreach (var position in topology.PlayableCoordinates)
                cells[GetIndex(position.x, position.y)] = createCell(position.x, position.y);
        }

        public Grid(int width, int height, Func<int, int, T> createCell, IGridTopology topology)
            : this(width, height, topology)
        {
            if (createCell == null) throw new ArgumentNullException(nameof(createCell));
            foreach (var position in topology.PlayableCoordinates)
                cells[GetIndex(position.x, position.y)] = createCell(position.x, position.y);
        }

        Grid(int width, int height, T[] cells, IGridTopology topology)
        {
            Width = width;
            Height = height;
            this.cells = cells;
            this.topology = topology;
        }

        public int Width { get; }
        public int Height { get; }
        public bool WrapEdges => topology.WrapEdges;
        public int Count => topology.CellCount;
        public int StorageCount => cells.Length;
        public IGridTopology Topology => topology;
        public bool IsPlayableCell(int x, int y) => topology.IsPlayableCell(x, y);

        public bool IsInBounds(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        public bool TryResolveCoordinates(ref int x, ref int y)
        {
            return topology.TryResolveCoordinates(ref x, ref y) && topology.IsPlayableCell(x, y);
        }

        public int GetDistance(int x, int y, int targetX, int targetY)
        {
            return topology.GetDistance(x, y, targetX, targetY);
        }

        public GridPattern GetPattern(GridPattern pattern) => topology.GetPattern(pattern);

        public GridPattern GetPattern(int originX, int originY, GridPattern pattern) =>
            topology.GetPattern(originX, originY, pattern);

        public bool TryResolveOffset(int originX, int originY, Vector2Int offset, out Vector2Int target)
        {
            return topology.TryResolveOffset(originX, originY, offset, out target)
                && topology.IsPlayableCell(target.x, target.y);
        }

        public bool TryResolveOffset(int originX, int originY, Vector2Int offset, out int targetX, out int targetY)
        {
            var resolved = TryResolveOffset(originX, originY, offset, out var target);
            targetX = target.x;
            targetY = target.y;
            return resolved;
        }

        public bool TryGetCellAtOffset(int originX, int originY, Vector2Int offset, out T cell)
        {
            if (!TryResolveOffset(originX, originY, offset, out var target))
            {
                cell = default;
                return false;
            }

            cell = cells[GetIndex(target.x, target.y)];
            return true;
        }

        public T GetCell(int x, int y)
        {
            EnsureInBounds(ref x, ref y);
            return cells[GetIndex(x, y)];
        }

        public bool TryGetCell(int x, int y, out T cell)
        {
            if (!TryResolveCoordinates(ref x, ref y))
            {
                cell = default;
                return false;
            }

            if (!topology.IsPlayableCell(x, y))
            {
                cell = default;
                return false;
            }

            cell = cells[GetIndex(x, y)];
            return true;
        }

        public void SetCell(int x, int y, T cell)
        {
            EnsureInBounds(ref x, ref y);
            cells[GetIndex(x, y)] = cell;
        }

        public bool TrySetCell(int x, int y, T cell)
        {
            if (!TryResolveCoordinates(ref x, ref y))
            {
                return false;
            }

            if (!topology.IsPlayableCell(x, y)) return false;

            cells[GetIndex(x, y)] = cell;
            return true;
        }

        public Grid<T> Copy()
        {
            var copiedCells = new T[cells.Length];
            Array.Copy(cells, copiedCells, cells.Length);
            return new Grid<T>(Width, Height, copiedCells, topology);
        }

        public Grid<T> Copy(Func<T, T> copyCell)
        {
            if (copyCell == null)
            {
                throw new ArgumentNullException(nameof(copyCell));
            }

            var copiedCells = new T[cells.Length];
            foreach (var position in topology.PlayableCoordinates)
            {
                var index = GetIndex(position.x, position.y);
                copiedCells[index] = copyCell(cells[index]);
            }

            return new Grid<T>(Width, Height, copiedCells, topology);
        }

        int GetIndex(int x, int y)
        {
            return x + y * Width;
        }

        void EnsureInBounds(ref int x, ref int y)
        {
            if (!TryResolveCoordinates(ref x, ref y) || !topology.IsPlayableCell(x, y))
            {
                throw new ArgumentOutOfRangeException(nameof(x), $"Grid location ({x}, {y}) is outside the {Width} x {Height} grid.");
            }
        }
    }
}
