using Config.API.Repositories;
using System;
using System.Runtime.CompilerServices;

namespace Config.API.Services
{
    public class ErrorService : IErrorService
    {
        private readonly IErrorRepository errorRepository;

        #region Constructor

        public ErrorService(IErrorRepository repository)
        {
            this.errorRepository = repository;
        }

        #endregion

        #region Save Error

        public string SaveError(Exception ex, object requestData, string userGuid, string companyGuid,
                                [CallerMemberName] string member = "", [CallerFilePath] string filePath = "")
        {
            // Save Error
            var response = errorRepository.SaveError(ex, requestData, userGuid, member, filePath);

            return response;
        }

        #endregion
    }
}
