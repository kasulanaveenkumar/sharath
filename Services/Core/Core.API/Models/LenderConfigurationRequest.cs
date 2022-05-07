using Common.Validations.Helper;
using System.ComponentModel.DataAnnotations;

namespace Core.API.Models
{
    public class LenderConfigurationRequest
    {
        public string LenderCompanyGuid { get; set; }

        [MaxLength(500, ErrorMessage = "LenderRefPrefix should not exceed more than 500 characters")]
        [CommonStringValidator]
        public string LenderRefPrefix { get; set; }

        [CommonStringValidator]
        public string AdditionalTnC { get; set; }

        public bool IsReportRequired { get; set; }

        [CommonStringValidator]
        [CommonEmailValidator]
        public string ReportEmailAddress { get; set; }

        public bool IsIllionIntegrationEnabled { get; set; }

        public bool IsAPIIntegrationEnabled { get; set; }

        public bool IsBSAllowed { get; set; }

        public bool IsNonOwnerAllowed { get; set; }

        public bool IsAllowAwaitedRef { get; set; }
    }
}
