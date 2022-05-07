using Common.Notifications.SMS;
using Core.API.Entities;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Core.API.Helper
{
    public class SmsHelper
    {
        private readonly SmsSender smsSender;

        #region Constructor

        public SmsHelper(IConfiguration configuration)
        {
            smsSender = new SmsSender(configuration);
        }

        #endregion

        #region Send SMS

        public async Task<bool> SendSMS(AppUsers user, string message)
        {
            return await smsSender.SendSMS(user.PhoneNumber, message, user.Email);
        }

        #endregion
    }
}
