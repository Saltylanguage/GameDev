using System;
using System.Collections.Generic;
using Noesis;
using UnityEngine;
using NoesisRect = Noesis.Rect;

namespace SaltyGame
{
    /// <summary>
    /// Batched Noesis renderer for the simulation grid. It deliberately draws
    /// the whole board in one control instead of creating one visual per cell.
    /// </summary>
    public sealed class SpeciesSimulationBoard : FrameworkElement
    {
        static readonly Dictionary<SpeciesId, int> AnimalAtlasIndexBySpecies =
            new Dictionary<SpeciesId, int>
            {
                [new SpeciesId("wolf")] = 0,
                [new SpeciesId("fox")] = 1,
                [new SpeciesId("eagle")] = 2,
                [new SpeciesId("shark")] = 3,
                [new SpeciesId("deer")] = 4,
                [new SpeciesId("hare")] = 5,
                [new SpeciesId("cow")] = 6,
                [new SpeciesId("elephant")] = 7,
            };

        Grid<SpeciesCell> cells;
        IReadOnlyDictionary<SpeciesId, SpeciesRules> speciesRules;
        CroppedBitmap[] animalSprites;
        CroppedBitmap[] grassTerrainTiles;
        CroppedBitmap[] desertTerrainTiles;
        SpeciesId playerSpecies;
        Pen playerSpeciesOutline;

        public void SetSpriteVisuals(CroppedBitmap[] animals, CroppedBitmap[] grassTerrain, CroppedBitmap[] desertTerrain)
        {
            if (ReferenceEquals(animalSprites, animals)
                && ReferenceEquals(grassTerrainTiles, grassTerrain)
                && ReferenceEquals(desertTerrainTiles, desertTerrain))
            {
                return;
            }

            animalSprites = animals;
            grassTerrainTiles = grassTerrain;
            desertTerrainTiles = desertTerrain;
            InvalidateVisual();
        }

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

        public void SetPlayerSpecies(SpeciesId species)
        {
            if (playerSpecies == species)
            {
                return;
            }

            playerSpecies = species;
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext context)
        {
            var width = ActualWidth > 0f ? ActualWidth : Width;
            var height = ActualHeight > 0f ? ActualHeight : Height;
            context.DrawRectangle(Brushes.Transparent, null, new NoesisRect(0f, 0f, width, height));

            if (cells == null || width <= 0f || height <= 0f)
            {
                return;
            }

            var cellSize = Math.Min(width / cells.Width, height / cells.Height);
            var boardWidth = cellSize * cells.Width;
            var boardHeight = cellSize * cells.Height;
            var left = (width - boardWidth) * 0.5f;
            var top = (height - boardHeight) * 0.5f;
            // Blob sprites are transparent overlays that must touch across
            // cell boundaries. Draw the diagnostic grid separately if needed;
            // a per-cell gap hides edge and diagonal continuity.
            const float gap = 0f;

            for (var y = 0; y < cells.Height; y++)
            {
                for (var x = 0; x < cells.Width; x++)
                {
                    var cell = cells.GetCell(x, y);
                    var cellTop = (cells.Height - 1 - y) * cellSize;
                    var cellRect = new NoesisRect(
                        left + x * cellSize + gap,
                        top + cellTop + gap,
                        Math.Max(0f, cellSize - gap * 2f),
                        Math.Max(0f, cellSize - gap * 2f));

                    DrawTerrain(context, cell, cellRect, x, y);
                    if (cell.IsCreature || (cell.IsPlantResource && !cell.IsTerrainResource))
                    {
                        DrawSpeciesSprite(context, cell, cellRect);
                    }

                    if (cell.IsCreature && cell.SpeciesId == playerSpecies)
                    {
                        DrawPlayerSpeciesOutline(context, cellRect, cellSize);
                    }
                }
            }
        }

        void DrawPlayerSpeciesOutline(Noesis.DrawingContext context, NoesisRect cellRect, float cellSize)
        {
            if (playerSpeciesOutline == null)
            {
                playerSpeciesOutline = new Pen { Brush = Brushes.Gold };
            }

            playerSpeciesOutline.Thickness = Math.Max(1f, cellSize * 0.09f);
            context.DrawRectangle(null, playerSpeciesOutline, cellRect);
        }

        void DrawTerrain(DrawingContext context, SpeciesCell cell, NoesisRect cellRect, int x, int y)
        {
            if (desertTerrainTiles != null)
            {
                DrawTerrainSprite(context, desertTerrainTiles, TerrainTileResolver.FullMask, cellRect);
                if (TerrainVisualFamilies.Get(cell.TerrainId) == TerrainVisualFamily.Grass
                    && grassTerrainTiles != null)
                {
                    var mask = TerrainTileResolver.ResolveTerrainMask(cells, x, y, cell.TerrainId);
                    DrawTerrainSprite(context, grassTerrainTiles, mask, cellRect);
                }

                return;
            }

            context.DrawRectangle(cell.IsPassable ? Brushes.SaddleBrown : Brushes.Black, null, cellRect);
        }

        void DrawSpeciesSprite(DrawingContext context, SpeciesCell cell, NoesisRect cellRect)
        {
            if (cell.IsPlantResource && !cell.IsTerrainResource)
            {
                if (grassTerrainTiles != null)
                {
                    DrawTerrainSprite(context, grassTerrainTiles, TerrainTileResolver.FullMask, cellRect);
                }

                return;
            }

            var index = GetAnimalAtlasIndex(cell);
            if (animalSprites == null
                || index < 0
                || index >= animalSprites.Length
                || animalSprites[index] == null)
            {
                return;
            }

            context.DrawImage(animalSprites[index], cellRect);
        }

        static void DrawTerrainSprite(
            DrawingContext context,
            CroppedBitmap[] sprites,
            int mask,
            NoesisRect cellRect)
        {
            if (mask >= 0 && mask < sprites.Length && sprites[mask] != null)
            {
                context.DrawImage(sprites[mask], cellRect);
            }
        }

        int GetAnimalAtlasIndex(SpeciesCell cell)
        {
            if (AnimalAtlasIndexBySpecies.TryGetValue(cell.SpeciesId, out var index))
            {
                return index;
            }

            return GetSpeciesRole(cell) == SpeciesRole.Carnivore ? 0 : 4;
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
