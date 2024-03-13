using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace OrderReaderUI.ValueConverters;

public class BoolToVisibilityConverter : IValueConverter
{
    public static readonly BoolToVisibilityConverter Instance = new();
    
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null) return Visibility.Visible;

        return parameter switch
        {
            null => (bool)value ? Visibility.Hidden : Visibility.Visible,
            string str1 when str1 == "Collapse" => (bool)value ? Visibility.Collapsed : Visibility.Visible,
            string str2 when str2 == "CollapseReversed" => (bool)value ? Visibility.Visible : Visibility.Collapsed,
            _ => (bool)value ? Visibility.Visible : Visibility.Hidden
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}