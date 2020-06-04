using System.Globalization;
using System.Windows.Controls;

namespace OrderReader
{
    public class MaximumValueRule : ValidationRule
    {
        public decimal MaximumValue { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string valueString = value as string;

            // First check if the value can be parsed as a decimal
            if (decimal.TryParse(valueString, NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal result))
            {
                // Then check if value is not exceeeding the maximum value
                if (result > MaximumValue)
                    return new ValidationResult(false, $"Value cannot exceed {MaximumValue}");
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
