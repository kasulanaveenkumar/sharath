using Common.Validations.Helper;
using System.ComponentModel.DataAnnotations;

namespace Core.API.Models
{
    public class UpdateIllionDetailsRequest
    {
        public bool IsActive { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "ReferralCode is required")]
        [CommonStringValidator]
        public string ReferralCode { get; set; }

        public bool IsStatementRequired { get; set; }
    }
}
