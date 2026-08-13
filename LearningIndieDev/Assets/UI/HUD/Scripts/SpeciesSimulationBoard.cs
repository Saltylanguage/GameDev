using System;
using System.Collections.Generic;
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
        IReadOnlyDictionary<SpeciesId, SpeciesRules> speciesRules;
        Dictionary<SpeciesId, Geometry> speciesPaths;
        Geometry plantPath;
        Geometry herbivorePath;
        Geometry carnivorePath;
        MatrixTransform[] vectorTransforms;

        public void SetSpeciesRules(IReadOnlyDictionary<SpeciesId, SpeciesRules> rules)
        {
            speciesRules = rules;
            InvalidateVisual();
        }

        public void SetGrid(Grid<SpeciesCell> grid)
        {
            cells = grid;
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext context)
        {
            var width = ActualWidth > 0f ? ActualWidth : Width;
            var height = ActualHeight > 0f ? ActualHeight : Height;
            context.DrawRectangle(Brushes.Transparent, null, new Rect(0f, 0f, width, height));

            if (cells == null || width <= 0f || height <= 0f)
            {
                return;
            }

            EnsureVectorTransforms(cells.Width, cells.Height);
            var cellSize = Math.Min(width / cells.Width, height / cells.Height);
            var boardWidth = cellSize * cells.Width;
            var boardHeight = cellSize * cells.Height;
            var left = (width - boardWidth) * 0.5f;
            var top = (height - boardHeight) * 0.5f;
            var gap = Math.Min(1.5f, cellSize * 0.08f);

            for (var y = 0; y < cells.Height; y++)
            {
                for (var x = 0; x < cells.Width; x++)
                {
                    var cell = cells.GetCell(x, y);
                    var cellTop = (cells.Height - 1 - y) * cellSize;
                    var cellRect = new Rect(
                        left + x * cellSize + gap,
                        top + cellTop + gap,
                        Math.Max(0f, cellSize - gap * 2f),
                        Math.Max(0f, cellSize - gap * 2f));
                    context.DrawRectangle(
                        cell.IsTerrainResource ? Brushes.ForestGreen : Brushes.SaddleBrown,
                        null,
                        cellRect);
                    if (cell.IsCreature || cell.IsPlantResource)
                    {
                        DrawSpeciesVector(context, cell, cellRect, y * cells.Width + x);
                    }
                }
            }
        }

        void DrawSpeciesVector(DrawingContext context, SpeciesCell cell, Rect cellRect, int cellIndex)
        {
            EnsureVectorPaths();
            var scale = Math.Min(cellRect.Width, cellRect.Height);
            var vectorTransform = vectorTransforms[cellIndex];
            vectorTransform.Matrix = new Matrix(
                scale, 0f, 0f, scale,
                cellRect.X + cellRect.Width * 0.5f,
                cellRect.Y + cellRect.Height * 0.5f);
            context.PushTransform(vectorTransform);
            context.DrawGeometry(GetSpeciesBrush(cell), null, GetSpeciesPath(cell));
            context.Pop();
        }

        void EnsureVectorTransforms(int width, int height)
        {
            var count = width * height;
            if (vectorTransforms != null && vectorTransforms.Length == count)
            {
                return;
            }

            vectorTransforms = new MatrixTransform[count];
            for (var index = 0; index < count; index++)
            {
                vectorTransforms[index] = new MatrixTransform();
            }
        }

        void EnsureVectorPaths()
        {
            if (plantPath != null)
            {
                return;
            }

            plantPath = Geometry.Parse("M0,-0.48 C0.2,-0.34 0.48,-0.24 0.45,-0.02 C0.42,0.2 0.18,0.3 0.02,0.1 C-0.12,0.34 -0.36,0.3 -0.46,0.1 C-0.56,-0.12 -0.28,-0.3 0,-0.48 Z");
            herbivorePath = Geometry.Parse("M-0.38,0.04 L-0.3,-0.46 L-0.1,-0.2 C-0.04,-0.23 0.04,-0.23 0.1,-0.2 L0.3,-0.46 L0.38,0.04 C0.38,0.3 0.2,0.46 0,0.46 C-0.2,0.46 -0.38,0.3 -0.38,0.04 Z");
            carnivorePath = Geometry.Parse("M0,-0.48 L0.18,-0.2 L0.46,-0.24 L0.3,0.04 L0.44,0.4 L0,0.24 L-0.44,0.4 L-0.3,0.04 L-0.46,-0.24 L-0.18,-0.2 Z");
            speciesPaths = new Dictionary<SpeciesId, Geometry>
            {
                [SpeciesIds.Plant] = plantPath,
                [new SpeciesId("fern")] = Geometry.Parse("M-0.05,0.46 L0.05,0.2 L-0.24,0.08 L0.06,0.02 L-0.22,-0.14 L0.08,-0.2 L0,-0.46 L0.22,-0.16 L0.24,0.16 L0.1,0.28 Z"),
                [new SpeciesId("reed")] = Geometry.Parse("M-0.06,0.46 L0.06,0.46 L0.06,-0.18 C0.08,-0.34 0.22,-0.44 0.34,-0.46 L0.4,-0.34 C0.22,-0.25 0.14,-0.16 0.14,0.46 Z"),
                [SpeciesIds.Herbivore] = herbivorePath,
                [new SpeciesId("hare")] = Geometry.Parse("M-0.34,0.08 L-0.3,-0.46 L-0.1,-0.2 C-0.04,-0.24 0.04,-0.24 0.1,-0.2 L0.3,-0.46 L0.34,0.08 C0.3,0.34 0.18,0.46 0,0.46 C-0.18,0.46 -0.3,0.34 -0.34,0.08 Z"),
                [new SpeciesId("deer")] = Geometry.Parse("M-0.4,-0.04 L-0.24,-0.22 L-0.34,-0.46 L-0.24,-0.48 L-0.12,-0.26 L0,-0.48 L0.1,-0.42 L0.06,-0.22 L0.4,-0.04 L0.26,0.24 L0,0.46 L-0.26,0.24 Z"),
                [new SpeciesId("snail")] = Geometry.Parse("M-0.46,0.26 L-0.28,0.26 C-0.42,-0.26 0.26,-0.5 0.34,-0.02 C0.4,0.24 0.1,0.4 -0.06,0.2 C-0.2,0.02 0.02,-0.2 0.16,-0.04 C0.24,0.06 0.14,0.18 0.02,0.12 L0.28,0.26 L0.46,0.26 L0.46,0.4 L-0.46,0.4 Z"),
                [new SpeciesId("beetle")] = Geometry.Parse("M0,-0.46 C0.34,-0.42 0.46,-0.12 0.34,0.24 C0.24,0.44 0.1,0.48 0,0.48 C-0.1,0.48 -0.24,0.44 -0.34,0.24 C-0.46,-0.12 -0.34,-0.42 0,-0.46 Z M0,-0.4 L0,0.4"),
                [SpeciesIds.Carnivore] = carnivorePath,
                [new SpeciesId("fox")] = Geometry.Parse("M-0.46,-0.22 L-0.28,-0.48 L-0.08,-0.3 L0.08,-0.3 L0.28,-0.48 L0.46,-0.22 L0.28,0.14 L0.08,0.46 L-0.08,0.46 L-0.28,0.14 Z"),
                [new SpeciesId("owl")] = Geometry.Parse("M-0.46,-0.22 L-0.28,-0.46 L0,-0.24 L0.28,-0.46 L0.46,-0.22 L0.36,0.3 C0.22,0.46 -0.22,0.46 -0.36,0.3 Z"),
                [new SpeciesId("stoat")] = Geometry.Parse("M-0.48,0.06 L-0.26,-0.18 L0.2,-0.18 L0.48,0.06 L0.32,0.26 L0.08,0.18 L-0.22,0.3 Z"),
                [new SpeciesId("wolf")] = Geometry.Parse("M-0.48,-0.12 L-0.3,-0.42 L-0.1,-0.26 L0.1,-0.26 L0.3,-0.42 L0.48,-0.12 L0.36,0.3 C0.2,0.46 -0.2,0.46 -0.36,0.3 Z"),
            };
        }

        Geometry GetSpeciesPath(SpeciesCell cell)
        {
            if (speciesPaths.TryGetValue(cell.SpeciesId, out var speciesPath))
            {
                return speciesPath;
            }

            switch (GetSpeciesRole(cell))
            {
                case SpeciesRole.Plant:
                    return plantPath;
                case SpeciesRole.Carnivore:
                    return carnivorePath;
                default:
                    return herbivorePath;
            }
        }

        Brush GetSpeciesBrush(SpeciesCell cell)
        {
            switch (GetSpeciesRole(cell))
            {
                case SpeciesRole.Plant:
                    return Brushes.LimeGreen;
                case SpeciesRole.Carnivore:
                    return Brushes.Crimson;
                default:
                    return Brushes.LightSkyBlue;
            }
        }

        SpeciesRole GetSpeciesRole(SpeciesCell cell)
        {
            if (speciesRules != null && speciesRules.TryGetValue(cell.SpeciesId, out var rules))
            {
                return rules.Role;
            }

            if (cell.SpeciesId == SpeciesIds.Plant)
            {
                return SpeciesRole.Plant;
            }

            return cell.SpeciesId == SpeciesIds.Carnivore
                ? SpeciesRole.Carnivore
                : SpeciesRole.Herbivore;
        }
    }
}
