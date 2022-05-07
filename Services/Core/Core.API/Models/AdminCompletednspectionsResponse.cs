using Core.API.Entities.SP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.API.Models
{
    public class AdminCompletednspectionsResponse
    {
        public List<Usp_GetInspections> InspectionsList { get; set; }

        public Int64 TotalRecords { get; set; }
    }

    public class AdminCompletedInspectionsList
    {
        public string SellerName { get; set; }

        public string AssetType { get; set; }

        public string Lender { get; set; }

        public string LenderRef { get; set; }

        public Int64 InspectionId { get; set; }

        public string Status { get; set; }

        public string BypassStatus { get; set; }

        [JsonIgnore]
        public string TemplateSetGuid { get; set; }

        [JsonIgnore]
        public string LenderCompanyGuid { get; set; }

        public Int32 ApplicationStatus { get; set; }

        public DateTime? UpdatedTime { get; set; }

        public bool IsSuspended { get; set; }

        public bool IsBypassRequested { get; set; }
    }
}
