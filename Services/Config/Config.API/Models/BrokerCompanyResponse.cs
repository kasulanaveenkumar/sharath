using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Config.API.Models
{
    public class BrokerCompanyResponse
    {
        public Int64 CompanyId { get; set; }

        public string CompanyGuid { get; set; }

        public string CompanyName { get; set; }

        public string ABN { get; set; }

        public string Mobile { get; set; }

        public string State { get; set; }

        public int NumberofBrokers { get; set; }

        public int NumberofActiveBrokers { get; set; }

        public bool IsVisible { get; set; }
    }
}
