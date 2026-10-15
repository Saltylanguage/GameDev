using Noesis;
using NoesisApp;

namespace SaltyGame
{
    public sealed class JourneyNodeHoverBehavior : Behavior<FrameworkElement>
    {
        protected override void OnAttached()
        {
            AssociatedObject.MouseEnter += OnMouseEnter;
            AssociatedObject.MouseLeave += OnMouseLeave;
        }

        protected override void OnDetaching()
        {
            if (AssociatedObject != null)
            {
                AssociatedObject.MouseEnter -= OnMouseEnter;
                AssociatedObject.MouseLeave -= OnMouseLeave;
            }
        }

        void OnMouseEnter(object sender, MouseEventArgs args)
        {
            var node = AssociatedObject.DataContext as JourneyMapNodeItem;
            if (node?.HoverCommand?.CanExecute(null) == true)
            {
                for (DependencyObject parent = AssociatedObject; parent != null;
                    parent = VisualTreeHelper.GetParent(parent))
                {
                    if (parent is ScrollViewer scroll)
                    {
                        node.PlaceHoverCard(scroll.HorizontalOffset, scroll.ViewportWidth);
                        break;
                    }
                }
                node.HoverCommand.Execute(null);
            }
        }

        void OnMouseLeave(object sender, MouseEventArgs args)
        {
            var node = AssociatedObject.DataContext as JourneyMapNodeItem;
            if (node?.LeaveCommand?.CanExecute(null) == true)
            {
                node.LeaveCommand.Execute(null);
            }
        }
    }
}
