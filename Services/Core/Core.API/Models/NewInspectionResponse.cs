using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.API.Models.Data;

namespace Core.API.Models
{
    public class NewInspectionResponse
    {
        public NewInspectionResponse()
        {
            Lenders = new List<Lenders>();

            Assets = new List<AssetWorkWithResponse>();

            States = new List<States>();

            BrokerUsers = new List<BrokerUsers>();
        }

        public bool ExemptPayment { get; set; }

        public List<Lenders> Lenders { get; set; }

        public List<AssetWorkWithResponse> Assets { get; set; }

        public List<States> States { get; set; }

        public List<BrokerUsers> BrokerUsers { get; set; }

        public bool IsLender { get; set; }
    }

    public class LenderConfigurations
    {
        public string LenderCompanyGuid { get; set; }

        public string LenderRefPrefix { get; set; }
    }

    public class Lenders
    {
        public string LenderGuid { get; set; }

        public string LenderName { get; set; }

        public bool IsPayer { get; set; }

        public string LenderPrefix { get; set; }

        public bool IsAllowAwaitedRef { get; set; }

        public bool IsForceLenderRefFormat { get; set; }

        public bool IsMapped { get; set; }
    }

    public class TemplateSets
    {
        public string TemplateGuid { get; set; }

        public string TemplateName { get; set; }
    }

    public class States
    {
        public Int64 StateId { get; set; }

        public string StateCode { get; set; }
    }
}
