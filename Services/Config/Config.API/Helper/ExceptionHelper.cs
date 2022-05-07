using System;
using Common.Messages;
using Config.API.Services;

namespace Config.API.Helper
{
    public class ExceptionHelper
    {
        #region Get Exception Msg

        public static string GetExceptionMsg(IErrorService errorService, Exception ex, object requestData, string userGuid, string companyGuid)
        {
            var errorId = errorService.SaveError(ex, requestData, userGuid, companyGuid);
            var msg = string.Format(CommonMessages.Error_Message, errorId);
            return msg;
        }

        #endregion
    }
}
