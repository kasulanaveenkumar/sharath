using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.API.Entities
{
    public class B2BWebHooks
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }

        public string TargetUrl { get; set; }

        public Int16 EventId { get; set; }

        [MaxLength(50)]
        public string CompanyGuid { get; set; }
    }
}
