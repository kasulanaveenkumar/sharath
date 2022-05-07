using Common.Validations.Helper;
using System.ComponentModel.DataAnnotations;

namespace Config.API.Models
{
    public class LendersWorkWithResponse
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "LenderGUID is Required")]
        public string LenderGUID { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Lender Name is Required")]
        [MaxLength(100, ErrorMessage = "Lender Name should not exceed more than 100 characters")]
        [CommonStringValidator]
        public string LenderName { get; set; }

        public bool IsPayer { get; set; }

        public bool IsMapped { get; set; }

        public bool ExcemptPayment { get; set; }
    }
}
