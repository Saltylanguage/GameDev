using Noesis;

namespace SaltyGame
{
    public static class GalapagOSButton
    {
        public static readonly DependencyProperty IconSourceProperty =
            DependencyProperty.RegisterAttached(
                "IconSource",
                typeof(ImageSource),
                typeof(GalapagOSButton));

        public static readonly DependencyProperty IconBeforeProperty =
            DependencyProperty.RegisterAttached(
                "IconBefore",
                typeof(bool),
                typeof(GalapagOSButton),
                new PropertyMetadata(false));

        public static void SetIconSource(DependencyObject element, ImageSource value)
        {
            element.SetValue(IconSourceProperty, value);
        }

        public static ImageSource GetIconSource(DependencyObject element)
        {
            return (ImageSource)element.GetValue(IconSourceProperty);
        }

        public static void SetIconBefore(DependencyObject element, bool value)
        {
            element.SetValue(IconBeforeProperty, value);
        }

        public static bool GetIconBefore(DependencyObject element)
        {
            return (bool)element.GetValue(IconBeforeProperty);
        }
    }
}
