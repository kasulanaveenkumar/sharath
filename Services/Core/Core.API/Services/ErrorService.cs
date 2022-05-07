using Core.API.Repositories;
using System;
using System.Runtime.CompilerServices;

namespace Core.API.Services
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
            var response = errorRepository.SaveError(ex, requestData, userGuid, companyGuid, member, filePath);

            return response;
        }

        #endregion
    }
}
