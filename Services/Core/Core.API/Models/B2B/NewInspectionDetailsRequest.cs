using System;

namespace Core.API.Models.B2B
{
    public class NewInspectionDetailsRequest
    {
        public string UserGuid { get; set; }

        public string CompanyGuid { get; set; }

        public string TemplateGuid { get; set; }

        public string planGuid { get; set; }

        public string LenderGuid { get; set; }

        public Int64 StateId { get; set; }

        public bool IsIncludeNoLenderPreference { get; set; }
    }
}
