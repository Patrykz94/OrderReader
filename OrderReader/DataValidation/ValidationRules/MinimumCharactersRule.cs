using System.Globalization;
using System.Windows.Controls;

namespace OrderReader
{
    public class MinimumCharactersRule : ValidationRule
    {
        public int MinimumCharacters { get; set; } = 1;

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string charString = value as string;

            if (charString.Length < MinimumCharacters)
                return new ValidationResult(false, $"Minimum {MinimumCharacters} characters");

            return new ValidationResult(true, null);
        }
    }
}
