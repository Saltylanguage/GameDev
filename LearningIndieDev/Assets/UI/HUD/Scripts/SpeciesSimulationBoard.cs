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
        const float FoxHuntCueDuration = 0.55f;
        const float MatingCueDuration = 0.82f;
        static readonly string[] HeartPixels =
        {
            ".##.##.",
            "#######",
            "#######",
            ".#####.",
            "..###..",
            "...#...",
        };

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

        SimulationBoardSnapshot snapshot;
        CroppedBitmap[] animalSprites;
        CroppedBitmap[] grassTerrainTiles;
        CroppedBitmap[] desertTerrainTiles;
        SpeciesId playerSpecies;
        float zoom = 1f;
        int foxHuntCueTick = -1;
        int foxHuntCueX;
        int foxHuntCueY;
        float foxHuntCueStartedAt;
        float nextFoxHuntCueFrame;
        bool foxHuntCueActive;
        int matingCueTick = -1;
        int matingCueX;
        int matingCueY;
        int matingCueMateX;
        int matingCueMateY;
        int matingCueOffspringX;
        int matingCueOffspringY;
        float matingCueStartedAt;
        float nextMatingCueFrame;
        bool matingCueActive;
        SolidColorBrush foxHuntShadowBrush;
        SolidColorBrush foxHuntAccentBrush;
        SolidColorBrush heartShadowBrush;
        SolidColorBrush heartFillBrush;
        SolidColorBrush sparkleBrush;

        /// <summary>Multiplier applied after fitting the grid to the board area.</summary>
        public float Zoom
        {
            get => zoom;
            set
            {
                var next = Mathf.Clamp(value, 0.5f, 2f);
                if (Mathf.Approximately(zoom, next))
                {
                    return;
                }

                zoom = next;
                InvalidateVisual();
            }
        }

        public void SetFoxHuntCue(int x, int y, int tick)
        {
            if (tick < 0)
            {
                foxHuntCueTick = -1;
                ClearFoxHuntCue();
                return;
            }

            if (tick == foxHuntCueTick)
            {
                return;
            }

            foxHuntCueTick = tick;
            foxHuntCueX = x;
            foxHuntCueY = y;
            foxHuntCueStartedAt = Time.unscaledTime;
            nextFoxHuntCueFrame = foxHuntCueStartedAt;
            foxHuntCueActive = true;
            InvalidateVisual();
        }

        public void ClearFoxHuntCue()
        {
            if (!foxHuntCueActive)
            {
                return;
            }

            foxHuntCueActive = false;
            InvalidateVisual();
        }

        public void UpdateFoxHuntCue()
        {
            if (!foxHuntCueActive)
            {
                return;
            }

            if (Time.unscaledTime - foxHuntCueStartedAt >= FoxHuntCueDuration)
            {
                ClearFoxHuntCue();
            }
            else if (Time.unscaledTime >= nextFoxHuntCueFrame)
            {
                nextFoxHuntCueFrame = Time.unscaledTime + 1f / 15f;
                InvalidateVisual();
            }
        }

        public void SetMatingCue(
            int parentX,
            int parentY,
            int mateX,
            int mateY,
            int offspringX,
            int offspringY,
            int tick)
        {
            if (tick < 0)
            {
                matingCueTick = -1;
                ClearMatingCue();
                return;
            }

            if (tick == matingCueTick)
            {
                return;
            }

            matingCueTick = tick;
            matingCueX = parentX;
            matingCueY = parentY;
            matingCueMateX = mateX;
            matingCueMateY = mateY;
            matingCueOffspringX = offspringX;
            matingCueOffspringY = offspringY;
            matingCueStartedAt = Time.unscaledTime;
            nextMatingCueFrame = matingCueStartedAt;
            matingCueActive = true;
            InvalidateVisual();
        }

        public void ClearMatingCue()
        {
            if (!matingCueActive)
            {
                return;
            }

            matingCueActive = false;
            InvalidateVisual();
        }

        public void UpdateMatingCue()
        {
            if (!matingCueActive)
            {
                return;
            }

            if (Time.unscaledTime - matingCueStartedAt >= MatingCueDuration)
            {
                ClearMatingCue();
            }
            else if (Time.unscaledTime >= nextMatingCueFrame)
            {
                nextMatingCueFrame = Time.unscaledTime + 1f / 30f;
                InvalidateVisual();
            }
        }

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

        public void SetSnapshot(SimulationBoardSnapshot nextSnapshot)
        {
            if (ReferenceEquals(snapshot, nextSnapshot))
            {
                return;
            }

            snapshot = nextSnapshot;
            playerSpecies = snapshot?.PlayerSpecies ?? default;
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

            if (snapshot == null || width <= 0f || height <= 0f)
            {
                return;
            }

            var cellSize = Math.Min(width / snapshot.Width, height / snapshot.Height) * Zoom;
            var boardWidth = cellSize * snapshot.Width;
            var boardHeight = cellSize * snapshot.Height;
            var left = (width - boardWidth) * 0.5f;
            var top = (height - boardHeight) * 0.5f;
            // Blob sprites are transparent overlays that must touch across
            // cell boundaries. Draw the diagnostic grid separately if needed;
            // a per-cell gap hides edge and diagonal continuity.
            const float gap = 0f;

            for (var y = 0; y < snapshot.Height; y++)
            {
                for (var x = 0; x < snapshot.Width; x++)
                {
                    var cell = snapshot.GetCell(x, y);
                    var cellTop = (snapshot.Height - 1 - y) * cellSize;
                    var cellRect = new NoesisRect(
                        left + x * cellSize + gap,
                        top + cellTop + gap,
                        Math.Max(0f, cellSize - gap * 2f),
                        Math.Max(0f, cellSize - gap * 2f));

                    DrawTerrain(context, cell, cellRect);
                    if (cell.IsCreature || (cell.IsPlantResource && !cell.IsTerrainResource))
                    {
                        DrawSpeciesSprite(context, cell, cellRect);
                    }

                }
            }

            DrawFoxHuntCue(context, left, top, cellSize);
            DrawMatingCue(context, left, top, cellSize);
        }

        void DrawFoxHuntCue(DrawingContext context, float left, float top, float cellSize)
        {
            if (!foxHuntCueActive || !snapshot.TryGetCell(foxHuntCueX, foxHuntCueY, out _))
            {
                return;
            }

            var progress = Mathf.Clamp01((Time.unscaledTime - foxHuntCueStartedAt) / FoxHuntCueDuration);
            var alpha = 1f - Mathf.SmoothStep(0f, 1f, progress);
            var pixel = Math.Max(2f, (float)Math.Round(cellSize * 0.075f));
            var pawWidth = pixel * 4f;
            var x = Mathf.Clamp(
                (float)Math.Round(left + (foxHuntCueX + 0.5f) * cellSize - pawWidth * 0.5f),
                left + 1f,
                left + snapshot.Width * cellSize - pawWidth - 1f);
            var cellTop = top + (snapshot.Height - 1 - foxHuntCueY) * cellSize;
            var y = (float)Math.Round(Math.Max(
                top + pixel,
                cellTop - pixel * 3f - Mathf.SmoothStep(0f, 1f, progress) * cellSize * 0.22f));

            var shadow = GetFoxHuntShadowBrush();
            var accent = GetFoxHuntAccentBrush();
            shadow.Opacity = alpha * 0.8f;
            accent.Opacity = alpha * 0.95f;
            DrawPawPrint(context, x, y, pixel, shadow);
            DrawPawPrint(context, x + pixel * 0.35f, y - pixel * 0.15f, pixel * 0.84f, accent);
        }

        void DrawMatingCue(DrawingContext context, float left, float top, float cellSize)
        {
            if (!matingCueActive
                || !snapshot.TryGetCell(matingCueX, matingCueY, out _)
                || !snapshot.TryGetCell(matingCueOffspringX, matingCueOffspringY, out _))
            {
                return;
            }

            var progress = Mathf.Clamp01((Time.unscaledTime - matingCueStartedAt) / MatingCueDuration);
            var fadeIn = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(progress / 0.22f));
            var fadeOut = 1f - Mathf.SmoothStep(0f, 1f, Mathf.Clamp01((progress - 0.7f) / 0.3f));
            var alpha = fadeIn * fadeOut;
            var scale = progress < 0.62f
                ? Mathf.Lerp(0.42f, 1f, Mathf.SmoothStep(0f, 1f, progress / 0.62f))
                : 1f + Mathf.Sin(Mathf.Clamp01((progress - 0.62f) / 0.38f) * Mathf.PI) * 0.18f;
            var pixel = Math.Max(2f, (float)Math.Round(cellSize * 0.16f));
            var parentTop = top + (snapshot.Height - 1 - matingCueY) * cellSize;
            var hasMateAnchor = snapshot.TryGetCell(matingCueMateX, matingCueMateY, out _);
            var mateAnchorX = hasMateAnchor ? matingCueMateX : matingCueX;
            var mateTop = hasMateAnchor
                ? top + (snapshot.Height - 1 - matingCueMateY) * cellSize
                : parentTop;
            var centerX = left + ((matingCueX + 0.5f) + (mateAnchorX + 0.5f)) * cellSize * 0.5f;
            var heartHeight = HeartPixels.Length * pixel * scale;
            var heartY = Mathf.Max(
                top + pixel,
                Mathf.Min(parentTop, mateTop) - heartHeight - pixel * 0.45f
                    - Mathf.SmoothStep(0f, 1f, progress) * cellSize * 0.10f);

            var shadow = GetHeartShadowBrush();
            var fill = GetHeartFillBrush();
            shadow.Opacity = alpha * 0.82f;
            fill.Opacity = alpha;
            DrawPixelArt(context, HeartPixels, centerX + pixel * 0.45f, heartY + pixel * 0.55f, pixel * scale, shadow);
            DrawPixelArt(context, HeartPixels, centerX, heartY, pixel * scale, fill);

            var childCenterX = left + (matingCueOffspringX + 0.5f) * cellSize;
            var childCenterY = top + (snapshot.Height - 1 - matingCueOffspringY + 0.5f) * cellSize;
            var sparkleProgress = Mathf.Clamp01(progress / 0.78f);
            var sparkleAlpha = (1f - sparkleProgress) * Mathf.SmoothStep(0f, 1f, sparkleProgress * 2f);
            var sparkleScale = Mathf.Lerp(0.45f, 1.15f, Mathf.SmoothStep(0f, 1f, sparkleProgress));
            var sparkle = GetSparkleBrush();
            sparkle.Opacity = sparkleAlpha;
            DrawSparkle(context, childCenterX, childCenterY, pixel * 1.5f * sparkleScale, sparkle);
        }

        static void DrawPawPrint(DrawingContext context, float x, float y, float pixel, Brush brush)
        {
            context.DrawRectangle(brush, null,
                new NoesisRect(x, y + pixel * 1.5f, pixel * 3f, pixel * 2f));
            context.DrawRectangle(brush, null,
                new NoesisRect(x - pixel * 0.5f, y + pixel * 0.5f, pixel, pixel));
            context.DrawRectangle(brush, null,
                new NoesisRect(x + pixel, y, pixel, pixel));
            context.DrawRectangle(brush, null,
                new NoesisRect(x + pixel * 2.5f, y + pixel * 0.5f, pixel, pixel));
        }

        static void DrawPixelArt(
            DrawingContext context,
            string[] pixels,
            float centerX,
            float top,
            float pixelSize,
            Brush brush)
        {
            var width = pixels[0].Length * pixelSize;
            var left = centerX - width * 0.5f;
            for (var row = 0; row < pixels.Length; row++)
            {
                for (var column = 0; column < pixels[row].Length; column++)
                {
                    if (pixels[row][column] != '#')
                    {
                        continue;
                    }

                    context.DrawRectangle(brush, null, new NoesisRect(
                        left + column * pixelSize,
                        top + row * pixelSize,
                        pixelSize,
                        pixelSize));
                }
            }
        }

        static void DrawSparkle(DrawingContext context, float centerX, float centerY, float size, Brush brush)
        {
            context.DrawRectangle(brush, null, new NoesisRect(centerX - size * 0.25f, centerY - size, size * 0.5f, size * 2f));
            context.DrawRectangle(brush, null, new NoesisRect(centerX - size, centerY - size * 0.25f, size * 2f, size * 0.5f));
            context.DrawEllipse(brush, null, new Noesis.Point(centerX, centerY), size * 0.32f, size * 0.32f);
        }

        SolidColorBrush GetFoxHuntShadowBrush()
        {
            return foxHuntShadowBrush ??= new SolidColorBrush(Noesis.Color.FromArgb(255, 92, 47, 33));
        }

        SolidColorBrush GetFoxHuntAccentBrush()
        {
            return foxHuntAccentBrush ??= new SolidColorBrush(Noesis.Color.FromArgb(255, 237, 177, 77));
        }

        SolidColorBrush GetHeartShadowBrush()
        {
            return heartShadowBrush ??= new SolidColorBrush(Noesis.Color.FromArgb(255, 91, 47, 53));
        }

        SolidColorBrush GetHeartFillBrush()
        {
            return heartFillBrush ??= new SolidColorBrush(Noesis.Color.FromArgb(255, 240, 128, 128));
        }

        SolidColorBrush GetSparkleBrush()
        {
            return sparkleBrush ??= new SolidColorBrush(Noesis.Color.FromArgb(255, 255, 244, 176));
        }

        void DrawTerrain(DrawingContext context, SimulationCellSnapshot cell, NoesisRect cellRect)
        {
            // Every passable tile samples its neighbors: Bare uses the Grass
            // mask too, so Grass vertices can fill dirt tiles inside a field.
            context.DrawRectangle(cell.IsPassable ? Brushes.SaddleBrown : Brushes.Black, null, cellRect);

            if (TerrainVisualFamilies.TryGetTerrainTileFamily(cell.TerrainId, out var family))
            {
                var tiles = family == TerrainVisualFamily.Grass
                    ? grassTerrainTiles
                    : desertTerrainTiles;
                if (tiles != null)
                {
                    DrawTerrainSprite(context, tiles, cell.TerrainVariantMask, cellRect);
                }
            }
        }

        void DrawSpeciesSprite(DrawingContext context, SimulationCellSnapshot cell, NoesisRect cellRect)
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

        int GetAnimalAtlasIndex(SimulationCellSnapshot cell)
        {
            if (AnimalAtlasIndexBySpecies.TryGetValue(cell.SpeciesId, out var index))
            {
                return index;
            }

            return GetSpeciesRole(cell) == SpeciesRole.Carnivore ? 0 : 4;
        }

        SpeciesRole GetSpeciesRole(SimulationCellSnapshot cell)
        {
            if (snapshot != null && snapshot.SpeciesRoles.TryGetValue(cell.SpeciesId, out var role))
            {
                return role;
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
