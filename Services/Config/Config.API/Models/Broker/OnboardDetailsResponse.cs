using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Config.API.Models.Broker
{
    public class OnboardDetailsResponse
    {
        public OnboardDetailsResponse()
        {
            CompanySuggestions = new List<Entities.Companies>();

            Assets = new List<AssetsWorkWithResponse>();

            Lenders = new List<LendersWorkWithResponse>();

            States = new List<StateOptionsResponse>();
        }

        public List<Entities.Companies> CompanySuggestions { get; set; }

        public List<AssetsWorkWithResponse> Assets { get; set; }

        public List<LendersWorkWithResponse> Lenders { get; set; }

        public List<StateOptionsResponse> States { get; set; }
    }
}
