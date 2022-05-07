using System.Collections.Generic;

namespace Config.API.Models.Lender
{
    public class SavePaymentDetailsRequest
    {
        public List<Payment.PaymentMethod> PaymentMethods { get; set; }
    }
}
