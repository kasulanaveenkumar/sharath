using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.API.Entities.SP
{
    public class Usp_GetInspections
    {
        public string BuyerName { get; set; }

        public string BuyerEmail { get; set; }

        public string SellerName { get; set; }

        public string SellerEmail { get; set; }

        public string AssetType { get; set; }

        public string Lender { get; set; }

        public string BrokerCompany { get; set; }

        public string LenderRef { get; set; }

        public long InspectionId { get; set; }

        public string Status { get; set; }

        public Int32 ApplicationStatus { get; set; }

        public DateTime? UpdatedTime { get; set; }

        public bool IsSuspended { get; set; }

        public bool IsBypassRequested { get; set; }

        public string BypassStatus { get; set; }

        [JsonIgnore]
        public Int64 TotalRecords { get; set; }

        public Int32 ReverseTimer { get; set; }

        public bool IsPurged { get; set; }

        public string InspectionGuid { get; set; }

        public bool IsShowFlag { get; set; }
    }
}
