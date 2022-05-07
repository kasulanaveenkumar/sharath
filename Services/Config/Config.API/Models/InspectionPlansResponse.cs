using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Config.API.Models
{
    public class InspectionPlansResponse
    {
        public string PlanGuid { get; set; }

        public string PlanName { get; set; }

        public Int16 MaxDocument { get; set; }

        public decimal Price { get; set; }

        public decimal LoanAmount { get; set; }

        public Int16 PlanLevel { get; set; }
    }
}
