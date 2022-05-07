using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.API.Models
{
    public class AdminInspectionsFilterResponse
    {
        public AdminInspectionsFilterResponse()
        {
            Assets = new List<TemplateSets>();

            Lenders = new List<Lenders>();

            ApplicationStatuses = new List<ApplicationStatus>();
        }

        public List<Models.TemplateSets> Assets { get; set; }

        public List<Models.Lenders> Lenders { get; set; }

        public List<Models.ApplicationStatus> ApplicationStatuses { get; set; }
    }
}
