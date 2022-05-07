using Config.API.Models.Broker;
using System;
using System.Collections.Generic;

namespace Config.API.Models
{
    public class SaveCompanyDetailsResponse
    {
        public SaveCompanyDetailsResponse()
        {
            Lenders = new List<LendersWorkWithResponse>();

            Assets = new List<AssetsWorkWithResponse>();
        }

        public Int64 CompanyId { get; set; }

        public string CompanyName { get; set; }

        public string RegisteredName { get; set; }

        public string ABN { get; set; }

        public string CompanyAddress { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string ZipCode { get; set; }

        public string Website { get; set; }

        public string Email { get; set; }

        public bool ExcemptPayment { get; set; }

        public bool AllowOnlyInvitedUser { get; set; }

        public string LiveStatus { get; set; }

        public string CompanyLogo { get; set; }

        public string CompanyDescription { get; set; }

        public List<LendersWorkWithResponse> Lenders { get; set; }

        public List<AssetsWorkWithResponse> Assets { get; set; }

        public List<BrokerUserResponse> BrokerUsers { get; set; }
    }
}
