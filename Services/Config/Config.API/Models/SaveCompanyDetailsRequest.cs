using Common.Validations.Helper;
using Config.API.Models.Broker;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Config.API.Models
{
    public class SaveCompanyDetailsRequest
    {
        public bool IsNewCompany { get; set; }

        public string ExistingCompanyGuid { get; set; }

        public CompanyDetails CompanyDetails { get; set; }

        public List<string> LendersWorkWith { get; set; }

        public List<string> AssetsWorkWith { get; set; }

        public string CompanyLogo { get; set; }

        [MaxLength(500, ErrorMessage = "CompanyDescription should not exceed more than 500 characters")]
        [CommonStringValidator]
        public string CompanyDescription { get; set; }

        /// <summary>
        /// Default: False
        /// Registration mail should send for all portal new signup & invited users
        /// Mail should not send only for the imported users
        /// This will be set to true during import broker / user
        /// After some point, we can remove this
        /// </summary>
        public bool SkipRegistrationMail { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "BrokerUsers List required")]
        public List<BrokerUsers> BrokerUsers { get; set; }
    }

    public class CompanyDetails
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

        [Required(AllowEmptyStrings = false, ErrorMessage = "ZIP Code is required")]
        [MaxLength(20, ErrorMessage = "ZIPCode should not exceed more than 20 characters")]
        [CommonStringValidator]
        public string ZIPCode { get; set; }

        [MaxLength(255, ErrorMessage = "Website should not exceed more than 20 characters")]
        [CommonStringValidator]
        public string Website { get; set; }

        [MaxLength(100, ErrorMessage = "Email should not exceed more than 100 characters")]
        [CommonStringValidator]
        [CommonEmailValidator]
        public string Email { get; set; }

        public bool ExcemptPayment { get; set; }

        public bool AllowOnlyInvitedUser { get; set; }
    }

    public class SaveCompanyLogoRequest
    {
        public string CompanyLogo { get; set; }

        [MaxLength(500, ErrorMessage = "CompanyDescription should not exceed more than 500 characters")]
        [CommonStringValidator]
        public string CompanyDescription { get; set; }

        public string CompanyGuid { get; set; }
    }
}
