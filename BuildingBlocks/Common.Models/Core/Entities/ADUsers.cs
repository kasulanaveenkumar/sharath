using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Models.Core.Entities
{
    public class ADUsers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }

        public Int64 UserId { get; set; }

        [StringLength(50)]
        public string UserGuid { get; set; }

        public string CompanyGuid { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(100)]
        public string SurName { get; set; }

        public string Email { get; set; }

        public string Mobile { get; set; }

        public Int64 UserTypeId { get; set; }

        public bool IsActive { get; set; }

        public bool IsPrimary { get; set; }
    }
}
