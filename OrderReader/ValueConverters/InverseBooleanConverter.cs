using Ninject.Planning.Targets;
using System;
using System.Globalization;

namespace OrderReader
{
    /// <summary>
    /// Takes in a boolean and returns it's inverted value
    /// </summary>
    class InverseBooleanConverter : BaseValueConverter<InverseBooleanConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool newValue;

            if (targetType != typeof(bool))
                newValue = System.Convert.ToBoolean(value);
            else
                newValue = (bool)value;

            return !newValue;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
