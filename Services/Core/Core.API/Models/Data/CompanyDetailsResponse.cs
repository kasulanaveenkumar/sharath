using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.API.Models.Data
{
    public class CompanyDetailsResponse
    {
        public string PaymentCustomerId { get; set; }

        public string PrimaryPaymentId { get; set; }
    }
}
