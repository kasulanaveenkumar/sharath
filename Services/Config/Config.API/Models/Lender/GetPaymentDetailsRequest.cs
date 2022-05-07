namespace Config.API.Models.Lender
{
    public class GetPaymentDetailsRequest
    {
        public bool IsPaymentByInvoice { get; set; }

        public bool IsPaymentByCard { get; set; }
    }
}
