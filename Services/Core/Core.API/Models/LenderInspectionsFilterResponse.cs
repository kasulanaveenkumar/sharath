using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.API.Models
{
    public class LenderInspectionsFilterResponse
    {
        public LenderInspectionsFilterResponse()
        {
            Assets = new List<TemplateSets>();

            Brokers = new List<Brokers>();

            ApplicationStatuses = new List<ApplicationStatus>();
        }

        public List<Models.TemplateSets> Assets { get; set; }

        public List<Models.Brokers> Brokers { get; set; }

        public List<Models.ApplicationStatus> ApplicationStatuses { get; set; }
    }

    public class Brokers
    {
        public string CompanyGuid { get; set; }

        public string CompanyName { get; set; }
    }
}
