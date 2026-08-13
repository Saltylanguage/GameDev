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
            // These animal silhouettes are traced from the user-provided reference sheet.
            // The source is a raster image, so there are no original vector paths to extract;
            // these polygons preserve the source iconography without introducing another asset
            // dependency. The sheet contains no plant glyph, so plants retain the leaf fallback.
            speciesPaths = new Dictionary<SpeciesId, Geometry>
            {
                [SpeciesIds.Plant] = plantPath,
                [new SpeciesId("fern")] = Geometry.Parse("M-0.05,0.46 L0.05,0.2 L-0.24,0.08 L0.06,0.02 L-0.22,-0.14 L0.08,-0.2 L0,-0.46 L0.22,-0.16 L0.24,0.16 L0.1,0.28 Z"),
                [new SpeciesId("reed")] = Geometry.Parse("M-0.06,0.46 L0.06,0.46 L0.06,-0.18 C0.08,-0.34 0.22,-0.44 0.34,-0.46 L0.4,-0.34 C0.22,-0.25 0.14,-0.16 0.14,0.46 Z"),
                [SpeciesIds.Herbivore] = Geometry.Parse("M-0.300,0.500 L0.333,0.119 L0.383,-0.048 L0.500,-0.167 L0.450,-0.381 L0.300,-0.500 L0.067,-0.500 L0.067,-0.405 L0.350,-0.381 L0.033,-0.357 L0.000,-0.262 L0.083,-0.143 L0.317,-0.095 L0.133,-0.071 L0.000,-0.167 L-0.067,-0.405 L-0.233,-0.405 L-0.167,-0.167 L-0.500,0.262 L-0.317,0.476 Z"),
                [new SpeciesId("hare")] = Geometry.Parse("M-0.300,0.500 L0.333,0.119 L0.383,-0.048 L0.500,-0.167 L0.450,-0.381 L0.300,-0.500 L0.067,-0.500 L0.067,-0.405 L0.350,-0.381 L0.033,-0.357 L0.000,-0.262 L0.083,-0.143 L0.317,-0.095 L0.133,-0.071 L0.000,-0.167 L-0.067,-0.405 L-0.233,-0.405 L-0.167,-0.167 L-0.500,0.262 L-0.317,0.476 Z"),
                [new SpeciesId("deer")] = Geometry.Parse("M-0.260,0.500 L-0.220,0.397 L-0.120,0.483 L-0.060,0.086 L0.260,0.086 L0.340,0.155 L0.500,0.086 L0.480,-0.000 L0.360,0.069 L0.420,-0.017 L0.380,-0.500 L0.280,-0.483 L0.280,-0.345 L0.240,-0.483 L0.180,-0.500 L0.200,-0.379 L0.100,-0.293 L0.020,-0.500 L-0.060,-0.500 L-0.060,-0.328 L-0.100,-0.500 L-0.200,-0.483 L-0.160,-0.345 L-0.360,-0.034 L-0.340,0.259 L-0.500,0.328 L-0.440,0.379 L-0.280,0.379 L-0.280,0.483 Z"),
                [new SpeciesId("snail")] = Geometry.Parse("M-0.432,0.500 L-0.076,0.446 L0.059,0.268 L0.398,0.089 L0.500,-0.125 L0.466,-0.339 L0.263,-0.482 L-0.025,-0.500 L-0.195,-0.464 L-0.449,-0.304 L-0.500,-0.214 L-0.314,-0.339 L0.059,-0.411 L0.195,-0.375 L0.297,-0.268 L0.025,-0.268 L0.161,-0.196 L0.110,-0.036 L0.280,-0.036 L0.212,-0.000 L0.110,-0.000 L0.059,-0.089 L-0.025,-0.036 L-0.110,-0.196 L-0.280,-0.196 L-0.195,-0.143 L-0.144,-0.018 L-0.297,-0.000 L-0.347,0.196 L-0.144,0.161 L-0.127,0.286 L-0.212,0.304 L-0.280,0.196 L-0.415,0.304 L-0.449,0.482 Z"),
                [new SpeciesId("beetle")] = Geometry.Parse("M-0.009,0.500 L0.179,0.278 L0.142,0.130 L0.292,0.241 L0.500,0.148 L0.462,0.111 L0.292,0.167 L-0.179,-0.296 L-0.123,-0.426 L-0.160,-0.500 L-0.236,-0.278 L-0.142,-0.148 L-0.292,-0.185 L-0.500,-0.019 L-0.406,-0.000 L-0.292,-0.111 L-0.217,-0.111 L0.104,0.204 L-0.009,0.463 Z"),
                [SpeciesIds.Carnivore] = Geometry.Parse("M-0.418,0.500 L-0.112,0.232 L-0.051,0.107 L0.255,0.089 L0.418,-0.018 L0.500,-0.339 L0.439,-0.357 L0.398,-0.500 L0.316,-0.500 L0.357,-0.357 L0.296,-0.304 L0.276,-0.500 L0.194,-0.500 L0.235,-0.393 L0.051,-0.232 L-0.071,-0.232 L-0.092,-0.500 L-0.194,-0.482 L-0.133,-0.429 L-0.194,-0.232 L-0.214,-0.464 L-0.337,-0.500 L-0.276,-0.304 L-0.500,0.071 L-0.500,0.393 L-0.439,0.482 Z"),
                [new SpeciesId("fox")] = Geometry.Parse("M-0.418,0.500 L-0.112,0.232 L-0.051,0.107 L0.255,0.089 L0.418,-0.018 L0.500,-0.339 L0.439,-0.357 L0.398,-0.500 L0.316,-0.500 L0.357,-0.357 L0.296,-0.304 L0.276,-0.500 L0.194,-0.500 L0.235,-0.393 L0.051,-0.232 L-0.071,-0.232 L-0.092,-0.500 L-0.194,-0.482 L-0.133,-0.429 L-0.194,-0.232 L-0.214,-0.464 L-0.337,-0.500 L-0.276,-0.304 L-0.500,0.071 L-0.500,0.393 L-0.439,0.482 Z"),
                [new SpeciesId("owl")] = Geometry.Parse("M-0.347,0.500 L-0.246,0.500 L-0.178,0.420 L-0.144,0.220 L-0.042,0.100 L0.331,-0.100 L0.076,-0.160 L-0.229,-0.120 L-0.178,-0.180 L0.280,-0.160 L0.500,-0.300 L0.415,-0.360 L0.025,-0.300 L-0.025,-0.360 L0.025,-0.500 L-0.347,-0.500 L-0.246,-0.420 L-0.246,-0.300 L-0.415,-0.180 L-0.449,0.020 L-0.398,0.320 L-0.500,0.360 L-0.364,0.480 Z"),
                [new SpeciesId("stoat")] = Geometry.Parse("M-0.232,0.500 L-0.089,0.500 L-0.036,0.440 L0.054,0.020 L0.446,-0.140 L0.500,-0.400 L0.429,-0.480 L0.196,-0.500 L0.304,-0.420 L0.268,-0.340 L0.250,-0.400 L-0.036,-0.400 L0.107,-0.440 L0.125,-0.500 L-0.125,-0.500 L-0.196,-0.340 L-0.214,-0.480 L-0.500,-0.480 L-0.321,-0.400 L-0.321,-0.320 L-0.429,-0.200 L-0.446,-0.020 L-0.304,0.300 L-0.393,0.440 L-0.250,0.480 Z"),
                [new SpeciesId("wolf")] = Geometry.Parse("M-0.439,0.500 L-0.173,0.482 L-0.071,0.395 L-0.031,0.254 L0.398,0.096 L0.500,-0.061 L0.500,-0.272 L0.316,-0.465 L-0.010,-0.500 L-0.316,-0.412 L-0.500,-0.254 L-0.480,-0.202 L-0.112,-0.377 L0.092,-0.377 L0.194,-0.289 Z"),
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
