using Config.API.Models.Data;
using System.Collections.Generic;

namespace Config.API.Services
{
    public interface IDataService
    {
        public void SaveUsers(List<SaveUsersRequest> model);

        public GetUserDetailsResponse GetUserDetailsByEmail(string email, string token);
    }
}
