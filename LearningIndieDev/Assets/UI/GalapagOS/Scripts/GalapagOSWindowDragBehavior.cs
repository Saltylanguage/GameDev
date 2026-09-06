using Noesis;
using NoesisApp;

namespace SaltyGame
{
    public sealed class GalapagOSWindowDragBehavior : Behavior<FrameworkElement>
    {
        const float HeaderHeight = 36.0f;
        const float RightControlWidth = 40.0f;

        TranslateTransform _transform;
        Point _relativePosition;

        protected override void OnAttached()
        {
            AssociatedObject.PreviewMouseLeftButtonDown += OnMouseLeftButtonDown;
            AssociatedObject.LostMouseCapture += OnLostMouseCapture;
        }

        protected override void OnDetaching()
        {
            EndDrag();

            FrameworkElement element = AssociatedObject;
            if (element != null)
            {
                element.PreviewMouseLeftButtonDown -= OnMouseLeftButtonDown;
                element.LostMouseCapture -= OnLostMouseCapture;
                element.ReleaseMouseCapture();
            }

            _transform = null;
        }

        void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point position = e.GetPosition(AssociatedObject);
            if (position.Y < 0.0f || position.Y > HeaderHeight ||
                position.X > AssociatedObject.ActualWidth - RightControlWidth)
            {
                return;
            }

            _relativePosition = position;
            _transform = AssociatedObject.RenderTransform as TranslateTransform ?? new TranslateTransform();
            AssociatedObject.RenderTransform = _transform;
            AssociatedObject.MouseMove += OnMouseMove;
            AssociatedObject.MouseLeftButtonUp += OnMouseLeftButtonUp;

            if (AssociatedObject.CaptureMouse())
            {
                e.Handled = true;
            }
            else
            {
                EndDrag();
            }
        }

        void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AssociatedObject.ReleaseMouseCapture();
        }

        void OnMouseMove(object sender, MouseEventArgs e)
        {
            Point position = e.GetPosition(AssociatedObject);
            Vector delta = position - _relativePosition;
            Vector constrainedDelta = ConstrainToParent(delta);

            _transform.X += constrainedDelta.X;
            _transform.Y += constrainedDelta.Y;
            e.Handled = true;
        }

        void OnLostMouseCapture(object sender, MouseEventArgs e)
        {
            EndDrag();
        }

        void EndDrag()
        {
            FrameworkElement element = AssociatedObject;
            if (element == null)
            {
                return;
            }

            element.MouseMove -= OnMouseMove;
            element.MouseLeftButtonUp -= OnMouseLeftButtonUp;
        }

        Vector ConstrainToParent(Vector delta)
        {
            FrameworkElement element = AssociatedObject;
            FrameworkElement parent = element.Parent;
            if (parent == null || parent.ActualWidth <= 0.0f || parent.ActualHeight <= 0.0f)
            {
                return delta;
            }

            Point topLeft = element.TranslatePoint(new Point(0.0f, 0.0f), parent);
            float maxX = parent.ActualWidth - element.ActualWidth;
            float maxY = parent.ActualHeight - element.ActualHeight;

            return new Vector(
                Clamp(delta.X, -topLeft.X, maxX - topLeft.X),
                Clamp(delta.Y, -topLeft.Y, maxY - topLeft.Y));
        }

        static float Clamp(float value, float minimum, float maximum)
        {
            if (minimum > maximum)
            {
                return 0.0f;
            }

            return value < minimum ? minimum : value > maximum ? maximum : value;
        }
    }
}
