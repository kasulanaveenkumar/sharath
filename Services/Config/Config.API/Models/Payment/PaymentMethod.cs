using Common.Validations.Helper;
using System.ComponentModel.DataAnnotations;

namespace Config.API.Models.Payment
{
    public class PaymentMethod
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "CardNumber is required")]
        [CommonStringValidator]
        public string CardNumber { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "CardHolderName is required")]
        [CommonStringValidator]
        public string CardHolderName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "ExpiryDate is required")]
        [CommonMonthYearValidator]
        public string ExpiryDate { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "CVC is required")]
        [CommonStringValidator]
        public string CVC { get; set; }

        public bool IsPrimary { get; set; }

        public string PaymentMethodId { get; set; }

        public bool IsDelete { get; set; }
    }
}
