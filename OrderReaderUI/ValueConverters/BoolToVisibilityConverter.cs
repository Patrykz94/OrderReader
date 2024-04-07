using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace OrderReaderUI.ValueConverters;

[ValueConversion(typeof(bool), typeof(Visibility))]
public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null) return Visibility.Visible;

        return parameter switch
        {
            null => (bool)value ? Visibility.Visible : Visibility.Collapsed,
            "Reversed" => (bool)value ? Visibility.Collapsed : Visibility.Visible,
            _ => (bool)value ? Visibility.Visible : Visibility.Collapsed,
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null) return false;
        
        return parameter switch
        {
            null => (Visibility)value == Visibility.Visible,
            "Reversed" => (Visibility)value == Visibility.Collapsed,
            _ => (Visibility)value == Visibility.Visible
        };
    }
}