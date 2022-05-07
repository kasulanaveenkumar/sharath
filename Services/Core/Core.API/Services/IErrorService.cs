using System;
using System.Runtime.CompilerServices;

namespace Core.API.Services
{
    public interface IErrorService
    {
        public string SaveError(Exception ex, object requestData, string userGuid, string companyGuid,
                               [CallerMemberName] string member = "", [CallerFilePath] string filePath = "");
    }
}
