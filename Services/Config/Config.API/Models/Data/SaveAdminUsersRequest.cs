using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Config.API.Models.Data
{
    public class SaveAdminUsersRequest
    {
        public string CompanyGuid { get; set; }

        public string AdminName { get; set; }

        public string AdminEmail { get; set; }

        public bool IsDefaultAdmin { get; set; }

        public Int32 UsersCount { get; set; }
    }
}
