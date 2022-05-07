using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Common.Validations.Helper
{
    public class CommonMonthYearValidator : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || value.ToString() == "")
                return new ValidationResult("Enter valid date");

            string pattern = @"^(0?[1-9]|1[0-2])/(19|2[0-1])?\d{2}$";

            if (Regex.IsMatch(value.ToString(), pattern))
                return ValidationResult.Success;
            else
                return new ValidationResult("Enter valid date");
        }
    }
}
