using Common.Notifications.Email;
using Common.Notifications.Models;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Notifications.InspectionNotifications
{
    public class InspectionStatusNotifier
    {
        #region Send Inspection Notification
        /// <summary>
        /// Send Inspection Notification
        /// </summary>
        /// <param name="model"></param>
        /// <param name="eventType"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static async Task<bool> SendInspectionNotification(InspectionNotification model, Events eventType, IConfiguration configuration)
        {
            /// 1. If email is not null 
            /// 2. Send email based on the status
            ///  
            /// 3. If Target URL Not null
            /// 3. invoke post api with Target URL

            EmailSender emailSender = new EmailSender(eventType, configuration);

            if (!string.IsNullOrEmpty(model.BrokerEmail))
            {
                // Seller Started Inspection
                if (eventType == Events.Inspection_SellerStarted)
                {
                    await emailSender.Send(
                      new EmailModel()
                      {
                          ToEmail = model.BrokerEmail,
                          TemplateData = new
                          {
                              BrokerName = model.BrokerName,
                              SellerName = model.SellerName,
                              InspectionId = model.InspectionId
                          }
                      });
                }
                // Seller Submitted Inspection
                else if (eventType == Events.Inspection_SellerSubmitted)
                {
                    await emailSender.Send(
                      new EmailModel()
                      {
                          ToEmail = model.BrokerEmail,
                          TemplateData = new
                          {
                              BrokerName = model.BrokerName,
                              SellerName = model.SellerName,
                              InspectionId = model.InspectionId
                          }
                      });
                }
                // Admin Rejected Inspection
                else if (eventType == Events.Inspection_AdminRejected)
                {
                    var documentList = new StringBuilder();

                    var documents = model.RejectedDocuments;
                    if (documents != null &&
                        documents.Count() > 0)
                    {
                        documents.ForEach(
                            doc =>
                            {
                                documentList.Append("<li>");
                                documentList.Append(doc.DocName);
                                documentList.Append("</li>");
                            });
                    }

                    await emailSender.Send(
                       new EmailModel()
                       {
                           ToEmail = model.BrokerEmail,
                           TemplateData = new
                           {
                               BrokerName = model.BrokerName,
                               SellerName = model.SellerName,
                               InspectionId = model.InspectionId,
                               Documents = documentList.ToString()
                           }
                       });
                }
                // Admin Processed Inspection
                else if (eventType == Events.Inspection_AdminProcessed)
                {
                    await emailSender.Send(
                       new EmailModel()
                       {
                           ToEmail = model.BrokerEmail,
                           TemplateData = new
                           {
                               BrokerName = model.BrokerName,
                               SellerName = model.SellerName,
                               InspectionId = model.InspectionId
                           }
                       });
                }
            }

            return true;
        }

        #endregion
    }
}
