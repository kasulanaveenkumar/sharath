using Core.API.Entities.SP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.API.Models
{
    public class CompaniesDashboardResponse
    {
        public CompaniesDashboardResponse()
        {
            InspectionDatas = new List<InspectionDatas>();
        }

        public List<InspectionDatas> InspectionDatas { get; set; }

        public string SellerResponseTime { get; set; }

        public string ProcessingTime { get; set; }
    }

    public class InspectionDatas
    {
        public string InspectionsStatus { get; set; }

        public Int32 InspectionsCount { get; set; }

        public string Percentage { get; set; }

        public Int32 CompaniesInvolved { get; set; }

        public Int32 BrokersInvolved { get; set; }
    }
}
