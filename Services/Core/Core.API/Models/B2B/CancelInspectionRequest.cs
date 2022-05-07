using System;

namespace Core.API.Models.B2B
{
    public class CancelInspectionRequest
    {
        public Int64 Id { get; set; }

        public string BrokerEmail { get; set; }
    }
}
