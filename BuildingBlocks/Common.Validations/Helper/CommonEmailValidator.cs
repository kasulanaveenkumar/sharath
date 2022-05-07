using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Common.Validations.Helper
{
    public class CommonEmailValidator : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || value.ToString() == "")
                return ValidationResult.Success;

            string pattern = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";

            if (Regex.IsMatch(value.ToString(), pattern, RegexOptions.IgnoreCase))
                return ValidationResult.Success;
            else
                return new ValidationResult("Enter valid Email");
        }
    }
}
