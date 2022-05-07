using Common.Validations.Helper;
using System.ComponentModel.DataAnnotations;

namespace Config.API.Models.Broker
{
    public class OnboardDetailsRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is required")]
        [CommonStringValidator]
        [CommonEmailValidator]
        public string Email { get; set; }
    }
}
