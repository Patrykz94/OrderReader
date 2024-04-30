using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace OrderReaderUI.ValueConverters;

public class OpacityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(parameter);

        if (value is not Color color || parameter is not double opacity) return value;
            opacity = double.Max(double.Min(opacity, 1.0), 0.0);
        return Color.FromArgb((byte)(255 * opacity), color.R, color.G, color.B);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}