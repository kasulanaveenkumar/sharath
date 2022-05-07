using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.API.Entities
{
    public class Notifications
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }

        public string NotificationGuid { get; set; }

        public string NotificationEventId { get; set; }

        public Int16 EnvetType { get; set; }

        public string EventDescription { get; set; }

        public string EventFor { get; set; }

        public bool IsActive { get; set; }
    }
}
