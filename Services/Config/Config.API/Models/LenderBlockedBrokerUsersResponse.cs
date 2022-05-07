using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Config.API.Models
{
    public class LenderBlockedBrokerUsersResponse
    {
        public string UserGuid { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Mobile { get; set; }

        public string Email { get; set; }

        public Int32 AverageInpsectionsPerMonth { get; set; }

        public bool IsAllowed { get; set; }
    }
}
