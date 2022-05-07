using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Config.API.Models.Lender
{
    public class InspectionPlanDetailReponse
    {
        public InspectionPlanDetailReponse()
        {
            StatePlanDetails = new List<TemplateSetStatePlanDetailsResponse>();

            StatePlans = new List<StatePlansResponse>();
        }

        public string TemplateSetGuid { get; set; }

        public bool IsApplyToAllStates { get; set; }

        public List<TemplateSetStatePlanDetailsResponse> StatePlanDetails { get; set; }

        public List<StatePlansResponse> StatePlans { get; set; }
    }

    public class TemplateSetStatePlanDetailsResponse
    {
        public TemplateSetStatePlanDetailsResponse()
        {
            PlanDetails = new List<TemplateSetPlanDetailsResponse>();
        }

        public Int64 StateId { get; set; }

        public string StateCode { get; set; }

        public List<TemplateSetPlanDetailsResponse> PlanDetails { get; set; }
    }

    public class TemplateSetPlanDetailsResponse
    {
        [JsonIgnore]
        public Int64 StateId { get; set; }

        [JsonIgnore]
        public Int64 TemplatePlanId { get; set; }

        public string TemplatePlanGuid { get; set; }

        public string TemplatePlanName { get; set; }

        public string TemplatePlanDescription { get; set; }

        public decimal BasePrice { get; set; }

        // Show Activate this plan / Deactivate this plan (in bottom)
        public bool IsActivated { get; set; }

        [JsonIgnore]
        public Int16 TemplatePlanLevel { get; set; }

        [JsonIgnore]
        public bool IsAppliedAllState { get; set; }
    }

    public class StatePlansResponse
    {
        public StatePlansResponse()
        {
            PlanDocuments = new List<PlanDocumentResponse>();
        }

        public Int64 StateId { get; set; }

        public string StateCode { get; set; }

        public List<PlanDocumentResponse> PlanDocuments { get; set; }
    }

    public class PlanDocumentResponse
    {
        public PlanDocumentResponse()
        {
            DocPlanDetails = new List<PlanDocumentDetailsResponse>();

            ImageDetails = new List<PlanImageResponse>();
        }

        // Document Name
        public string Name { get; set; }

        // Document Description
        public string Description { get; set; }

        // Warning Message
        public string WarningMessage { get; set; }

        public Int64 DocumentId { get; set; }

        // Additional Data (Show or Hide)
        public bool IsShowAdditionalData { get; set; }

        // Additional Data (Selected or Unselected)
        public bool IsAdditionalDataSelected { get; set; }

        public bool IsAdditionalDataReadOnly { get; set; }

        public Int16 HasAdditionalData { get; set; }

        public List<PlanDocumentDetailsResponse> DocPlanDetails { get; set; }

        public List<PlanImageResponse> ImageDetails { get; set; }
    }

    public class PlanImageResponse
    {
        public PlanImageResponse()
        {
            ImagePlanDetails = new List<PlanDocumentDetailsResponse>();
        }
        // Image Name
        public string Name { get; set; }

        // Image Description
        public string Description { get; set; }

        // Warning Message
        public string WarningMessage { get; set; }

        public Int64 ImageId { get; set; }

        // Make Mandatory (Show or Hide)
        public bool IsShowMandatory { get; set; }

        // Make Mandatory (Selected or Unselected)
        public bool IsMandatorySelected { get; set; }

        public bool IsMandatoryReadOnly { get; set; }

        public List<PlanDocumentDetailsResponse> ImagePlanDetails { get; set; }
    }

    public class PlanDocumentDetailsResponse
    {
        public string TemplatePlanGuid { get; set; }

        public bool IsShowDocument { get; set; }

        public bool IsDocumentSelected { get; set; }

        public bool IsDocumentReadOnly { get; set; }
    }
}
