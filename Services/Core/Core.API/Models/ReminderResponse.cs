using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.API.Models
{
    public class ReminderResponse
    {
        public AppUser Buyer { get; set; }

        public AppUser Seller { get; set; }

        public string LenderName { get; set; }

        public string CompanyName { get; set; }

        public string BrokerName { get; set; }

        public string BrokerEmail { get; set; }

        public string AssetType { get; set; }

        public Int64 InspectionId { get; set; }

        public string InspectionStatus { get; set; }

        public List<string> PendingDocuments { get; set; }

        public List<string> RejectedDocuments { get; set; }

        public DateTime CreatedTime { get; set; }

        public string WebAppShortLink { get; set; }
    }
}
