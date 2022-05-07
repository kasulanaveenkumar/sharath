using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Config.API.Models
{
    public class Enums
    {
        public enum CompanyTypes
        {
            Broker = 1,
            Lender = 2,
        }

        public enum LenderVisibilities
        {
            All = 0,
            Self = 1,
            Selective = 2
        }

        public enum CompanyContactTypes
        {
            PrimaryContact = 1,
            EscalationContact = 2,
            BillingContact = 3
        }

        public enum OptionTypes
        {
            DirectOption = 1,
            State = 2
        }
    }
}
