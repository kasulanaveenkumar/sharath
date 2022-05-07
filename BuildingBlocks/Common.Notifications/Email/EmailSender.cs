using Common.Notifications.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Common.Notifications.Email
{
    public class EmailSender
    {
        private readonly string _sendGridKey;
        private readonly string _from;
        private readonly string _fromName;
        private readonly bool _isTestEmailEnabled;
        private readonly string _testEmailGroup;
        private readonly string _templateId;

        #region Constructor

        public EmailSender(Events eventType, IConfiguration configuration)
        {
            _sendGridKey = configuration.GetSection("EmailSettings").GetSection("SendGridKey").Value;
            _from = configuration.GetSection("EmailSettings").GetSection("From").Value;
            _fromName = configuration.GetSection("EmailSettings").GetSection("FromName").Value;
            _isTestEmailEnabled = bool.Parse(configuration.GetSection("EmailSettings").GetSection("TestEmailEnabled").Value);
            _testEmailGroup = configuration.GetSection("EmailSettings").GetSection("TestEmailGroup").Value;

            // Get Send Grid Template Id
            switch (eventType)
            {
                case Events.User_Login_OTP:
                    _templateId = configuration.GetSection("EmailSettings").GetSection("SendGridUserLoginOTPTemplateId").Value;
                    break;

                case Events.User_Registration:
                    _templateId = configuration.GetSection("EmailSettings").GetSection("SendGridUserRegistrationTemplateId").Value;
                    break;

                case Events.User_Reset_Password:
                    _templateId = configuration.GetSection("EmailSettings").GetSection("SendGridUserResetPasswordTemplateId").Value;
                    break;

                case Events.User_Broker_Invite:
                    _templateId = configuration.GetSection("EmailSettings").GetSection("SendGridUserBrokerInviteTemplateId").Value;
                    break;

                case Events.User_ExistingUser_Invite:
                    _templateId = configuration.GetSection("EmailSettings").GetSection("SendGridExistingUserInviteNotificationTemplateId").Value;
                    break;

                case Events.User_Broker_JoinExistingCompany:
                    _templateId = configuration.GetSection("EmailSettings").GetSection("SendGridUserBrokerJoinExistingCompanyTemplateId").Value;
                    break;

                case Events.Inspection_Created:
                    _templateId = configuration.GetSection("EmailSettings").GetSection("SendGridCoreNewInspectionTemplateId").Value;
                    break;

                case Events.EditInspection_SendReminder:
                    _templateId = configuration.GetSection("EmailSettings").GetSection("SendGridCoreEditInspectionSendReminderTemplateId").Value;
                    break;

                case Events.Inspection_Rejected:
                    _templateId = configuration.GetSection("EmailSettings").GetSection("SendGridCoreInspectionRejectedTemplateId").Value;
                    break;

                case Events.Inspection_Submission:
                    _templateId = configuration.GetSection("EmailSettings").GetSection("SendGridCoreInpsectionSubmissionTemplateId").Value;
                    break;

                case Events.Inspection_Shared:
                    _templateId = configuration.GetSection("EmailSettings").GetSection("SendGridCoreInpsectionShareTemplateId").Value;
                    break;

                case Events.WebApp_Login_OTP:
                    _templateId = configuration.GetSection("EmailSettings").GetSection("SendGridCoreWebAppLoginOTPTemplateId").Value;
                    break;

                case Events.WebApp_SendOpenInspection_Ids:
                    _templateId = configuration.GetSection("EmailSettings").GetSection("SendGridCoreWebAppSendOpenInspectionIdsTemplateId").Value;
                    break;

                case Events.Forward_SMS_To_Email:
                    _templateId = configuration.GetSection("EmailSettings").GetSection("SendGridForwardSMSToTemplateId").Value;
                    break;

                case Events.Inspection_SellerStarted:
                    _templateId = configuration.GetSection("EmailSettings").GetSection("SendGridSellerStartedInspectionTemplateId").Value;
                    break;

                case Events.Inspection_SellerSubmitted:
                    _templateId= configuration.GetSection("EmailSettings").GetSection("SendGridSellerSubmittedInspectionTemplateId").Value;
                    break;

                case Events.Inspection_AdminRejected:
                    _templateId = configuration.GetSection("EmailSettings").GetSection("SendGridAdminRejectedInspectionTemplateId").Value;
                    break;

                case Events.Inspection_AdminProcessed:
                    _templateId = configuration.GetSection("EmailSettings").GetSection("SendGridAdminProcessedInspectionTemplateId").Value;
                    break;

                case Events.User_ImportBroker_Invite:
                    _templateId = configuration.GetSection("EmailSettings").GetSection("SendGridImportBrokerInviteTemplateId").Value;
                    break;
            }
        }

        #endregion

        #region Send Mail

        public async Task<bool> Send(EmailModel model)
        {
            bool result = false;

            var client = new SendGridClient(_sendGridKey);

            var sendGridMessage = new SendGridMessage()
            {
                From = new EmailAddress(_from, _fromName),
                TemplateId = _templateId
            };

            var personalization = new CustomPersonalization
            {
                Tos = new List<EmailAddress>()
            };

            if (_isTestEmailEnabled)
            {
                sendGridMessage.Subject = $"Verimoto for {model.ToEmail} | " + sendGridMessage.Subject;
                personalization.Tos.Add(new EmailAddress(_testEmailGroup, "Verimoto Test"));
            }
            else
            {
                personalization.Tos.Add(new EmailAddress(model.ToEmail, model.ToEmail));
            }

            personalization.DynamicTemplateData = model.TemplateData;

            sendGridMessage.Personalizations = new List<Personalization>
                {
                    personalization
                };

            // If Attachments included?
            if (model.FileAttachments != null &&
                model.FileAttachments.Count() > 0)
            {
                model.FileAttachments.ForEach(
                    attachment =>
                    {
                        sendGridMessage.AddAttachment(attachment.FileName, attachment.Base64FileData);
                    });
            }

            var response = await client.SendEmailAsync(sendGridMessage);

            if (response.IsSuccessStatusCode)
            {
                result = true;
            }

            return result;
        }

        [JsonObject(IsReference = false)]
        public class CustomPersonalization : Personalization
        {
            [JsonProperty(PropertyName = "dynamic_template_data", IsReference = false)]
            public object DynamicTemplateData { get; set; }
        }

        #endregion
    }
}
