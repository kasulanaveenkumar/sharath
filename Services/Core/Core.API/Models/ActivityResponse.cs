using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.API.Models
{
    public class ActivityResponse
    {
        public string Role { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Action { get; set; }

        public DateTime ProcessedTime { get; set; }

        [JsonIgnore]
        public string UserGuid { get; set; }

        [JsonIgnore]
        public Int32 UserType { get; set; }
    }
}
