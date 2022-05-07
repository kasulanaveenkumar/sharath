using Config.API.Models.Asset;
using System;
using System.Collections.Generic;

namespace Config.API.Models.B2B
{
    public class NewInspectionResponse
    {
        public NewInspectionResponse()
        {
            Lenders = new List<LendersWorkWithResponse>();

            Assets = new List<AssetsWorkWithResponse>();

            States = new List<StateOptionsResponse>();

            BrokerUsers = new List<BrokerUsers>();
        }

        public bool ExemptPayment { get; set; }

        public List<LendersWorkWithResponse> Lenders { get; set; }

        public List<AssetsWorkWithResponse> Assets { get; set; }

        public List<StateOptionsResponse> States { get; set; }

        public List<BrokerUsers> BrokerUsers { get; set; }

        public TemplateDetails TemplateDetails { get; set; }
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

    public class AssetWorkWithResponse
    {
        public string TemplateSetGUID { get; set; }

        public string TemplateName { get; set; }

        public bool IsMapped { get; set; }
    }

    public class States
    {
        public Int64 StateId { get; set; }

        public string StateCode { get; set; }
    }

    public class BrokerUsers
    {
        public string UserGuid { get; set; }

        public string Name { get; set; }

        public string SurName { get; set; }

        public string Email { get; set; }

        public string Mobile { get; set; }

        public string CompanyGuid { get; set; }

        public bool IsSelected { get; set; }
    }
}
