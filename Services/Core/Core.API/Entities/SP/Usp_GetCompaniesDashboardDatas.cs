using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.API.Entities.SP
{
    public class Usp_GetCompaniesDashboardDatas
    {
        public Int16 InspectionsStatus { get; set; }

        public Int32 InspectionsCount { get; set; }

        public Int32 CompaniesInvolved { get; set; }

        public Int32 BrokersInvolved { get; set; }

        public string SellerResponseTime { get; set; }

        public string ProcessingTime { get; set; }
    }
}
