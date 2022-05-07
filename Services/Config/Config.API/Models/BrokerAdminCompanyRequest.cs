using Common.Validations.Helper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Config.API.Models
{
    public class BrokerAdminCompanyRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "CompanyName is required")]
        [MaxLength(100, ErrorMessage = "CompanyName should not exceed more than 100 characters")]
        [CommonStringValidator]
        public string CompanyName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "RegisteredCompanyName is required")]
        [MaxLength(100, ErrorMessage = "RegisteredCompanyName should not exceed more than 100 characters")]
        [CommonStringValidator]
        public string RegisteredCompanyName { get; set; }

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
        public string ZipCode { get; set; }

        [MaxLength(255, ErrorMessage = "Website should not exceed more than 20 characters")]
        [CommonStringValidator]
        public string Website { get; set; }

        [MaxLength(100, ErrorMessage = "Email should not exceed more than 100 characters")]
        [CommonStringValidator]
        [CommonEmailValidator]
        public string Email { get; set; }
        public string CompanyLogo { get; set; }

        [MaxLength(500, ErrorMessage = "CompanyDescription should not exceed more than 500 characters")]
        [CommonStringValidator]
        public string CompanyDescription { get; set; }

        public List<LendersWorkWithResponse> Lenders { get; set; }

        public List<AssetsWorkWithResponse> Assets { get; set; }
    }
}
