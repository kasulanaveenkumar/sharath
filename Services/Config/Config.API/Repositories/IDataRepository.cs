using Config.API.Models.Data;
using System.Collections.Generic;

namespace Config.API.Repositories
{
    public interface IDataRepository
    {
        public void SaveUsers(List<SaveUsersRequest> model);

        public void SaveCompanyDetails(SaveCompanyDetailsRequest model, string token);

        public GetUserDetailsResponse GetUserDetailsByEmail(string email, string token);
    }
}
