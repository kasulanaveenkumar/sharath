using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.API.Entities
{
    public class Applications
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }

        [Required]
        [StringLength(50)]
        public string TemplateSetGuid { get; set; }

        [StringLength(50)]
        public string TemplateSetPlanGuid { get; set; }

        [Required]
        [StringLength(50)]
        public string RefNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string ExternalRefNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string LenderCompanyGuid { get; set; }

        [Required]
        [StringLength(50)]
        public string BrokerCompanyGuid { get; set; }

        public Int16 ApplicationStatus { get; set; }

        public Int16 PurgeStatus { get; set; }

        public bool IsSuspended { get; set; }

        public int ProcessedBy { get; set; }

        public Int64 CreatedBy { get; set; }

        public DateTime CreatedTime { get; set; }

        public Int64 UpdatedBy { get; set; }

        public DateTime? UpdatedTime { get; set; }

        public string StateCode { get; set; }

        public bool IsBypassRequested { get; set; }

        public string WebAppShortLink { get; set; }

        [StringLength(50)]
        public string InspectionGuid { get; set; }

        public Int32 RejectionCount { get; set; }

        [StringLength(500)]
        public string BankStatementUrl { get; set; }

        public DateTime? LastNotifiedTime { get; set; }

        public int? DVSStatus { get; set; }

        public string DeviceDetails { get; set; }
    }
}
