using Common.Validations.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Config.API.Models
{
    public class LenderCompanyRequest
    {
        public string LenderCompanyGuid { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "LenderName is required")]
        [MaxLength(100, ErrorMessage = "LenderName should not exceed more than 100 characters")]
        [CommonStringValidator]
        public string LenderName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "LenderCompanyName is required")]
        [MaxLength(100, ErrorMessage = "LenderCompanyName should not exceed more than 100 characters")]
        [CommonStringValidator]
        public string LenderCompanyName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "ABN is required")]
        [MaxLength(50, ErrorMessage = "ABN should not exceed more than 50 characters")]
        [CommonStringValidator]
        public string ABN { get; set; }

        [MaxLength(255, ErrorMessage = "Website should not exceed more than 20 characters")]
        [CommonStringValidator]
        public string Website { get; set; }

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
        public string ZipCode { get; set; }

        [MaxLength(100, ErrorMessage = "Email should not exceed more than 100 characters")]
        [CommonStringValidator]
        [CommonEmailValidator]
        public string Email { get; set; }

        // Legal Details
        [MaxLength(100, ErrorMessage = "ContractLocation should not exceed more than 100 characters")]
        public string ContractLocation { get; set; }

        [CommonStringValidator]
        public string SignDate { get; set; }

        [CommonStringValidator]
        public string GoLiveDate { get; set; }

        // Visibility and Lodge
        public Int16 LenderVisibility { get; set; }

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

    public class CompanyContacts
    {
        [MaxLength(100, ErrorMessage = "Name should not exceed more than 100 characters")]
        [CommonStringValidator]
        public string Name { get; set; }

        [MaxLength(100, ErrorMessage = "SurName should not exceed more than 100 characters")]
        [CommonStringValidator]
        public string SurName { get; set; }

        [MaxLength(255, ErrorMessage = "Email should not exceed more than 255 characters")]
        [CommonStringValidator]
        [CommonEmailValidator]
        public string Email { get; set; }

        [MaxLength(20, ErrorMessage = "MobileNumber should not exceed more than 20 digits")]
        [CommonMobileNumberValidator]
        public string Mobile { get; set; }

        public Int16 CompanyContactTypeId { get; set; }
    }

    public class LenderConfigurations
    {
        public string LenderCompanyGuid { get; set; }

        [MaxLength(500, ErrorMessage = "LenderRefPrefix should not exceed more than 500 characters")]
        [CommonStringValidator]
        public string LenderRefPrefix { get; set; }

        [CommonStringValidator]
        public string AdditionalTnC { get; set; }

        public bool IsReportRequired { get; set; }

        [CommonStringValidator]
        [CommonEmailValidator]
        public string ReportEmailAddress { get; set; }

        public bool IsIllionIntegrationEnabled { get; set; }

        public bool IsAPIIntegrationEnabled { get; set; }

        public bool IsBSAllowed { get; set; }

        public bool IsNonOwnerAllowed { get; set; }

        public bool IsAllowAwaitedRef { get; set; }
    }
}
