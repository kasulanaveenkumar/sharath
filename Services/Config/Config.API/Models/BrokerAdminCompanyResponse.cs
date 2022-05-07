﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Config.API.Models
{
    public class BrokerAdminCompanyResponse
    {
        public BrokerAdminCompanyResponse()
        {
            Lenders = new List<LendersWorkWithResponse>();
            Assets = new List<AssetsWorkWithResponse>();
        }

        public string CompanyName { get; set; }
        public string RegisteredCompanyName { get; set; }
        public string ABN { get; set; }
        public string CompanyAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Website { get; set; }
        public string Email { get; set; }
        public string CompanyLogo { get; set; }
        public string CompanyDescription { get; set; }
        public List<Models.LendersWorkWithResponse> Lenders { get; set; }
        public List<Models.AssetsWorkWithResponse> Assets { get; set; }
    }
}
