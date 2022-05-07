using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class Enums
    {
        public enum UserTypes
        {
            All = 0, // For User Types

            Broker = 1,
            Lender = 2,
            SupportTeam = 3,
            Consumer = 4,
            Seller = 5
        }

        public enum CompanyTypes
        {
            Broker = 1,
            Lender = 2,
        }

        public enum UserRoles
        {
            Temp = -1, // Temp for before login validations

            Onboarding = -2, // For Onboarding API

            AllUserRoles = 0, // For All User Roles. Used only for Role based authorization

            BrokerAdmin = 1,
            BillingResponsible = 2,
            PrimaryContact = 3,
            SimpleBroker = 4,

            Lender = 5,

            SupportTeamAdmin = 6,
            Reviewer = 7,
            Support = 8,

            Consumer = 9,

            DefaultAdmin = 10,

            LenderAdmin = 11,
            SimpleLender = 12,
            SimpleSupportUser = 13

        }

        public enum Role
        {
            Buyer = 1,
            Seller = 2
        }
    }
}
