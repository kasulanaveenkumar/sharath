using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Config.API.Models
{
    public class AllCompaniesResponse
    {
        public string CompanyGuid { get; set; }

        public string CompanyName { get; set; }

        public Int64 CompanyTypeId { get; set; }

        public bool IsPayer { get; set; }

        public bool ExcemptPayment { get; set; }
    }
}
