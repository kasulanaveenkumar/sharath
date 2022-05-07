using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Models.Core.Entities
{
    public class AppActivities
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }

        [StringLength(50)]
        public string Description { get; set; }

        public bool IsEnabledForNotifications { get; set; }

        public bool IsEmailRequired { get; set; }

        [StringLength(500)]
        public string NotificationDescription { get; set; }

        [StringLength(50)]
        public string NotificationGuid { get; set; }
    }
}
