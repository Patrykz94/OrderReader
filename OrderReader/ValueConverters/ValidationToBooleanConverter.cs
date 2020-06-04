using System;
using System.Globalization;

namespace OrderReader
{
    /// <summary>
    /// A converter that takes in a multiple booleans
    /// If all booleans were false, a true is returned
    /// If any of the booleans were true, a false is returned
    /// </summary>
    public class ValidationToBooleanConverter : BaseMultiValueConverter<ValidationToBooleanConverter>
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length > 0)
            {
                // Iterate over each of the values
                foreach (var value in values)
                {
                    // If a value is true, that means there's at least 1 error
                    if (value is bool && (bool)value)
                    {
                        // in that case validation returns false
                        return false;
                    }
                }
            }

            // If no errors found, return true
            return true;
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
