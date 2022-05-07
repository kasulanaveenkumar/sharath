using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Config.API.Repositories
{
    public interface IErrorRepository
    {
        public string SaveError(Exception ex, object requestData, string userGuid, string companyGuid,
                                [CallerMemberName] string member = "", [CallerFilePath] string filePath = "");
    }
}
