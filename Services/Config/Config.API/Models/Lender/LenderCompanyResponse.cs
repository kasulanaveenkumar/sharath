using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Config.API.Models.Lender
{
    public class LenderCompanyResponse
    {
        // Company Details
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
