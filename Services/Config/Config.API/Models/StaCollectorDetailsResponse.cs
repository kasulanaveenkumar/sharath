using System;

namespace Config.API.Models
{
    public class StaCollectorDetailsResponse
    {
        public string CompanyName { get; set; }

        public string UserFirstName { get; set; }

        public string UserLastName { get; set; }

        public string UserEmail { get; set; }

        public string UserMobileNumber { get; set; }

        public Int32 CollectorAggregatorRole { get; set; }
    }
}
