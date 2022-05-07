using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.API.Models
{
    public class LenderConfigurationResponse
    {
        public string LenderRefPrefix { get; set; }

        public string AdditionalTnC { get; set; }

        public bool IsReportRequired { get; set; }

        public string ReportEmailAddress { get; set; }

        public bool IsIllionIntegrationEnabled { get; set; }

        public bool IsAPIIntegrationEnabled { get; set; }

        public bool IsBSAllowed { get; set; }

        public bool IsNonOwnerAllowed { get; set; }

        public bool IsAllowAwaitedRef { get; set; }
    }
}
