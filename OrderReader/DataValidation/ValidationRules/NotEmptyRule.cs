using System.Globalization;
using System.Windows.Controls;

namespace OrderReader
{
    public class NotEmptyRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string charString = value as string;
            if (charString.Length <= 0)
                return new ValidationResult(false, $"Value cannot be empty");

            return new ValidationResult(true, null);
        }
    }
}
