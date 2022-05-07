using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.API.Models
{
    public class InspectionPlansRequest
    {
        public string LenderGuid { get; set; }

        public string TemplateGuid { get; set; }

        public Int64 StateId { get; set; }
    }
}
