using Common.Validations.Helper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Config.API.Models.Lender
{
    public class LenderRegisterRequest
    {
        public NewCompanyDetail NewCompanyDetail { get; set; }

        public PrimaryContact PrimaryContact { get; set; }

        public List<string> AssetsWorkWith { get; set; }
    }

    public class NewCompanyDetail
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "CompanyName is required")]
        [MaxLength(100, ErrorMessage = "CompanyName should not exceed more than 100 characters")]
        [CommonStringValidator]
        public string CompanyName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "RegisteredCompanyName is required")]
        [MaxLength(100, ErrorMessage = "RegisteredCompanyName should not exceed more than 100 characters")]
        [CommonStringValidator]
        public string RegisteredName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "ABN is required")]
        [MaxLength(50, ErrorMessage = "ABN should not exceed more than 50 characters")]
        [CommonStringValidator]
        public string ABN { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "CompanyAddress is required")]
        [MaxLength(255, ErrorMessage = "CompanyAddress should not exceed more than 255 characters")]
        [CommonStringValidator]
        public string CompanyAddress { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "City is required")]
        [MaxLength(255, ErrorMessage = "City should not exceed more than 255 characters")]
        [CommonStringValidator]
        public string City { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "State is required")]
        [MaxLength(255, ErrorMessage = "State should not exceed more than 255 characters")]
        [CommonStringValidator]
        public string State { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "ZIPCode is required")]
        [MaxLength(20, ErrorMessage = "ZIPCode should not exceed more than 20 characters")]
        [CommonStringValidator]
        public string ZIPCode { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Website is required")]
        [MaxLength(255, ErrorMessage = "Website should not exceed more than 20 characters")]
        [CommonStringValidator]
        public string Website { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is required")]
        [MaxLength(100, ErrorMessage = "Email should not exceed more than 100 characters")]
        [CommonStringValidator]
        [CommonEmailValidator]
        public string Email { get; set; }
    }

    public class PrimaryContact
    {
        [MaxLength(100, ErrorMessage = "Name should not exceed more than 100 characters")]
        [CommonStringValidator]
        public string Name { get; set; }

        [MaxLength(100, ErrorMessage = "SurName should not exceed more than 100 characters")]
        [CommonStringValidator]
        public string SurName { get; set; }

        [MaxLength(100, ErrorMessage = "Email should not exceed more than 100 characters")]
        [CommonStringValidator]
        [CommonEmailValidator]
        public string Email { get; set; }

        [MaxLength(20, ErrorMessage = "MobileNumber should not exceed more than 20 digit")]
        [CommonMobileNumberValidator]
        public string Mobile { get; set; }
    }

    public class LenderConfiguration
    {
        public string LenderCompanyGuid { get; set; }

        [MaxLength(500, ErrorMessage = "LenderRefPrefix should not exceed more than 500 characters")]
        [CommonStringValidator]
        public string LenderRefPrefix { get; set; }

        public bool IsReportRequired { get; set; }

        [CommonStringValidator]
        [CommonEmailValidator]
        public string ReportEmailAddress { get; set; }

        public bool IsBSAllowed { get; set; }

        public bool IsNonOwnerAllowed { get; set; }

        public bool IsAllowAwaitedRef { get; set; }
    }
}
