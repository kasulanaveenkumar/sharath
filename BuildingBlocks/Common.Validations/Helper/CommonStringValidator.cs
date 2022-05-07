using System.ComponentModel.DataAnnotations;

namespace Common.Validations.Helper
{
    public class CommonStringValidator : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string notAllowedCharacters = @"$'*<>\^`{|}~";

            if (value == null || value.ToString() == "")
                return ValidationResult.Success;

            foreach (var item in notAllowedCharacters)
            {
                if (value.ToString().Contains(item))
                    return new ValidationResult(@"Special characters ($'*/<>\^`{|}~) are not accepted.");
            }

            return ValidationResult.Success;
        }
    }
}
