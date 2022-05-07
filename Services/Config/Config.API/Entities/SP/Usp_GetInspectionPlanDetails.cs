using System;

namespace Config.API.Entities.SP
{
    public class Usp_GetInspectionPlanDetails
    {
        public Int64 Id { get; set; }   

        public string TemplateSetGuid { get; set; }
        
        public string TemplateName { get; set; }

        public string PlanGuid { get; set; }

        public string PlanName { get; set; }

        public string PlanDescription { get; set; }

        public decimal Price { get; set; }

        public Int16 PlanLevel { get; set; }

        public bool? IsActive { get; set; }

        public bool? IsAppliedAllState { get; set; }

        public Int64 StateId {  get; set; }

        public string StateCode { get; set; }

        public Int64 DocId { get; set; }

        public string DocName { get; set; }

        public string DocDescription { get; set; }

        public string DocWarningMessage { get; set; }

        public Int16 DocPosition { get; set; }

        public bool? IsShowAdditionalData { get; set; }

        public bool? IsAdditionalDataSelected { get; set; }

        public bool? IsShowDocument { get; set; }

        public bool? IsDocumentSelected { get; set; }

        public bool? isDocumentReadOnly { get; set; }    

        public Int64 ImageId { get; set; }  

        public string ImageName { get; set; }   

        public string ImageDescription { get; set; }

        public string WarningMessage { get; set; }

        public Int16 ImagePosition { get; set; }

        public bool? IsShowMandatory { get; set; }

        public bool? IsMandatorySelected { get; set; }
         
    }
}
