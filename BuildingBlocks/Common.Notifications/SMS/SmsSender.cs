using Common.Notifications.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Twilio;
using Twilio.Types;
using Twilio.Rest.Api.V2010.Account;
using Common.Notifications.Email;

namespace Common.Notifications.SMS
{
    public class SmsSender
    {
        private readonly string _smsFrom;
        private readonly string _smsAuthToken;
        private readonly string _smsAccountSid;
        private readonly bool _isTestEmailEnabled;
        private readonly bool _isSmsActive;
        private readonly IConfiguration _configuration;


        #region Constructor

        public SmsSender(IConfiguration configuration)
        {
            _smsFrom = configuration.GetSection("SMSSettings").GetSection("SmsFrom").Value;
            _smsAuthToken = configuration.GetSection("SMSSettings").GetSection("SmsAuthToken").Value;
            _smsAccountSid = configuration.GetSection("SMSSettings").GetSection("SmsAccountSid").Value;
            _isTestEmailEnabled = bool.Parse(configuration.GetSection("SMSSettings").GetSection("TestEmailEnabled").Value);
            _isSmsActive = bool.Parse(configuration.GetSection("SMSSettings").GetSection("EnableSMSOption").Value);

            _configuration = configuration;
        }

        #endregion

        #region Send SMS

        public async Task<bool> SendSMS(string smsTo, string message, string email = null)
        {
            try
            {
                if (string.IsNullOrEmpty(smsTo) || string.IsNullOrEmpty(_smsFrom))
                    return false;

                if (!_isSmsActive)
                {
                    if (String.IsNullOrWhiteSpace(email))
                        return false;

                    // Send SMS to Empty Template
                    EmailSender emailSender = new EmailSender(Events.Forward_SMS_To_Email, _configuration);
                    await emailSender.Send(
                        new EmailModel()
                        {
                            ToEmail = email,
                            TemplateData = new
                            {
                                Message = message,
                                Subject = "Verimoto - SMS to " + smsTo
                            }
                        });


                    return true;
                }

                TwilioClient.Init(_smsAccountSid, _smsAuthToken);

                var result = MessageResource.Create(to: new PhoneNumber(smsTo), from: new PhoneNumber(_smsFrom), body: message);

                var errorCode = result.ErrorCode;
                var errorMessage = result.ErrorMessage;
                var body = result.Body;

                string twilioResponse = JsonConvert.SerializeObject(result);

                if (!string.IsNullOrEmpty(result.ErrorMessage))
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        #endregion

        #region Test SMS

        public async Task<bool> TestSMS(string smsTo, string message)
        {
            smsTo = "+91" + smsTo;
            return await SendSMS(smsTo, message);
        }

        #endregion
    }
}
