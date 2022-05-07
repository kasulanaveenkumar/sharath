using System;
using System.Collections.Generic;

namespace Common.Notifications.Models
{
    public class InspectionNotification
    {
        public Int64 InspectionId { get; set; }

        public string BrokerName { get; set; }

        public string SellerName { get; set; }

        public Int16 ApplicationStatusId { get; set; }  // Try to replace this with enum

        public List<RejectedDocuments> RejectedDocuments { get; set; }

        public string BrokerEmail { get; set; }

        public string WebhookTargetURL { get; set; }
    }

    public class RejectedDocuments
    {
        public string DocName { get; set; }

        public string SellerAction { get; set; }
    }
}
