using Config.API.Models.Data;
using Config.API.Repositories;
using System.Collections.Generic;

namespace Config.API.Services
{
    public class DataService : IDataService
    {
        private readonly IDataRepository dataRepository;

        #region Constructor

        public DataService(IDataRepository repository)
        {
            this.dataRepository = repository;
        }

        #endregion

        #region Save Users

        public void SaveUsers(List<SaveUsersRequest> model)
        {
            // Save Users
            dataRepository.SaveUsers(model);
        }

        #endregion

        #region Get User Details By Email

        public GetUserDetailsResponse GetUserDetailsByEmail(string email, string token)
        {
            // Get User Details By Email
            var response = dataRepository.GetUserDetailsByEmail(email, token);

            return response;
        }

        #endregion
    }
}
