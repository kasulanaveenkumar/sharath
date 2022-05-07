using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Models.Core.Entities
{
    public class AppActivityLogs
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }
    
        public Int64 AppActivityId { get; set; }

        public Int32 UserType { get; set; }

        [Required]
        [StringLength(50)]
        public string UserGuid { get; set; }

        public DateTime ProcessedTime { get; set; }

        public Int64 ApplicationId { get; set; }

        public bool IsNotified { get; set; }

        public bool IsWebAppUser { get; set; }

        public Int32 ProcessDuration { get; set; }
    }
}
