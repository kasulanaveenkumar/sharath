using Core.API.Entities.SP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.API.Models
{
    public class LenderInspectionsResponse
    {
        public List<Usp_GetInspections> InspectionsList { get; set; }
        //public List<LenderInspectionsList> InspectionsList { get; set; }

        public Int32 TotalRecords { get; set; }
    }

    public class LenderInspectionsList
    {
        public string BuyerName { get; set; }

        public string SellerName { get; set; }

        public string AssetType { get; set; }

        public string BrokerName { get; set; }

        public string CompanyName { get; set; }

        public long InspectionId { get; set; }

        public string Status { get; set; }

        [JsonIgnore]
        public string TemplateSetGuid { get; set; }

        [JsonIgnore]
        public string BrokerCompanyGuid { get; set; }

        public Int32 ApplicationStatus { get; set; }

        public DateTime? UpdatedTime { get; set; }

        [JsonIgnore]
        public long CreatedBy { get; set; }

        public bool IsSuspended { get; set; }

        public bool IsBypassRequested { get; set; }
    }
}
