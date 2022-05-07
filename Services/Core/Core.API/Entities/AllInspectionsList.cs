using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.API.Entities
{
    public class AllInspectionsList
    {
        public string BuyerName { get; set; }

        public string SellerName { get; set; }

        public string AssetType { get; set; }

        public string Lender { get; set; }

        public string LenderRef { get; set; }

        public long InspectionId { get; set; }

        public string Status { get; set; }

        public Int16 ApplicationStatus { get; set; }

        public DateTime? UpdatedTime { get; set; }

        public bool IsSuspended { get; set; }

        public bool IsBypassRequested { get; set; }

        [JsonIgnore]
        public int TotalRecordCount { get; set; }
    }
}
