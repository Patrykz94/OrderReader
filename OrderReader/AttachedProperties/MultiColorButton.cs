using System.Windows;
using System.Windows.Media;

namespace OrderReader.AttachedProperties;

public static class MultiColorButton
{
    public static readonly DependencyProperty Column1BackgroundProperty = DependencyProperty.RegisterAttached(
        "Column1Background", typeof(Brush), typeof(MultiColorButton), new PropertyMetadata(Brushes.Transparent));
    public static readonly DependencyProperty Column2BackgroundProperty = DependencyProperty.RegisterAttached(
        "Column2Background", typeof(Brush), typeof(MultiColorButton), new PropertyMetadata(Brushes.Transparent));
    public static readonly DependencyProperty Column3BackgroundProperty = DependencyProperty.RegisterAttached(
        "Column3Background", typeof(Brush), typeof(MultiColorButton), new PropertyMetadata(Brushes.Transparent));

    // Column 1
    public static Brush GetColumn1Background(DependencyObject obj)
    {
        return (Brush)obj.GetValue(Column1BackgroundProperty);
    }

    public static void SetColumn1Background(DependencyObject obj, Brush value)
    {
        obj.SetValue(Column1BackgroundProperty, value);
    }

    // Column 2
    public static Brush GetColumn2Background(DependencyObject obj)
    {
        return (Brush)obj.GetValue(Column2BackgroundProperty);
    }

    public static void SetColumn2Background(DependencyObject obj, Brush value)
    {
        obj.SetValue(Column2BackgroundProperty, value);
    }

    // Column 3
    public static Brush GetColumn3Background(DependencyObject obj)
    {
        return (Brush)obj.GetValue(Column3BackgroundProperty);
    }

    public static void SetColumn3Background(DependencyObject obj, Brush value)
    {
        obj.SetValue(Column3BackgroundProperty, value);
    }
}