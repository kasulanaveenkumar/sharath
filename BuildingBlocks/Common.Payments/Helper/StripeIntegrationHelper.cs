using Common.Messages;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Payments.Helper
{
    public class StripeIntegrationHelper
    {
        private PaymentMethodService paymentMethodService = new PaymentMethodService();

        #region Constructor

        public StripeIntegrationHelper(string stripeApiKey)
        {
            StripeConfiguration.ApiKey = stripeApiKey;
        }

        #endregion

        #region Add New Customer Details

        public string AddNewCustomerDetails(string companyName, string email, string abn)
        {
            var customerService = new CustomerService();

            var customerCreateOptions = new CustomerCreateOptions
            {
                Name = companyName,
                Email = email,
                Description = abn
            };

            var newCustomer = customerService.Create(customerCreateOptions);

            return newCustomer.Id;
        }

        #endregion

        #region Save Card Details

        public List<Models.CardDetails> SaveCardDetails(string customerId, List<Models.CardDetails> cardDetails)
        {
            cardDetails.ForEach(
                card =>
                {
                    // Delete Card Details
                    if (card.IsDelete &&
                        !string.IsNullOrEmpty(card.PaymentMethodId))
                    {
                        paymentMethodService.Detach(card.PaymentMethodId);
                    }

                    // Add New Card Details
                    if (string.IsNullOrEmpty(card.PaymentMethodId))
                    {
                        var paymentMethodCreateOptions = new PaymentMethodCreateOptions
                        {
                            Type = "card",
                            Card = new PaymentMethodCardOptions
                            {
                                Number = card.CardNumber,
                                ExpMonth = card.ExpMonth,
                                ExpYear = card.ExpYear,
                                Cvc = card.Cvc
                            },
                            Metadata = new Dictionary<string, string>()
                            {
                                { "CardHolderName", card.CardHolderName },
                                { "Cvc", card.Cvc }
                            }
                        };

                        try
                        {
                            var paymentMethod = paymentMethodService.Create(paymentMethodCreateOptions);
                            var paymentMethodAttachOptions = new PaymentMethodAttachOptions
                            {
                                Customer = customerId
                            };

                            paymentMethodService.Attach(paymentMethod.Id, paymentMethodAttachOptions);

                            card.PaymentMethodId = paymentMethod.Id;
                            card.ErrorMessage = string.Empty;
                        }
                        catch (StripeException ex)
                        {
                            card.ErrorMessage = ConfigMessages.PaymentDetails_Error_InvalidCardDetails;
                        }
                    }
                });

            return cardDetails;
        }

        #endregion

        #region Get Cards Details Mapped To Customer

        public List<Models.CardDetails> GetCardsDetailsMappedToCustomer(string customerId, string primaryPaymentId)
        {
            var cardDetails = new List<Models.CardDetails>();

            var paymentMethodListOptions = new PaymentMethodListOptions
            {
                Type = "card",
                Customer = customerId
            };

            StripeList<PaymentMethod> paymentMethods = paymentMethodService.List(paymentMethodListOptions);

            paymentMethods.Data.ForEach(
                data =>
                {
                    var cardHolderName = "";
                    var cvc = "";
                    data.Metadata.TryGetValue("CardHolderName", out cardHolderName);
                    data.Metadata.TryGetValue("Cvc", out cvc);

                    var expMonth = data.Card.ExpMonth < 10
                                 ? string.Join("", "0", data.Card.ExpMonth)
                                 : data.Card.ExpMonth.ToString();

                    var expYear = data.Card.ExpYear.ToString();

                    var expDate = string.Join("/", expMonth, expYear);

                    cardDetails.Add(
                        new Models.CardDetails()
                        {
                            CardHolderName = cardHolderName,
                            CardNumber = string.Join("", "XXXX XXXX XXXX ", data.Card.Last4),
                            ExpMonth = data.Card.ExpMonth,
                            ExpYear = data.Card.ExpYear,
                            ExpDate = expDate,
                            Cvc = cvc,
                            Brand = data.Card.Brand,
                            IsPrimary = primaryPaymentId == data.Id
                                      ? true
                                      : false,
                            PaymentMethodId = data.Id,
                            CustomerId = data.CustomerId,
                            IsCardSelected = primaryPaymentId == data.Id
                                           ? true
                                           : false
                        });
                });

            return cardDetails;
        }

        #endregion

        #region Process Payment

        public void ProcessPayment(long amount, string description, string customerId, string paymentMethodId, out string transactionId, out string failedReason)
        {
            PaymentIntent response = null;
            try
            {
                var paymentIntentCreateOptions = new PaymentIntentCreateOptions
                {
                    Amount = amount,
                    Currency = "AUD",
                    PaymentMethodTypes = new List<string>
                    {
                        "card"
                    },
                    Description = description, //Inspection ID
                    Customer = customerId,
                    PaymentMethod = paymentMethodId
                };
                var paymentIntentService = new PaymentIntentService();
                var paymentIntent = paymentIntentService.Create(paymentIntentCreateOptions);
                var paymentIntentConfirmOptions = new PaymentIntentConfirmOptions
                {
                    PaymentMethod = paymentIntent.PaymentMethodId,
                };

                response = paymentIntentService.Confirm(paymentIntent.Id, paymentIntentConfirmOptions);

                failedReason = "";

                transactionId = response.Charges.Data[0].Id;
            }
            catch (StripeException ex)
            {
                failedReason = ex.Message;

                transactionId = ex.StripeError.Charge;
            }
        }

        #endregion

        #region Validate Card

        public Models.CardDetails ValidateCard(Models.CardDetails cardDetails)
        {
            try
            {
                var options = new TokenCreateOptions
                {
                    Card = new TokenCardOptions
                    {
                        Number = cardDetails.CardNumber,
                        ExpMonth = cardDetails.ExpMonth,
                        ExpYear = cardDetails.ExpYear,
                        Cvc = cardDetails.Cvc,
                    },
                };
                var service = new TokenService();
                var result = service.Create(options);

                cardDetails.ErrorMessage = "";
            }
            catch (StripeException ex)
            {
                cardDetails.ErrorMessage = string.Join(".", ConfigMessages.PaymentDetails_Error_InvalidCardDetails, ex.Message);
            }

            return cardDetails;
        }

        #endregion
    }
}
