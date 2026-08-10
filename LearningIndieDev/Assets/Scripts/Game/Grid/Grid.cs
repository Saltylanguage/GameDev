using System;

namespace SaltyGame
{
    public sealed class Grid<T>
    {
        readonly T[] cells;

        public Grid(int width, int height)
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
            cells = new T[checked(width * height)];
        }

        public Grid(int width, int height, Func<int, int, T> createCell)
            : this(width, height)
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

        Grid(int width, int height, T[] cells)
        {
            Width = width;
            Height = height;
            this.cells = cells;
        }

        public int Width { get; }
        public int Height { get; }
        public int Count => cells.Length;

        public bool IsInBounds(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        public T GetCell(int x, int y)
        {
            EnsureInBounds(x, y);
            return cells[GetIndex(x, y)];
        }

        public bool TryGetCell(int x, int y, out T cell)
        {
            if (!IsInBounds(x, y))
            {
                cell = default;
                return false;
            }

            cell = cells[GetIndex(x, y)];
            return true;
        }

        public void SetCell(int x, int y, T cell)
        {
            EnsureInBounds(x, y);
            cells[GetIndex(x, y)] = cell;
        }

        public bool TrySetCell(int x, int y, T cell)
        {
            if (!IsInBounds(x, y))
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
            return new Grid<T>(Width, Height, copiedCells);
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

            return new Grid<T>(Width, Height, copiedCells);
        }

        int GetIndex(int x, int y)
        {
            return x + y * Width;
        }

        void EnsureInBounds(int x, int y)
        {
            if (!IsInBounds(x, y))
            {
                throw new ArgumentOutOfRangeException(nameof(x), $"Grid location ({x}, {y}) is outside the {Width} x {Height} grid.");
            }
        }
    }
}
