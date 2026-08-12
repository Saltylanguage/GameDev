using System;
using Noesis;

namespace SaltyGame
{
    /// <summary>
    /// Batched Noesis renderer for the simulation grid. It deliberately draws
    /// the whole board in one control instead of creating one visual per cell.
    /// </summary>
    public sealed class SpeciesSimulationBoard : FrameworkElement
    {
        Grid<SpeciesCell> cells;

        public void SetGrid(Grid<SpeciesCell> grid)
        {
            cells = grid;
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext context)
        {
            var width = ActualWidth > 0f ? ActualWidth : Width;
            var height = ActualHeight > 0f ? ActualHeight : Height;
            context.DrawRectangle(Brushes.SaddleBrown, null, new Rect(0f, 0f, width, height));

            if (cells == null || width <= 0f || height <= 0f)
            {
                return;
            }

            var cellWidth = width / cells.Width;
            var cellHeight = height / cells.Height;
            var gap = Math.Min(1.5f, Math.Min(cellWidth, cellHeight) * 0.08f);

            for (var y = 0; y < cells.Height; y++)
            {
                for (var x = 0; x < cells.Width; x++)
                {
                    var cell = cells.GetCell(x, y);
                    var top = (cells.Height - 1 - y) * cellHeight;
                    context.DrawRectangle(
                        GetBrush(cell),
                        null,
                        new Rect(
                            x * cellWidth + gap,
                            top + gap,
                            Math.Max(0f, cellWidth - gap * 2f),
                            Math.Max(0f, cellHeight - gap * 2f)));
                }
            }
        }

        static Brush GetBrush(SpeciesCell cell)
        {
            if (cell.IsCreature)
            {
                if (cell.SpeciesId == SpeciesIds.Herbivore)
                {
                    return Brushes.Orange;
                }

                if (cell.SpeciesId == SpeciesIds.Carnivore)
                {
                    return Brushes.Crimson;
                }

                return Brushes.LightSkyBlue;
            }

            return cell.IsPlantResource ? Brushes.ForestGreen : Brushes.SaddleBrown;
        }
    }
}
