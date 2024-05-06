using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace OrderReader.ValueConverters;

[ValueConversion(typeof(int), typeof(Visibility))]
public class IntToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null) return Visibility.Visible;

        var paramIsNumber = int.TryParse((string?)parameter, out var result);

        if (paramIsNumber)
            return (int)value >= result ? Visibility.Visible : Visibility.Collapsed;

        return parameter switch
        {
            null => (int)value == 0 ? Visibility.Collapsed : Visibility.Visible,
            "Reversed" => (int)value == 0 ? Visibility.Visible : Visibility.Collapsed,
            _ => (int)value == 0 ? Visibility.Collapsed : Visibility.Visible
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null) return Visibility.Visible;

        return parameter switch
        {
            null => (Visibility)value == Visibility.Visible ? 1 : 0,
            "Reversed" => (Visibility)value == Visibility.Visible ? 0 : 1,
            _ => (Visibility)value == Visibility.Visible ? 1 : 0
        };
    }
}