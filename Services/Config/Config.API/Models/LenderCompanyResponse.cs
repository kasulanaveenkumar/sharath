using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Config.API.Models
{
    public class LenderCompanyResponse
    {
        public long LenderId { get; set; }
        public string LenderName { get; set; }
        public string LenderCompanyName { get; set; }
        public string LenderCompanyGuid { get; set; }
        public string ABN { get; set; }
        public string Website { get; set; }
        public string CompanyAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Email { get; set; }
        public bool IsPayer { get; set; }
        public string LiveStatus { get; set; }

        // Legal Details
        public string ContractLocation { get; set; }
        public DateTime? SignDate { get; set; }
        public DateTime? GoLiveDate { get; set; }

        // Visibility and Lodge
        public Int16 LenderVisibility { get; set; }

        // Contact Details
        public List<CompanyContacts> CompanyContacts { get; set; }

        // Report Customisation
        public string CompanyLogo { get; set; }
        public string CompanyDescription { get; set; }

        // Assets
        public List<Models.AssetsWorkWithResponse> Assets { get; set; }

        // Lender Configurations
        public LenderConfigurations LenderConfigurations { get; set; }
    }
}
