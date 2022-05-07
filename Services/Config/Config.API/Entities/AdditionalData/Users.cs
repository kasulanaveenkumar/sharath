using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Config.API.Entities.AdditionalData
{
    public class Users
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }

        public string CompanyGuid { get; set; }

        public string AdminName { get; set; }

        public string AdminEmail { get; set; }

        public bool IsDefaultAdmin { get; set; }

        public Int32 UsersCount { get; set; }
    }
}
