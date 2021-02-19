using System.Globalization;
using System.Windows.Controls;

namespace OrderReader
{
    public class MinimumIntValueRule : ValidationRule
    {
        public decimal MinimumIntValue { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string valueString = value as string;

            // First check if the value can be parsed as an integer
            if (int.TryParse(valueString, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result))
            {
                // Then check if value is not too low
                if (result < MinimumIntValue)
                    return new ValidationResult(false, $"Value cannot be less than {MinimumIntValue}");
            }
            else
            {
                // If not, tell the sure that it is not valid
                return new ValidationResult(false, $"Invalid value entered");
            }

            return new ValidationResult(true, null);
        }
    }
}
