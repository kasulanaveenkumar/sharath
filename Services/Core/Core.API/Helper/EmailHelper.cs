using Common.Notifications.Email;
using Common.Notifications.Models;
using Core.API.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.API.Helper
{
    public class EmailHelper
    {
        private readonly IConfiguration _configuration;

        #region Constructor

        public EmailHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #endregion

        #region Send New Inspection Email

        public async void SendNewInspectionEmail(string sellerEmail, string sellerName,
                                                        string userName, string userEmail, string userMobile,
                                                        List<TemplateDocument> documents, string assetType, string mobileWebAppUrl,
                                                        string lenderName, string buyerName, string inspectionId)
        {
            var documentList = new StringBuilder();

            if (documents != null &&
                documents.Count() > 0)
            {
                documents.ForEach(
                    doc =>
                    {
                        //This Hard coding needs to be avoided
                        if (!doc.DocumentName.ToLower().Contains("ppsr"))
                        {
                            // Include short notes based on the document category.
                            string extraText = string.Empty;
                            if (doc.DocumentName != null && doc.DocumentName.ToLower().Contains("driver licence"))
                                extraText = "(it must be the physical and not digital copy)";
                            else if (doc.DocumentName != null && doc.DocumentName.ToLower().Contains("bank statements"))
                                extraText = "(or payout letter when requested)";

                            documentList.Append("<li>");
                            documentList.Append(doc.DocumentName);
                            documentList.Append(extraText);
                            documentList.Append("</li>");
                        }
                    });
            }

            // Send New Inspection Email
            EmailSender emailSender = new EmailSender(Events.Inspection_Created, _configuration);
            await emailSender.Send(
                new EmailModel()
                {
                    ToEmail = sellerEmail,
                    TemplateData = new
                    {
                        SellerName = sellerName,
                        MobileWebAppLink = mobileWebAppUrl,
                        InspectionID = inspectionId,
                        Documents = documentList.ToString(),
                        AssetType = assetType,
                        BrokerName = userName,
                        BrokerEmail = userEmail,
                        BrokerMobile = userMobile,
                        LenderName = lenderName,
                        BuyerName = buyerName
                    }
                });

            //var eventData = new Common.Models.Events.Core.InspectionCreatedEvent();
            //eventData.ToEmail = sellerEmail;
            //eventData.SellerName = sellerName;
            //eventData.BrokerEmail = userDetails.Email;
            //eventData.BrokerMobile = userDetails.Mobile;
            //eventData.BrokerName = userDetails.Name + ' ' + userDetails.SurName;
            //eventData.AssetType = inspectionRepository.GetAssets().FirstOrDefault(a => a.TemplateGuid == templateSetGuid).TemplateName;
            //eventData.Documents = string.Join(Environment.NewLine, documents.Select(d => d.DocumentName));
            //eventData.InspectionID = inspectionId;
            //eventData.MobileWebAppLink = Startup.AppConfiguration.GetSection("BaseURL").GetSection("MobileWebAppURL").Value;
            //messageSender.SendMessage(MessageConstants.QueueName_Notification, eventData);
        }

        #endregion

        #region Send Reminder Email

        public async void SendReminderEmail(string sellerEmail, string reminderMessage)
        {
            EmailSender emailSender = new EmailSender(Events.EditInspection_SendReminder, _configuration);
            await emailSender.Send(
                new EmailModel()
                {
                    ToEmail = sellerEmail,
                    TemplateData = new
                    {
                        ToEmail = sellerEmail,
                        RemainderMessage = reminderMessage
                    }
                });
        }

        #endregion

        #region Send Inspection Rejected Email

        public async void SendInspectionRejectedEmail(string sellerEmail, string sellerName,
                                                             string mobileWebAppURL, Int64 inspectionId,
                                                             string lenderName)
        {
            EmailSender emailSender = new EmailSender(Events.Inspection_Rejected, _configuration);
            await emailSender.Send(
                new EmailModel()
                {
                    ToEmail = sellerEmail,
                    TemplateData = new
                    {
                        SellerName = sellerName,
                        MobileWebAppURL = mobileWebAppURL,
                        InspectionId = inspectionId,
                        LenderName = lenderName
                    }
                });
        }

        #endregion

        #region Send Inspection Submission Email

        public async void SendInspectionSubmissionEmail(string reportEmail, string sellerName,
                                                               string lenderName, Int64 inspectionId,
                                                               string lenderRefNumber, string brokerCompanyName,
                                                               string brokerUser, List<FileAttachments> fileAttachments = null)
        {
            EmailSender emailSender = new EmailSender(Events.Inspection_Submission, _configuration);
            await emailSender.Send(
                new EmailModel()
                {
                    ToEmail = reportEmail,
                    TemplateData = new
                    {
                        LenderName = lenderName,
                        InspectionId = inspectionId,
                        LenderRefNumber = lenderRefNumber,
                        BrokerCompanyName = brokerCompanyName,
                        BrokerUser = brokerUser,
                        SellerName = sellerName
                    },
                    FileAttachments = fileAttachments
                });
        }

        #endregion

        #region Initiate OTP Email

        public async void InitiateOTPEmail(Entities.AppUsers appUser)
        {
            EmailSender emailSender = new EmailSender(Events.WebApp_Login_OTP, _configuration);
            await emailSender.Send(
                new EmailModel()
                {
                    ToEmail = appUser.Email,
                    TemplateData = new
                    {
                        Email = appUser.Email,
                        Name = appUser.Name,
                        SurName = appUser.SurName,
                        OTP = appUser.LoginOTP
                    }
                });
        }
        #endregion

        #region Send Inspection Share Email

        public async void SendInspectionShareEmail(string ownerBrokerName, string sharedBrokerEmail, string sharedBrokerName, 
                                                          long inspectionId, string assetName, string inspectionLink)
        {
            // Send New Inspection Email
            EmailSender emailSender = new EmailSender(Events.Inspection_Shared, _configuration);
            await emailSender.Send(
                new EmailModel()
                {
                    ToEmail = sharedBrokerEmail,
                    TemplateData = new
                    {
                        OwnerBrokerName = ownerBrokerName,
                        SharedBrokerName = sharedBrokerName,
                        InspectionId = inspectionId,
                        AssetName = assetName,
                        InspectionLink = inspectionLink
                    }
                });
        }

        #endregion
    }
}
