using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Config.API.Entities.AdditionalData
{
    public class BrokerUsers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }

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
