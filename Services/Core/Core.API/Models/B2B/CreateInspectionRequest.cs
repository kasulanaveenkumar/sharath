using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Core.API.Models.B2B
{
    public class CreateInspectionRequest
    {
        public AppUser Seller { get; set; }

        public AppUser Buyer { get; set; }

        public string LenderGuid { get; set; }

        public string LenderRef { get; set; }

        public string ExternalRef { get; set; }

        public string AssetGuid { get; set; }

        public string PlanGuid { get; set; }

        public string StateCode { get; set; }

        [JsonIgnore]
        public List<string> UsersToShare { get; set; }

        public List<Brokers> Brokers { get; set; }
    }

    public class Brokers
    {
        public string Email { get; set; }

        public bool IsOwner { get; set; }

        [JsonIgnore]
        public string UserGuid { get; set; }

        [JsonIgnore]
        public Int64 UserId { get; set; }
    }
}
