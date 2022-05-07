using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.API.Entities
{
    public class IllionIntegrationDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }

        public string CompanyGuid { get; set; }

        [StringLength(50)]
        public string ReferralCode { get; set; }

        public bool IsStatementRequired { get; set; }

        public bool IsActive { get; set; }
    }
}
