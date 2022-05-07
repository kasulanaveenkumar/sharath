using Common.Validations.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Core.API.Models
{
    public class NewInspectionRequest
    {
        public AppUser Buyer { get; set; }

        [Required(ErrorMessage = "Seller Details is required")]
        public AppUser Seller { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "LenderGuid is required")]
        public string LenderGuid { get; set; }

        [StringLength(50, ErrorMessage = "LenderRef should not exceed more than 50 characters")]
        [CommonStringValidator]
        public string LenderRef { get; set; }

        [StringLength(50, ErrorMessage = "ExternalRef should not exceed more than 50 characters")]
        [CommonStringValidator]
        public string ExternalRef { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "TemplateSetGuid is required")]
        public string TemplateSetGuid { get; set; }

        public string TemplateSetPlanGuid { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "StateCode is required")]
        [CommonStringValidator]
        public string StateCode { get; set; }

        public string PaymentMethodId { get; set; }

        [Required(ErrorMessage = "Documents is required")]
        public List<TemplateDocument> Documents { get; set; }

        public List<string> UsersToShare { get; set; }

        public bool IsIncludeNoLenderPreference { get; set; }

        public bool IsPreferenceSaved { get; set; }
    }
    public class AppUser
    {
        [MaxLength(50, ErrorMessage = "Name should not exceed more than 50 characters")]
        [CommonStringValidator]
        public string Name { get; set; }

        [MaxLength(50, ErrorMessage = "SurName should not exceed more than 50 characters")]
        [CommonStringValidator]
        public string SurName { get; set; }

        [MaxLength(256, ErrorMessage = "Email should not exceed more than 256 characters")]
        [CommonStringValidator]
        [CommonEmailValidator]
        public string Email { get; set; }

        [MaxLength(20, ErrorMessage = "PhoneNumber should not exceed more than 20 digits")]
        [CommonMobileNumberValidator]
        public string PhoneNumber { get; set; }
    }
    public class TemplateDocument
    {
        public Int64 DocumentId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "DocumentName is required")]
        [StringLength(50, ErrorMessage = "DocumentName should not exceed more than 50 characters", MinimumLength = 1)]
        [CommonStringValidator]
        public string DocumentName { get; set; }

        public bool IsAdditionalDataMandatory { get; set; }

        public bool IsAdditionalDataRequired { get; set; }

        [Required(ErrorMessage = "Image Details is required")]
        public List<TemplateImage> ImageDetails { get; set; }

        public Int16 Position { get; set; }

        public string DocumentRequired { get; set; }
    }
    public class TemplateImage
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "ImageName is required")]
        [StringLength(50, ErrorMessage = "ImageName should not exceed more than 100 characters")]
        [CommonStringValidator]
        public string ImageName { get; set; }

        public Int16 DocGroup { get; set; }

        public Int16 ImageType { get; set; }

        public Int16 Position { get; set; }

        public bool IsMandatory { get; set; }

        public bool IsDefaultSelected { get; set; }
    }
    public class SaveNoLenderPreferenceRequest
    {
        public string TemplateSetGuid { get; set; }

        public List<NoLenderPreference> NoLenderPreferences { get; set; }

        public bool IsPreferenceSaved { get; set; }
    }
    public class NoLenderPreference
    {
        public Int64 DocumentId { get; set; }

        public List<int> ImageTypes { get; set; }
    }
}

