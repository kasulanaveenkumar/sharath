using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Common.Payments.Models
{
    public class CardDetails
    {
        public string CardHolderName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "CardNumber is required")]
        //[CommonStringValidator]
        public string CardNumber { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "ExpMonth is required")]
        public Int64 ExpMonth { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "ExpYear is required")]
        public Int64 ExpYear { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Cvc is required")]
        //[CommonStringValidator]
        public string Cvc { get; set; }

        //[CommonStringValidator]
        public string Brand { get; set; }

        public bool IsPrimary { get; set; }

        public string PaymentMethodId { get; set; }

        public string CustomerId { get; set; }

        public bool IsDelete { get; set; }

        //[CommonMonthYearValidator]
        public string ExpDate { get; set; }

        [JsonIgnore]
        public string ErrorMessage { get; set; }

        public bool IsCardSelected { get; set; }
    }
}
