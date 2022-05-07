using System;
using System.Runtime.CompilerServices;

namespace Core.API.Repositories
{
    public interface IErrorRepository
    {
        public string SaveError(Exception ex, object requestData, string userGuid, string companyGuid,
                                [CallerMemberName] string member = "", [CallerFilePath] string filePath = "");
    }
}
