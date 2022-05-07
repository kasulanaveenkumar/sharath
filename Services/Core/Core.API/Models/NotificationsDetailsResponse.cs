using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.API.Models
{
    public class NotificationsDetailsResponse
    {
        public List<NotificationsList> NotificationsList { get; set; }

        public Int32 TotalRecords { get; set; }
    }

    public class NotificationsList
    {
        public Int64 InspectionId { get; set; }

        public string Buyer { get; set; }

        public string Seller { get; set; }

        public string AssetType { get; set; }

        public DateTime? ProcessedTime { get; set; }

        public string NotificationType { get; set; }

        public Int64 InspectionStatus { get; set; }
    }
}
