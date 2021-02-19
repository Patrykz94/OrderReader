using System.Globalization;
using System.Windows.Controls;

namespace OrderReader
{
    public class MaximumIntValueRule : ValidationRule
    {
        public decimal MaximumIntValue { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string valueString = value as string;

            // First check if the value can be parsed as an integer
            if (int.TryParse(valueString, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result))
            {
                // Then check if value is not exceeeding the maximum value
                if (result > MaximumIntValue)
                    return new ValidationResult(false, $"Value cannot exceed {MaximumIntValue}");
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
