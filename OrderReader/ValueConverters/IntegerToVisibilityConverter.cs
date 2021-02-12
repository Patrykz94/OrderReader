using System;
using System.Globalization;
using System.Windows;

namespace OrderReader
{
    /// <summary>
    /// A converter that takes in a number and returns a <see cref="Visibility"/>
    /// </summary>
    public class IntegerToVisibilityConverter : BaseValueConverter<IntegerToVisibilityConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null)
                return (int)value > 0 ? Visibility.Hidden : Visibility.Visible;
            else if (parameter.GetType() == typeof(string) && (string)parameter == "Collapse")
                return (int)value > 0 ? Visibility.Collapsed : Visibility.Visible;
            else if (parameter.GetType() == typeof(string) && (string)parameter == "CollapseReversed")
                return (int)value > 0 ? Visibility.Visible : Visibility.Collapsed;
            else
                return (int)value > 0 ? Visibility.Visible : Visibility.Hidden;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
