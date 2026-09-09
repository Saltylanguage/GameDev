using System.Windows.Input;
using Noesis;

namespace SaltyGame
{
    public static class GalapagOSWindow
    {
        public static readonly DependencyProperty IconSourceProperty =
            DependencyProperty.RegisterAttached(
                "IconSource",
                typeof(ImageSource),
                typeof(GalapagOSWindow));

        public static readonly DependencyProperty CloseCommandProperty =
            DependencyProperty.RegisterAttached(
                "CloseCommand",
                typeof(ICommand),
                typeof(GalapagOSWindow));

        public static readonly DependencyProperty CloseEnabledProperty =
            DependencyProperty.RegisterAttached(
                "CloseEnabled",
                typeof(bool),
                typeof(GalapagOSWindow),
                new PropertyMetadata(true));

        public static void SetIconSource(DependencyObject element, ImageSource value)
        {
            element.SetValue(IconSourceProperty, value);
        }

        public static ImageSource GetIconSource(DependencyObject element)
        {
            return (ImageSource)element.GetValue(IconSourceProperty);
        }

        public static void SetCloseCommand(DependencyObject element, ICommand value)
        {
            element.SetValue(CloseCommandProperty, value);
        }

        public static ICommand GetCloseCommand(DependencyObject element)
        {
            return (ICommand)element.GetValue(CloseCommandProperty);
        }

        public static void SetCloseEnabled(DependencyObject element, bool value)
        {
            element.SetValue(CloseEnabledProperty, value);
        }

        public static bool GetCloseEnabled(DependencyObject element)
        {
            return (bool)element.GetValue(CloseEnabledProperty);
        }
    }
}
