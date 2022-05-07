using System.ComponentModel.DataAnnotations;
using Common.Validations.Helper;

namespace Common.Models.Users
{
    public class AuthenticateRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is required")]
        [CommonStringValidator]
        [CommonEmailValidator]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
