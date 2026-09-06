using System;
using System.Windows.Input;
using Noesis;
using NoesisApp;

namespace SaltyGame
{
    public static class GalapagOSDesktopIcon
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached(
                "Command",
                typeof(ICommand),
                typeof(GalapagOSDesktopIcon));

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached(
                "CommandParameter",
                typeof(object),
                typeof(GalapagOSDesktopIcon));

        public static void SetCommand(DependencyObject element, ICommand value)
        {
            element.SetValue(CommandProperty, value);
        }

        public static ICommand GetCommand(DependencyObject element)
        {
            return (ICommand)element.GetValue(CommandProperty);
        }

        public static void SetCommandParameter(DependencyObject element, object value)
        {
            element.SetValue(CommandParameterProperty, value);
        }

        public static object GetCommandParameter(DependencyObject element)
        {
            return element.GetValue(CommandParameterProperty);
        }
    }

    public sealed class GalapagOSDesktopIconBehavior : Behavior<FrameworkElement>
    {
        public float GridSize
        {
            get { return (float)GetValue(GridSizeProperty); }
            set { SetValue(GridSizeProperty, value); }
        }

        public static readonly DependencyProperty GridSizeProperty =
            DependencyProperty.Register(
                "GridSize",
                typeof(float),
                typeof(GalapagOSDesktopIconBehavior),
                new PropertyMetadata(16.0f));

        public bool SnapToGrid
        {
            get { return (bool)GetValue(SnapToGridProperty); }
            set { SetValue(SnapToGridProperty, value); }
        }

        public static readonly DependencyProperty SnapToGridProperty =
            DependencyProperty.Register(
                "SnapToGrid",
                typeof(bool),
                typeof(GalapagOSDesktopIconBehavior),
                new PropertyMetadata(true));

        public bool ConstrainToParentBounds
        {
            get { return (bool)GetValue(ConstrainToParentBoundsProperty); }
            set { SetValue(ConstrainToParentBoundsProperty, value); }
        }

        public static readonly DependencyProperty ConstrainToParentBoundsProperty =
            DependencyProperty.Register(
                "ConstrainToParentBounds",
                typeof(bool),
                typeof(GalapagOSDesktopIconBehavior),
                new PropertyMetadata(true));

        public float DragThreshold
        {
            get { return (float)GetValue(DragThresholdProperty); }
            set { SetValue(DragThresholdProperty, value); }
        }

        public static readonly DependencyProperty DragThresholdProperty =
            DependencyProperty.Register(
                "DragThreshold",
                typeof(float),
                typeof(GalapagOSDesktopIconBehavior),
                new PropertyMetadata(4.0f));

        protected override void OnAttached()
        {
            AssociatedObject.PreviewMouseLeftButtonDown += OnMouseLeftButtonDown;
            AssociatedObject.LostMouseCapture += OnLostMouseCapture;
        }

        protected override void OnDetaching()
        {
            EndDrag(true);

            FrameworkElement element = AssociatedObject;
            if (element != null)
            {
                element.PreviewMouseLeftButtonDown -= OnMouseLeftButtonDown;
                element.ReleaseMouseCapture();
            }
        }

        void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!AssociatedObject.IsEnabled)
            {
                return;
            }

            if (e.ClickCount >= 2)
            {
                ExecuteCommand();
                e.Handled = true;
                return;
            }

            EndDrag(false);
            // ponytail: Canvas-only positioning keeps icon movement predictable; add another layout adapter if desktop icons move to a different panel type.
            _canvas = AssociatedObject.Parent as Canvas;
            if (_canvas == null)
            {
                e.Handled = true;
                return;
            }

            float startLeft = GetCanvasPosition(Canvas.GetLeft(AssociatedObject));
            float startTop = GetCanvasPosition(Canvas.GetTop(AssociatedObject));
            _initialPointer = e.GetPosition(_canvas);
            _grabOffset = new Vector(
                _initialPointer.X - startLeft,
                _initialPointer.Y - startTop);
            _dragging = false;

            AssociatedObject.MouseMove += OnMouseMove;
            AssociatedObject.MouseLeftButtonUp += OnMouseLeftButtonUp;

            if (AssociatedObject.CaptureMouse())
            {
                e.Handled = true;
            }
            else
            {
                EndDrag(false);
            }
        }

        void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_canvas == null || e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }

            Point pointer = e.GetPosition(_canvas);
            Vector movement = pointer - _initialPointer;
            if (!_dragging && movement.X * movement.X + movement.Y * movement.Y < DragThreshold * DragThreshold)
            {
                return;
            }

            _dragging = true;
            SetPosition(
                pointer.X - _grabOffset.X,
                pointer.Y - _grabOffset.Y);
            e.Handled = true;
        }

        void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AssociatedObject.ReleaseMouseCapture();
            EndDrag(true);
            e.Handled = true;
        }

        void OnLostMouseCapture(object sender, MouseEventArgs e)
        {
            EndDrag(true);
        }

        void ExecuteCommand()
        {
            ICommand command = GalapagOSDesktopIcon.GetCommand(AssociatedObject);
            if (command == null)
            {
                return;
            }

            object parameter = GalapagOSDesktopIcon.GetCommandParameter(AssociatedObject);
            if (command.CanExecute(parameter))
            {
                command.Execute(parameter);
            }
        }

        void SetPosition(float left, float top)
        {
            if (ConstrainToParentBounds)
            {
                float maxLeft = _canvas.ActualWidth - AssociatedObject.ActualWidth;
                float maxTop = _canvas.ActualHeight - AssociatedObject.ActualHeight;
                if (maxLeft >= 0.0f)
                {
                    left = Clamp(left, 0.0f, maxLeft);
                }
                if (maxTop >= 0.0f)
                {
                    top = Clamp(top, 0.0f, maxTop);
                }
            }

            Canvas.SetLeft(AssociatedObject, left);
            Canvas.SetTop(AssociatedObject, top);
        }

        void SnapPosition()
        {
            if (_canvas == null || !SnapToGrid || GridSize <= 0.0f)
            {
                return;
            }

            float left = GetCanvasPosition(Canvas.GetLeft(AssociatedObject));
            float top = GetCanvasPosition(Canvas.GetTop(AssociatedObject));
            float snappedLeft = (float)(Math.Round(left / GridSize, MidpointRounding.AwayFromZero) * GridSize);
            float snappedTop = (float)(Math.Round(top / GridSize, MidpointRounding.AwayFromZero) * GridSize);
            SetPosition(snappedLeft, snappedTop);
        }

        void EndDrag(bool snap)
        {
            if (snap && _dragging)
            {
                SnapPosition();
            }

            FrameworkElement element = AssociatedObject;
            if (element != null)
            {
                element.MouseMove -= OnMouseMove;
                element.MouseLeftButtonUp -= OnMouseLeftButtonUp;
            }

            _canvas = null;
            _dragging = false;
        }

        static float GetCanvasPosition(float position)
        {
            return float.IsNaN(position) || float.IsInfinity(position) ? 0.0f : position;
        }

        static float Clamp(float value, float minimum, float maximum)
        {
            return value < minimum ? minimum : value > maximum ? maximum : value;
        }

        Canvas _canvas;
        Point _initialPointer;
        Vector _grabOffset;
        bool _dragging;
    }
}
