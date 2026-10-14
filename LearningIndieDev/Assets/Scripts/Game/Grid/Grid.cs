using System;
using System.Collections.Generic;
using UnityEngine;

namespace SaltyGame
{
    public sealed class Grid<T>
    {
        readonly T[] cells;
        readonly Dictionary<GridPattern, GridPattern> wrappedPatterns;

        public Grid(int width, int height, bool wrapEdges = false)
        {
            if (width <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width), width, "Grid width must be greater than zero.");
            }

            if (height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height), height, "Grid height must be greater than zero.");
            }

            Width = width;
            Height = height;
            WrapEdges = wrapEdges;
            wrappedPatterns = new Dictionary<GridPattern, GridPattern>();
            cells = new T[checked(width * height)];
        }

        public Grid(int width, int height, Func<int, int, T> createCell, bool wrapEdges = false)
            : this(width, height, wrapEdges)
        {
            if (createCell == null)
            {
                throw new ArgumentNullException(nameof(createCell));
            }

            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    cells[GetIndex(x, y)] = createCell(x, y);
                }
            }
        }

        Grid(int width, int height, T[] cells, bool wrapEdges, Dictionary<GridPattern, GridPattern> wrappedPatterns)
        {
            Width = width;
            Height = height;
            this.cells = cells;
            WrapEdges = wrapEdges;
            this.wrappedPatterns = wrappedPatterns;
        }

        public int Width { get; }
        public int Height { get; }
        public bool WrapEdges { get; }
        public int Count => cells.Length;

        public bool IsInBounds(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        public bool TryResolveCoordinates(ref int x, ref int y)
        {
            if (!WrapEdges) return IsInBounds(x, y);
            x = Modulo(x, Width);
            y = Modulo(y, Height);
            return true;
        }

        public int GetDistance(int x, int y, int targetX, int targetY)
        {
            return Math.Max(Math.Abs(GetAxisOffset(targetX - x, Width)), Math.Abs(GetAxisOffset(targetY - y, Height)));
        }

        public GridPattern GetPattern(GridPattern pattern)
        {
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

        int GetAxisOffset(int offset, int size)
        {
            if (!WrapEdges) return offset;
            offset %= size;
            if (offset > size / 2) offset -= size;
            if (offset < -size / 2) offset += size;
            return offset;
        }

        static int Modulo(int value, int size) => (value % size + size) % size;

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

            cells[GetIndex(x, y)] = cell;
            return true;
        }

        public Grid<T> Copy()
        {
            var copiedCells = new T[cells.Length];
            Array.Copy(cells, copiedCells, cells.Length);
            return new Grid<T>(Width, Height, copiedCells, WrapEdges, wrappedPatterns);
        }

        public Grid<T> Copy(Func<T, T> copyCell)
        {
            if (copyCell == null)
            {
                throw new ArgumentNullException(nameof(copyCell));
            }

            var copiedCells = new T[cells.Length];
            for (var index = 0; index < cells.Length; index++)
            {
                copiedCells[index] = copyCell(cells[index]);
            }

            return new Grid<T>(Width, Height, copiedCells, WrapEdges, wrappedPatterns);
        }

        int GetIndex(int x, int y)
        {
            return x + y * Width;
        }

        void EnsureInBounds(ref int x, ref int y)
        {
            if (!TryResolveCoordinates(ref x, ref y))
            {
                throw new ArgumentOutOfRangeException(nameof(x), $"Grid location ({x}, {y}) is outside the {Width} x {Height} grid.");
            }
        }
    }
}
