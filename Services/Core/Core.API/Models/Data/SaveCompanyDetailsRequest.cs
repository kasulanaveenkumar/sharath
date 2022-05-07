using System;

namespace Core.API.Models.Data
{
    public class SaveCompanyDetailsRequest
    {
        public string CompanyGuid { get; set; }

        public string CompanyName { get; set; }

        public Int64 CompanyTypeId { get; set; }

        public bool IsPayer { get; set; }

        public bool ExcemptPayment { get; set; }
    }
}
