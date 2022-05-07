using System;

namespace Core.API.Entities.SP
{
    public class Usp_B2B_GetInspections
    {
        public Int64 Id { get; set; }

        public Int16 ApplicationStatusId { get; set; }

        public string ApplicationStatus { get; set; }

        public string ReferenceNumber { get; set; }

        public string ExternalRef { get; set; }

        public DateTime CreationDate { get; set; }

        public Int32 TotalDocuments { get; set; }

        public Int32 DocumentsPending { get; set; }

        public Int32 DocumentsUploaded { get; set; }

        public Int32 DocumentsAccepted { get; set; }

        public Int32 DocumentsRejected { get; set; }

        public Int32 DocumentsProcessed { get; set; }

        public Int32 UploadPercentage { get; set; }

        public Int32 CompletionPercentage { get; set; }

        public bool IsSellerActionRequired { get; set; }

        public string BuyerName { get; set; }

        public string BuyerSurName { get; set; }

        public string BuyerEmail { get; set; }

        public string BuyerMobile { get; set; }

        public string SellerName { get; set; }

        public string SellerSurName { get; set; }

        public string SellerEmail { get; set; }

        public string SellerMobile { get; set; }

        public string BrokerName { get; set; }

        public string BrokerSurName { get; set; }

        public string BrokerEmail { get; set; }

        public string BrokerMobile { get; set; }

        public string CompanyName { get; set; }

        public bool IsOwner { get; set; }
    }
}