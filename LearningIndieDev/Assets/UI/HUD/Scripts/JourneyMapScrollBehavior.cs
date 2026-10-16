using System.ComponentModel;
using Noesis;
using NoesisApp;

namespace SaltyGame
{
    public sealed class JourneyMapScrollBehavior : Behavior<ScrollViewer>
    {
        VM_SimulationShell viewModel;

        protected override void OnAttached()
        {
            AssociatedObject.PreviewMouseWheel += OnMouseWheel;
            AssociatedObject.DataContextChanged += OnDataContextChanged;
            AssociatedObject.IsVisibleChanged += OnVisibilityChanged;
            AssociatedObject.Loaded += OnLoaded;
            AttachViewModel();
        }

        protected override void OnDetaching()
        {
            if (AssociatedObject != null)
            {
                AssociatedObject.PreviewMouseWheel -= OnMouseWheel;
                AssociatedObject.DataContextChanged -= OnDataContextChanged;
                AssociatedObject.IsVisibleChanged -= OnVisibilityChanged;
                AssociatedObject.Loaded -= OnLoaded;
            }
            if (viewModel != null) viewModel.PropertyChanged -= OnViewModelChanged;
            viewModel = null;
        }

        void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs args) => AttachViewModel();
        void OnVisibilityChanged(object sender, DependencyPropertyChangedEventArgs args) => FocusCurrentNode();
        void OnLoaded(object sender, RoutedEventArgs args) => FocusCurrentNode();

        void AttachViewModel()
        {
            if (viewModel != null) viewModel.PropertyChanged -= OnViewModelChanged;
            viewModel = AssociatedObject.DataContext as VM_SimulationShell;
            if (viewModel != null) viewModel.PropertyChanged += OnViewModelChanged;
            FocusCurrentNode();
        }

        void OnViewModelChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(VM_SimulationShell.JourneyMapFocusX)) FocusCurrentNode();
        }

        void FocusCurrentNode()
        {
            if (viewModel == null || !AssociatedObject.IsVisible) return;
            var viewport = AssociatedObject.ViewportWidth > 0f ? AssociatedObject.ViewportWidth : 1280f;
            AssociatedObject.ScrollToHorizontalOffset(viewModel.JourneyMapFocusX - viewport * 0.35f);
        }

        void OnMouseWheel(object sender, MouseWheelEventArgs args)
        {
            AssociatedObject.ScrollToHorizontalOffset(AssociatedObject.HorizontalOffset - args.Delta * (2f / 3f));
            args.Handled = true;
        }
    }
}
