using System;

namespace Core.API.Models.B2B
{
    public class UpdateInspectionRequest
    {
        public Int64 Id { get; set; }

        public string BrokerEmail { get; set; }

        public Stakeholder Buyer { get; set; }

        public Stakeholder Seller { get; set; }

        public string RefNumber { get; set; }

        public string ExternalRefNumber { get; set; }
    }
}