using System;
using System.Globalization;
using System.Windows;

namespace OrderReader
{
    /// <summary>
    /// A converter that takes in a string and returns a <see cref="Visibility"/> based on it's value
    /// </summary>
    public class StringNullOrEmptyToVisibilityConverter : BaseValueConverter<StringNullOrEmptyToVisibilityConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null)
                return string.IsNullOrEmpty(value as string) ? Visibility.Hidden : Visibility.Visible;
            else if (parameter.GetType() == typeof(string) && (string)parameter == "Collapse")
                return string.IsNullOrEmpty(value as string) ? Visibility.Collapsed : Visibility.Visible;
            else if (parameter.GetType() == typeof(string) && (string)parameter == "CollapseReversed")
                return string.IsNullOrEmpty(value as string) ? Visibility.Visible : Visibility.Collapsed;
            else
                return string.IsNullOrEmpty(value as string) ? Visibility.Visible : Visibility.Hidden;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
