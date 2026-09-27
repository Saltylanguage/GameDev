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
        internal const float MinimumZoomScale = 0.75f;
        internal const float MaximumZoomScale = 4f;
        const float ZoomPerWheelNotch = 1.15f;
        const float FoxHuntCueDuration = 0.55f;
        const float MatingCueDuration = 1.1f;
        static readonly string[] HeartPixels =
        {
            ".##.##.",
            "#######",
            "#######",
            ".#####.",
            "..###..",
            "...#...",
        };
        static readonly string[] BirthPoofPixels =
        {
            "..##..##..",
            ".########.",
            "##########",
            ".########.",
            "...####...",
        };
        static readonly string[] CanopyPixels =
        {
            "....11211....",
            "..112222221..",
            ".12223232221.",
            "1223223223221",
            "1222323322221",
            "1223223223221",
            ".12222222221.",
            "..111222111..",
            "....11111....",
        };
        static readonly string[] RockPixels =
        {
            "...1111...",
            ".11222211.",
            "1222222221",
            "1222323221",
            ".12222221.",
            "..111111..",
        };
        static readonly float[] CanopyPositions =
        {
            0.025f, 0.11f, 0.21f, 0.34f, 0.50f, 0.66f, 0.79f, 0.90f, 0.975f,
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
        float matingCuePauseStartedAt;
        bool matingCueActive;
        bool matingCuePaused;
        IReadOnlyList<SpeciesBirthEvent> birthCues = Array.Empty<SpeciesBirthEvent>();
        IReadOnlyList<HuntFootprint> huntFootprints = Array.Empty<HuntFootprint>();
        readonly MatrixTransform panZoomTransform;
        float panOffsetX;
        float panOffsetY;
        Point lastPanPosition;
        bool isPanning;
        SolidColorBrush foxHuntShadowBrush;
        SolidColorBrush foxHuntAccentBrush;
        SolidColorBrush heartShadowBrush;
        SolidColorBrush heartFillBrush;
        SolidColorBrush sparkleBrush;
        SolidColorBrush birthPoofShadowBrush;
        SolidColorBrush birthPoofFillBrush;
        SolidColorBrush canopyShadowBrush;
        SolidColorBrush canopyFillBrush;
        SolidColorBrush canopyLightBrush;
        SolidColorBrush meadowBrush;
        SolidColorBrush flowerBrush;
        SolidColorBrush pebbleBrush;
        SolidColorBrush rockShadowBrush;
        SolidColorBrush rockFillBrush;
        SolidColorBrush rockLightBrush;

        public event Action<float> ZoomRequested;

        public SpeciesSimulationBoard()
        {
            panZoomTransform = new MatrixTransform();
            panZoomTransform.Matrix = new Matrix(1f, 0f, 0f, 1f, 0f, 0f);
            RenderTransform = panZoomTransform;
            MouseLeftButtonDown += OnPanStarted;
            MouseWheel += OnZoom;
            LostMouseCapture += OnLostMouseCapture;
            Unloaded += OnBoardUnloaded;
        }

        /// <summary>Multiplier applied after fitting the grid to the board area.</summary>
        public float Zoom
        {
            get => zoom;
            set
            {
                var next = Mathf.Clamp(value, MinimumZoomScale, MaximumZoomScale);
                if (Mathf.Approximately(zoom, next))
                {
                    return;
                }

                zoom = next;
                ConstrainPanToViewport();
                ApplyPanZoomTransform();
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
            matingCuePaused = false;
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
            matingCuePaused = false;
            InvalidateVisual();
        }

        public void SetBirthCues(IReadOnlyList<SpeciesBirthEvent> births)
        {
            birthCues = births ?? Array.Empty<SpeciesBirthEvent>();
            InvalidateVisual();
        }

        public void SetHuntFootprints(IReadOnlyList<HuntFootprint> footprints)
        {
            huntFootprints = footprints ?? Array.Empty<HuntFootprint>();
            InvalidateVisual();
        }

        public void UpdateMatingCue()
        {
            UpdateMatingCue(false);
        }

        public void UpdateMatingCue(bool paused)
        {
            if (!matingCueActive)
            {
                return;
            }

            if (paused)
            {
                if (!matingCuePaused)
                {
                    matingCuePauseStartedAt = Time.unscaledTime;
                    matingCuePaused = true;
                }

                return;
            }

            if (matingCuePaused)
            {
                var pausedDuration = Time.unscaledTime - matingCuePauseStartedAt;
                matingCueStartedAt += pausedDuration;
                nextMatingCueFrame += pausedDuration;
                matingCuePaused = false;
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
            ConstrainPanToViewport();
            ApplyPanZoomTransform();
            InvalidateVisual();
        }

        void OnPanStarted(object sender, MouseButtonEventArgs e)
        {
            var viewport = Parent as UIElement;
            if (viewport == null)
            {
                return;
            }

            lastPanPosition = e.GetPosition(viewport);
            if (!CaptureMouse())
            {
                return;
            }

            isPanning = true;
            MouseMove += OnPanMoved;
            MouseLeftButtonUp += OnPanEnded;
            e.Handled = true;
        }

        void OnPanMoved(object sender, MouseEventArgs e)
        {
            if (!isPanning)
            {
                return;
            }

            if (e.LeftButton != MouseButtonState.Pressed)
            {
                ReleaseMouseCapture();
                EndPan();
                return;
            }

            var viewport = Parent as UIElement;
            if (viewport == null)
            {
                return;
            }

            var position = e.GetPosition(viewport);
            panOffsetX += position.X - lastPanPosition.X;
            panOffsetY += position.Y - lastPanPosition.Y;
            lastPanPosition = position;
            ConstrainPanToViewport();
            ApplyPanZoomTransform();
            e.Handled = true;
        }

        void OnPanEnded(object sender, MouseButtonEventArgs e)
        {
            ReleaseMouseCapture();
            EndPan();
            e.Handled = true;
        }

        void OnLostMouseCapture(object sender, MouseEventArgs e)
        {
            EndPan();
        }

        void OnBoardUnloaded(object sender, RoutedEventArgs e)
        {
            EndPan();
            ReleaseMouseCapture();
        }

        void EndPan()
        {
            if (!isPanning)
            {
                return;
            }

            isPanning = false;
            MouseMove -= OnPanMoved;
            MouseLeftButtonUp -= OnPanEnded;
        }

        void OnZoom(object sender, MouseWheelEventArgs e)
        {
            if (snapshot == null || e.Delta == 0)
            {
                return;
            }

            var viewport = Parent as UIElement;
            if (viewport == null)
            {
                return;
            }

            var oldZoom = Zoom;
            var nextZoom = Mathf.Clamp(
                oldZoom * Mathf.Pow(ZoomPerWheelNotch, e.Delta / 120f),
                MinimumZoomScale,
                MaximumZoomScale);
            if (Mathf.Approximately(nextZoom, oldZoom))
            {
                return;
            }

            var width = ActualWidth > 0f ? ActualWidth : Width;
            var height = ActualHeight > 0f ? ActualHeight : Height;
            if (width <= 0f || height <= 0f)
            {
                return;
            }

            GetBoardLayout(width, height, out var oldCellSize, out var oldLeft, out var oldTop);
            if (oldCellSize <= 0f)
            {
                return;
            }

            var position = e.GetPosition(viewport);
            var contentX = (position.X - panOffsetX - oldLeft) / oldCellSize;
            var contentY = (position.Y - panOffsetY - oldTop) / oldCellSize;
            if (ZoomRequested != null)
            {
                ZoomRequested(nextZoom);
            }
            else
            {
                Zoom = nextZoom;
            }

            GetBoardLayout(width, height, out var newCellSize, out var newLeft, out var newTop);
            panOffsetX = position.X - newLeft - contentX * newCellSize;
            panOffsetY = position.Y - newTop - contentY * newCellSize;
            ConstrainPanToViewport();
            ApplyPanZoomTransform();
            e.Handled = true;
        }

        void ConstrainPanToViewport()
        {
            if (snapshot == null || snapshot.Width <= 0 || snapshot.Height <= 0)
            {
                return;
            }

            var viewport = Parent as FrameworkElement;
            var viewportWidth = viewport != null ? viewport.ActualWidth : ActualWidth;
            var viewportHeight = viewport != null ? viewport.ActualHeight : ActualHeight;
            var width = ActualWidth > 0f ? ActualWidth : Width;
            var height = ActualHeight > 0f ? ActualHeight : Height;
            if (viewportWidth <= 0f || viewportHeight <= 0f || width <= 0f || height <= 0f)
            {
                return;
            }

            GetBoardLayout(width, height, out var cellSize, out var left, out var top);
            var right = left + cellSize * snapshot.Width;
            var bottom = top + cellSize * snapshot.Height;
            ConstrainOffset(viewportWidth, left, right, ref panOffsetX);
            ConstrainOffset(viewportHeight, top, bottom, ref panOffsetY);
        }

        void GetBoardLayout(float width, float height, out float cellSize, out float left, out float top)
        {
            cellSize = Math.Min(width / snapshot.Width, height / snapshot.Height) * Zoom;
            left = (width - cellSize * snapshot.Width) * 0.5f;
            top = (height - cellSize * snapshot.Height) * 0.5f;
        }

        static void ConstrainOffset(float viewportSize, float contentStart, float contentEnd, ref float offset)
        {
            var minimumOffset = viewportSize - contentEnd;
            var maximumOffset = -contentStart;
            offset = minimumOffset > maximumOffset
                ? (viewportSize - contentStart - contentEnd) * 0.5f
                : Mathf.Clamp(offset, minimumOffset, maximumOffset);
        }

        void ApplyPanZoomTransform()
        {
            panZoomTransform.Matrix = new Matrix(1f, 0f, 0f, 1f, panOffsetX, panOffsetY);
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
                }
            }

            DrawForestEdge(context, left, top, cellSize);
            DrawHuntFootprints(context, left, top, cellSize);
            for (var y = 0; y < snapshot.Height; y++)
            {
                for (var x = 0; x < snapshot.Width; x++)
                {
                    var cell = snapshot.GetCell(x, y);
                    var cellRect = new NoesisRect(
                        left + x * cellSize,
                        top + (snapshot.Height - 1 - y) * cellSize,
                        cellSize,
                        cellSize);
                    if (matingCueActive && IsBirthCueCell(x, y))
                    {
                        DrawBirthPoof(context, cellRect, cellSize);
                    }

                    if (cell.IsCreature || (cell.IsPlantResource && !cell.IsTerrainResource))
                    {
                        DrawSpeciesSprite(context, cell, cellRect);
                    }
                }
            }

            DrawFoxHuntCue(context, left, top, cellSize);
            DrawMatingCue(context, left, top, cellSize);
        }

        void DrawForestEdge(DrawingContext context, float left, float top, float cellSize)
        {
            // Crowns overhang from outside the playable field. They suggest a
            // forest boundary without marking traversable cells as walls.
            var pixel = Math.Max(2f, (float)Math.Round(cellSize * 0.28f));
            var shadow = canopyShadowBrush ??= new SolidColorBrush(Noesis.Color.FromArgb(255, 47, 72, 40));
            var fill = canopyFillBrush ??= new SolidColorBrush(Noesis.Color.FromArgb(255, 77, 112, 55));
            var light = canopyLightBrush ??= new SolidColorBrush(Noesis.Color.FromArgb(255, 114, 145, 70));
            shadow.Opacity = 0.92f;
            fill.Opacity = 0.85f;
            light.Opacity = 0.82f;
            for (var index = 0; index < CanopyPositions.Length; index++)
            {
                if (index == 4)
                {
                    continue;
                }

                var centerX = left + CanopyPositions[index] * snapshot.Width * cellSize;
                DrawCanopy(context, centerX, top - pixel * (7f + index % 2 * 0.35f),
                    pixel, shadow, fill, light);
            }

            var rockDark = rockShadowBrush ??= new SolidColorBrush(Noesis.Color.FromArgb(255, 68, 69, 59));
            var rockFill = rockFillBrush ??= new SolidColorBrush(Noesis.Color.FromArgb(255, 129, 127, 105));
            var rockLight = rockLightBrush ??= new SolidColorBrush(Noesis.Color.FromArgb(255, 174, 165, 129));
            for (var index = 0; index < 3; index++)
            {
                var centerY = top + (index + 0.5f) * snapshot.Height * cellSize / 3f;
                DrawCanopy(context, left - pixel * 6.2f, centerY - pixel * 4f,
                    pixel, shadow, fill, light);
                if (index != 1)
                {
                    DrawRockCluster(context, left - pixel * 5.5f,
                        centerY + (index == 0 ? cellSize * 0.46f : -cellSize * 0.52f),
                        pixel, rockDark, rockFill, rockLight);
                }
            }

            var meadow = meadowBrush ??= new SolidColorBrush(Noesis.Color.FromArgb(255, 91, 130, 57));
            var flower = flowerBrush ??= new SolidColorBrush(Noesis.Color.FromArgb(255, 238, 215, 130));
            var pebble = pebbleBrush ??= new SolidColorBrush(Noesis.Color.FromArgb(255, 135, 119, 100));
            for (var y = 0; y < snapshot.Height; y++)
            {
                for (var x = 0; x < snapshot.Width; x++)
                {
                    var cell = snapshot.GetCell(x, y);
                    var hash = unchecked((x * 73856093) ^ (y * 19349663));
                    var centerX = left + (x + 0.5f) * cellSize;
                    var centerY = top + (snapshot.Height - y - 0.5f) * cellSize;
                    if (cell.IsTerrainResource && cell.FoodReserve > 5f && hash % 19 == 0)
                    {
                        var blade = Math.Max(1f, (float)Math.Round(cellSize * 0.08f));
                        meadow.Opacity = 0.58f;
                        flower.Opacity = 0.84f;
                        context.DrawRectangle(meadow, null, new NoesisRect(centerX - blade * 2f, centerY, blade, blade * 3f));
                        context.DrawRectangle(meadow, null, new NoesisRect(centerX + blade, centerY - blade, blade, blade * 4f));
                        context.DrawRectangle(flower, null, new NoesisRect(centerX + blade, centerY - blade * 2f, blade, blade));
                    }
                    else if (!cell.IsTerrainResource && hash % 47 == 0)
                    {
                        // Low pebbles are a traversable ground detail.
                        var stone = Math.Max(1f, (float)Math.Round(cellSize * 0.12f));
                        pebble.Opacity = 0.62f;
                        context.DrawRectangle(pebble, null, new NoesisRect(centerX - stone, centerY, stone * 2f, stone));
                        context.DrawRectangle(pebble, null, new NoesisRect(centerX + stone, centerY + stone, stone, stone));
                    }
                }
            }
        }

        static void DrawCanopy(DrawingContext context, float centerX, float top,
            float pixel, Brush shadow, Brush fill, Brush light)
        {
            var left = centerX - CanopyPixels[0].Length * pixel * 0.5f;
            for (var row = 0; row < CanopyPixels.Length; row++)
            {
                for (var column = 0; column < CanopyPixels[row].Length; column++)
                {
                    Brush brush;
                    switch (CanopyPixels[row][column])
                    {
                        case '1': brush = shadow; break;
                        case '2': brush = fill; break;
                        case '3': brush = light; break;
                        default: continue;
                    }

                    context.DrawRectangle(brush, null,
                        new NoesisRect(left + column * pixel, top + row * pixel, pixel, pixel));
                }
            }
        }

        static void DrawRockCluster(DrawingContext context, float centerX, float top,
            float pixel, Brush shadow, Brush fill, Brush light)
        {
            var left = centerX - RockPixels[0].Length * pixel * 0.5f;
            for (var row = 0; row < RockPixels.Length; row++)
            {
                for (var column = 0; column < RockPixels[row].Length; column++)
                {
                    Brush brush;
                    switch (RockPixels[row][column])
                    {
                        case '1': brush = shadow; break;
                        case '2': brush = fill; break;
                        case '3': brush = light; break;
                        default: continue;
                    }

                    context.DrawRectangle(brush, null,
                        new NoesisRect(left + column * pixel, top + row * pixel, pixel, pixel));
                }
            }
        }

        bool IsBirthCueCell(int x, int y)
        {
            for (var index = 0; index < birthCues.Count; index++)
            {
                if (birthCues[index].ChildX == x && birthCues[index].ChildY == y)
                {
                    return true;
                }
            }

            return birthCues.Count == 0 && matingCueOffspringX == x && matingCueOffspringY == y;
        }

        void DrawHuntFootprints(DrawingContext context, float left, float top, float cellSize)
        {
            if (huntFootprints.Count == 0)
            {
                return;
            }

            var brush = GetFoxHuntShadowBrush();
            var pixel = Math.Max(1.5f, (float)Math.Round(cellSize * 0.08f));
            for (var index = 0; index < huntFootprints.Count; index++)
            {
                var footprint = huntFootprints[index];
                var age = snapshot.Tick - footprint.Tick;
                if (age < 0 || age >= 9 || !snapshot.TryGetCell(footprint.X, footprint.Y, out _))
                {
                    continue;
                }

                var stride = (index & 1) == 0 ? -1f : 1f;
                var offsetX = -footprint.DirectionY * stride * cellSize * 0.12f;
                var offsetY = -footprint.DirectionX * stride * cellSize * 0.12f;
                var centerX = left + (footprint.X + 0.5f) * cellSize + offsetX;
                var centerY = top + (snapshot.Height - footprint.Y - 0.5f) * cellSize + offsetY;
                brush.Opacity = (1f - age / 9f) * 0.78f;
                DrawPawPrint(context, centerX - pixel * 1.5f, centerY - pixel * 1.5f, pixel, brush);
            }
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

            var progress = GetMatingCueProgress();
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

            var sparkleProgress = Mathf.Clamp01(progress / 0.78f);
            var sparkleAlpha = (1f - sparkleProgress) * Mathf.SmoothStep(0f, 1f, sparkleProgress * 2f);
            var sparkleScale = Mathf.Lerp(0.45f, 1.15f, Mathf.SmoothStep(0f, 1f, sparkleProgress));
            var sparkle = GetSparkleBrush();
            sparkle.Opacity = sparkleAlpha;
            if (birthCues.Count == 0)
            {
                DrawBirthSparkle(context, matingCueOffspringX, matingCueOffspringY,
                    left, top, cellSize, pixel, sparkleScale, sparkle);
            }
            else
            {
                for (var index = 0; index < birthCues.Count; index++)
                {
                    DrawBirthSparkle(context, birthCues[index].ChildX, birthCues[index].ChildY,
                        left, top, cellSize, pixel, sparkleScale, sparkle);
                }
            }
        }

        void DrawBirthSparkle(DrawingContext context, int x, int y,
            float left, float top, float cellSize, float pixel, float scale, Brush brush)
        {
            if (!snapshot.TryGetCell(x, y, out _))
            {
                return;
            }

            DrawSparkle(context,
                left + (x + 0.5f) * cellSize,
                top + (snapshot.Height - y - 0.5f) * cellSize,
                pixel * 1.5f * scale,
                brush);
        }

        void DrawBirthPoof(DrawingContext context, NoesisRect cellRect, float cellSize)
        {
            if (!matingCueActive)
            {
                return;
            }

            var progress = GetMatingCueProgress();
            var poofProgress = Mathf.Clamp01(progress / 0.8f);
            var fadeIn = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(progress / 0.08f));
            var fadeOut = 1f - Mathf.SmoothStep(0f, 1f, poofProgress);
            var alpha = fadeIn * fadeOut;
            var scale = Mathf.Lerp(0.9f, 1.6f, Mathf.SmoothStep(0f, 1f, poofProgress));
            var pixel = Math.Max(2f, (float)Math.Round(cellSize * 0.1f));
            var centerX = cellRect.X + cellRect.Width * 0.5f;
            var height = BirthPoofPixels.Length * pixel * scale;
            var top = cellRect.Y + (cellRect.Height - height) * 0.5f;
            var shadow = GetBirthPoofShadowBrush();
            var fill = GetBirthPoofFillBrush();
            shadow.Opacity = alpha * 0.7f;
            fill.Opacity = alpha;
            DrawPixelArt(
                context,
                BirthPoofPixels,
                centerX + pixel * 0.35f,
                top + pixel * 0.35f,
                pixel * scale,
                shadow);
            DrawPixelArt(context, BirthPoofPixels, centerX, top, pixel * scale, fill);
        }

        float GetMatingCueProgress()
        {
            var currentTime = matingCuePaused ? matingCuePauseStartedAt : Time.unscaledTime;
            return Mathf.Clamp01((currentTime - matingCueStartedAt) / MatingCueDuration);
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

        SolidColorBrush GetBirthPoofShadowBrush()
        {
            return birthPoofShadowBrush ??= new SolidColorBrush(Noesis.Color.FromArgb(255, 103, 80, 52));
        }

        SolidColorBrush GetBirthPoofFillBrush()
        {
            return birthPoofFillBrush ??= new SolidColorBrush(Noesis.Color.FromArgb(255, 255, 246, 215));
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
