using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.API.Models
{
    public class ReviewDataResponse
    {
        public ReviewDataResponse()
        {
            Documents = new List<AppDocuments>();
        }

        public Models.InspectionDetails InspectionDetails { get; set; }

        public List<AppDocuments> Documents { get; set; }

        public bool IsReportRequired { get; set; }

        public int? DVSStatus { get; set; }

        public bool IsPPSRSearchBySerialNumberEnabled { get; set; }

        /// 0 - Do not show | 1 - Show without any status | 2 - Show with verified status
        public int PPSRForVehicleStatus { get; set; }

        /// 0 - Do not show | 1 - Show without any status | 2 - Show with verified status
        public int PPSRForBoatStatus { get; set; }

        /// 0 - Do not show | 1 - Show without any status | 2 - Show with verified status
        public int PPSRForTrailerStatus { get; set; }
    }

    public class InspectionRejectFlagReasonsResponse
    {
        public Int64 ReasonId { get; set; }

        public Int16 ImageType { get; set; }

        [JsonIgnore]
        public Int16 ReasonType { get; set; }

        public string Description { get; set; }
    }
}
