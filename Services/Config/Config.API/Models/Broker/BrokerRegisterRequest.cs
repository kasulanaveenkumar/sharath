using Common.Validations.Helper;
using Config.API.Models.Payment;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Config.API.Models.Broker
{
    public class BrokerRegisterRequest
    {
        public bool IsNewCompany { get; set; }

        public string ExistingCompanyGuid { get; set; }

        public NewCompanyDetail NewCompanyDetail { get; set; }

        public List<PaymentMethod> PaymentMethods { get; set; }

        public List<string> LendersWorkWith { get; set; }

        public List<string> AssetsWorkWith { get; set; }
    }

    public class NewCompanyDetail
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "CompanyName is Required")]
        [MaxLength(100, ErrorMessage = "CompanyName should not exceed more than 100 characters")]
        [CommonStringValidator]
        public string CompanyName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "RegisteredCompanyName is Required")]
        [MaxLength(100, ErrorMessage = "RegisteredCompanyName should not exceed more than 100 characters")]
        [CommonStringValidator]
        public string RegisteredName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "ABN is Required")]
        [MaxLength(50, ErrorMessage = "ABN should not exceed more than 50 characters")]
        [CommonStringValidator]
        public string ABN { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "CompanyAddress is Required")]
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

        [Required(AllowEmptyStrings = false, ErrorMessage = "ZIPCode is Required")]
        [MaxLength(20, ErrorMessage = "ZIPCode should not exceed more than 20 characters")]
        [CommonStringValidator]
        public string ZIPCode { get; set; }

        [MaxLength(255, ErrorMessage = "Website should not exceed more than 255 characters")]
        [CommonStringValidator]
        public string Website { get; set; }

        [MaxLength(100, ErrorMessage = "Email should not exceed more than 100 characters")]
        [CommonStringValidator]
        [CommonEmailValidator]
        public string Email { get; set; }
    }
}
