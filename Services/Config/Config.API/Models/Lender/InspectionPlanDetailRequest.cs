using Common.Validations.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Config.API.Models.Lender
{
    public class InspectionPlanDetailRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "TemplateSetGuid is required")]
        public string TemplateSetGuid { get; set; }

        public bool IsApplyToAllStates { get; set; }

        public bool IsUseDbValue { get; set; }

        public List<TemplateSetStatePlanDetails> StatePlanDetails { get; set; }

        public List<StatePlansRequest> StatePlans { get; set; }
    }

    public class TemplateSetStatePlanDetails
    {
        public Int64 StateId { get; set; }

        public List<TemplateSetPlanDetails> PlanDetails { get; set; }
    }

    public class TemplateSetPlanDetails
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "TemplatePlanGuid is required")]
        public string TemplatePlanGuid { get; set; }

        public decimal BasePrice { get; set; }

        public bool IsActivated { get; set; }
    }

    public class StatePlansRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "StateCode is required")]
        [CommonStringValidator]
        public string StateCode { get; set; }

        public Int64 StateId { get; set; }

        public List<PlanDocumentRequest> PlanDocuments { get; set; }
    }

    public class PlanDocumentRequest
    {
        public Int64 DocumentId { get; set; }

        // Additional Data (Selected or Unselected)
        public bool IsAdditionalDataSelected { get; set; }

        /// <summary>
        /// 0 - Not Required || 1 - Required and Not Mandatory || 2 - Required and Mandatory
        /// </summary>
        public Int16 HasAdditionalData { get; set; }

        public List<PlanDocumentDetailsRequest> DocPlanDetails { get; set; }

        public List<PlanImageRequest> ImageDetails { get; set; }
    }

    public class PlanImageRequest
    {
        public Int64 ImageId { get; set; }

        // Make Mandatory (Selected or Unselected)
        public bool IsMandatorySelected { get; set; }

        public bool IsShowMandatory { get; set; }

        public List<PlanDocumentDetailsRequest> ImagePlanDetails { get; set; }
    }

    public class PlanDocumentDetailsRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "TemplatePlanGuid is required")]
        public string TemplatePlanGuid { get; set; }

        public bool IsDocumentSelected { get; set; }
    }

    public class PlanInfo
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "PlanGuid is required")]
        public string PlanGuid { get; set; }

        public Int64 StateId { get; set; }

        public Int64 PlanId { get; set; }
    }
}
