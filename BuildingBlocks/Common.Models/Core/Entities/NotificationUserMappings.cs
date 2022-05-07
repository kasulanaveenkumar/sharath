using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Models.Core.Entities
{
    public class NotificationUserMappings
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }

        public Int64 AppActivityId { get; set; }

        [StringLength(50)]
        public string UserGuid { get; set; }

        [StringLength(50)]
        public string CompanyGuid { get; set; }
    }
}
