using Common.Validations.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core.API.Models
{
    public class EditInspectionRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "InspectionId is required")]
        public Int64 InspectionId { get; set; }

        public AppUser Buyer { get; set; }

        //[Required(AllowEmptyStrings = false, ErrorMessage = "Seller Details is required")]
        public AppUser Seller { get; set; }

        public string LenderGuid { get; set; }

        [StringLength(50, ErrorMessage = "LenderRef should not exceed more than 50 characters")]
        [CommonStringValidator]
        public string LenderRef { get; set; }

        public string TemplateSetGuid { get; set; }

        public string TemplateSetPlanGuid { get; set; }

        [CommonStringValidator]
        public string StateCode { get; set; }

        public string PaymentMethodId { get; set; }

        public List<TemplateDocument> Documents { get; set; }

        public List<string> UsersToShare { get; set; }
    }
}

