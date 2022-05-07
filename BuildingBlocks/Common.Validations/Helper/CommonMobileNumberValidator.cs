using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Common.Validations.Helper
{
    public class CommonMobileNumberValidator : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || value.ToString() == "")
                return ValidationResult.Success;

            string pattern = @"^\+[0-9]{10,12}";

            if (Regex.IsMatch(value.ToString().Replace(" ", ""), pattern, RegexOptions.IgnoreCase))
                return ValidationResult.Success;
            else
                return new ValidationResult("Enter valid Mobile Number");
        }
    }
}
