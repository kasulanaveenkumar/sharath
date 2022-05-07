using System;

namespace Config.API.Models
{
    public class CompaniesListResponse
    {
        public long CompanyId { get; set; }

        public string CompanyName { get; set; }

        public string ABN { get; set; }

        public Int32 UsersCount { get; set; }

        public string PrimaryContactName { get; set; }

        public string PrimaryContactEmail { get; set; }

        public string LiveStatus { get; set; }

        public string CompanyGuid { get; set; }
    }
}
