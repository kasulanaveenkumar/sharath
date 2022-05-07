using Common.Validations.Helper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Config.API.Models.Lender
{
    public class LenderCompanyRequest
    {
        [MaxLength(100, ErrorMessage = "LenderName should not exceed more than 100 characters")]
        public string LenderName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Lender Company Name is Required")]
        [MaxLength(100, ErrorMessage = "LenderCompanyName should not exceed more than 100 characters")]
        [CommonStringValidator]
        public string LenderCompanyName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "ABN is Required")]
        [MaxLength(50, ErrorMessage = "ABN should not exceed more than 50 characters")]
        [CommonStringValidator]
        public string ABN { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Company Address is Required")]
        [MaxLength(255, ErrorMessage = "CompanyAddress should not exceed more than 255 characters")]
        [CommonStringValidator]
        public string CompanyAddress { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "City is Required")]
        [MaxLength(255, ErrorMessage = "City should not exceed more than 255 characters")]
        [CommonStringValidator]
        public string City { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "State is Required")]
        [MaxLength(255, ErrorMessage = "State should not exceed more than 255 characters")]
        [CommonStringValidator]
        public string State { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "ZIP Code is Required")]
        [MaxLength(20, ErrorMessage = "ZIPCode should not exceed more than 20 characters")]
        [CommonStringValidator]
        public string ZipCode { get; set; }

        [MaxLength(255, ErrorMessage = "Website should not exceed more than 20 characters")]
        [CommonStringValidator]
        public string Website { get; set; }

        [MaxLength(100, ErrorMessage = "Email should not exceed more than 100 characters")]
        [CommonStringValidator]
        [CommonEmailValidator]
        public string Email { get; set; }

        // Contact Details
        public List<CompanyContacts> CompanyContacts { get; set; }

        // Report Customisation
        public string CompanyLogo { get; set; }

        [MaxLength(500, ErrorMessage = "CompanyDescription should not exceed more than 500 characters")]
        [CommonStringValidator]
        public string CompanyDescription { get; set; }

        // Assets
        public List<string> Assets { get; set; }

        // Lender Configurations
        public LenderConfigurations LenderConfigurations { get; set; }
    }

    public class LenderConfigurations
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "LenderCompanyGuid is required")]
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
