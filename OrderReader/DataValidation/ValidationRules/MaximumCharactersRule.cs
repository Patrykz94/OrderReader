using System.Globalization;
using System.Windows.Controls;

namespace OrderReader
{
    public class MaximumCharactersRule : ValidationRule
    {
        public int MaximumCharacters { get; set; } = 50;

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string charString = value as string;

            if (charString.Length > MaximumCharacters)
                return new ValidationResult(false, $"Maximum {MaximumCharacters} characters");

            return new ValidationResult(true, null);
        }
    }
}
