using Noesis;

namespace SaltyGame
{
    public static class GalapagOSMetricRow
    {
        public static readonly DependencyProperty IconSourceProperty =
            DependencyProperty.RegisterAttached(
                "IconSource",
                typeof(ImageSource),
                typeof(GalapagOSMetricRow));

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.RegisterAttached(
                "Label",
                typeof(string),
                typeof(GalapagOSMetricRow));

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.RegisterAttached(
                "Value",
                typeof(string),
                typeof(GalapagOSMetricRow));

        public static void SetIconSource(DependencyObject element, ImageSource value)
        {
            element.SetValue(IconSourceProperty, value);
        }

        public static ImageSource GetIconSource(DependencyObject element)
        {
            return (ImageSource)element.GetValue(IconSourceProperty);
        }

        public static void SetLabel(DependencyObject element, string value)
        {
            element.SetValue(LabelProperty, value);
        }

        public static string GetLabel(DependencyObject element)
        {
            return (string)element.GetValue(LabelProperty);
        }

        public static void SetValue(DependencyObject element, string value)
        {
            element.SetValue(ValueProperty, value);
        }

        public static string GetValue(DependencyObject element)
        {
            return (string)element.GetValue(ValueProperty);
        }
    }
}
