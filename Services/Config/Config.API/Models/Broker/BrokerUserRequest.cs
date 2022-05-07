using Common.Validations.Helper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Config.API.Models.Broker
{
    public class BrokerUserRequest
    {
        public string CompanyGuid { get; set; }

        public bool SkipRegistrationMail { get; set; }

        public List<BrokerUsers> BrokerUsers { get; set; }
    }

    public class BrokerUsers
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is required")]
        [MaxLength(100, ErrorMessage = "Email should not exceed more than 100 characters")]
        [CommonStringValidator]
        [CommonEmailValidator]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "FirstName is required")]
        [MaxLength(100, ErrorMessage = "FirstName should not exceed more than 100 characters")]
        [CommonStringValidator]
        public string FirstName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "LastName is required")]
        [MaxLength(100, ErrorMessage = "LastName should not exceed more than 100 characters")]
        [CommonStringValidator]
        public string LastName { get; set; }

        public string UserGuid { get; set; }

        public string Mobile { get; set; }

        public bool IsAdmin { get; set; }

        public bool IsBillingResponsible { get; set; }

        public bool IsPrimaryContact { get; set; }
    }
}
