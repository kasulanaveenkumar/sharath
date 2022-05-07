using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Config.API.Models.Data
{
    public class SaveBrokerUsersRequest
    {
        public Int64 UserId { get; set; }

        public string UserGuid { get; set; }

        public string CompanyGuid { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Email { get; set; }

        public string Mobile { get; set; }

        public Int64 UserTypeId { get; set; }

        public bool IsActive { get; set; }
    }
}
