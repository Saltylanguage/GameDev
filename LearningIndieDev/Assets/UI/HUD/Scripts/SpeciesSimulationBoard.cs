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
        const int AtlasTileSize = 128;
        const int TerrainFamilyTileOffset = 16;
        const string AnimalAtlasResource = "CellularArt/Animals_01_SpriteSheet";
        const string TerrainAtlasResource = "CellularArt/Terrain_01_SpriteSheet";

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
        TextureSource animalAtlas;
        TextureSource terrainAtlas;
        CroppedBitmap[] animalSprites;
        CroppedBitmap[] terrainTiles;
        bool warnedMissingAtlases;

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
            context.DrawRectangle(Brushes.Transparent, null, new NoesisRect(0f, 0f, width, height));

            if (cells == null || width <= 0f || height <= 0f)
            {
                return;
            }

            EnsureSpriteAtlases();
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
                }
            }
        }

        void DrawTerrain(DrawingContext context, SpeciesCell cell, NoesisRect cellRect, int x, int y)
        {
            if (TryGetTerrainFamilyOffset(cell.TerrainId, out var familyOffset)
                && terrainTiles != null)
            {
                var tileIndex = TerrainTileResolver.ResolveTerrainTileIndex(cells, x, y, cell.TerrainId)
                    + familyOffset;
                context.DrawImage(terrainTiles[tileIndex], cellRect);
                return;
            }

            context.DrawRectangle(cell.IsPassable ? Brushes.SaddleBrown : Brushes.Black, null, cellRect);
        }

        void DrawSpeciesSprite(DrawingContext context, SpeciesCell cell, NoesisRect cellRect)
        {
            if (cell.IsPlantResource && !cell.IsTerrainResource)
            {
                if (terrainTiles != null)
                {
                    context.DrawImage(terrainTiles[0], cellRect);
                }

                return;
            }

            if (animalSprites == null)
            {
                return;
            }

            context.DrawImage(animalSprites[GetAnimalAtlasIndex(cell)], cellRect);
        }

        void EnsureSpriteAtlases()
        {
            if (animalSprites != null || warnedMissingAtlases)
            {
                return;
            }

            var animalTexture = UnityEngine.Resources.Load<Texture2D>(AnimalAtlasResource);
            var terrainTexture = UnityEngine.Resources.Load<Texture2D>(TerrainAtlasResource);
            if (animalTexture == null || terrainTexture == null)
            {
                warnedMissingAtlases = true;
                Debug.LogWarning(
                    $"SpeciesSimulationBoard could not load '{AnimalAtlasResource}' or '{TerrainAtlasResource}'.");
                return;
            }

            animalAtlas = new TextureSource(animalTexture);
            terrainAtlas = new TextureSource(terrainTexture);
            animalSprites = CreateSprites(animalAtlas, 8, 4);
            terrainTiles = CreateSprites(terrainAtlas, 32, 4);
        }

        static bool TryGetTerrainFamilyOffset(TerrainId terrainId, out int offset)
        {
            if (terrainId == TerrainIds.Grass)
            {
                offset = 0;
                return true;
            }

            // Bare temporarily uses the desert family until a dedicated bare
            // ground atlas is authored. Keep the simulation TerrainId intact.
            if (terrainId == TerrainIds.Bare || terrainId == TerrainIds.Desert)
            {
                offset = TerrainFamilyTileOffset;
                return true;
            }

            offset = 0;
            return false;
        }

        static CroppedBitmap[] CreateSprites(BitmapSource source, int count, int columns)
        {
            var sprites = new CroppedBitmap[count];
            for (var index = 0; index < count; index++)
            {
                sprites[index] = new CroppedBitmap(
                    source,
                    new Int32Rect(
                        (index % columns) * AtlasTileSize,
                        (index / columns) * AtlasTileSize,
                        AtlasTileSize,
                        AtlasTileSize));
            }

            return sprites;
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
