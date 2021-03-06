using System;
using System.Collections.Generic;

namespace Core.API.Models.B2B
{
    public class AllInspectionResponse
    {
        public AllInspectionResponse()
        {
            Company = new CompanyDetails();

            Seller = new Stakeholder();
            Buyer = new Stakeholder();

            PrimaryBroker = new BrokerDetails();
            SharedBrokers = new List<BrokerDetails>();
        }

        public long Id { get; set; }

        public Int16 ApplicationStatusId { get; set; }

        public string ApplicationStatus { get; set; }

        public string ReferenceNumber { get; set; }

        public string ExternalRef { get; set; }


        //public long LenderId { get; set; }

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


        //public bool IsActionRequired { get; set; }

        public CompanyDetails Company { get; set; }

        public Stakeholder Seller { get; set; }

        public Stakeholder Buyer { get; set; }

        public BrokerDetails PrimaryBroker { get; set; }

        public List<BrokerDetails> SharedBrokers { get; set; }
    }
}