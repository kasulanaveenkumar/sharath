using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.API.Models.Data
{
    public class UserDetailsResponse
    {
        public string Name { get; set; }

        public string SurName { get; set; }

        public string Email { get; set; }

        public string Mobile { get; set; }
    }
}
