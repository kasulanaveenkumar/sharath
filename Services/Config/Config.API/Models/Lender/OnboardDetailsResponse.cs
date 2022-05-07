using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Config.API.Models.Lender
{
    public class OnboardDetailsResponse
    {
        public OnboardDetailsResponse()
        {
            Assets = new List<AssetsWorkWithResponse>();

            States = new List<StateOptionsResponse>();
        }

        public List<AssetsWorkWithResponse> Assets { get; set; }

        public List<StateOptionsResponse> States { get; set; }
    }
}
