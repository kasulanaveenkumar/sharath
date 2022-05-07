using Common.Models.Core.Entities;
using Core.API.Models.Data;
using System.Collections.Generic;

namespace Core.API.Repositories
{
    public interface IDataRepository
    {
        public void SaveCompanyDetails(SaveCompanyDetailsRequest model);

        public void SaveUsers(List<SaveUsersRequest> model);

        public Entities.ADUsers GetUserDetailsByUserGuid(string userGuid);

        public List<Entities.ADUsers> GetUsersDetailByUserGuid(List<string> userGuids);

        public Entities.ADUsers GetUserDetailsByUserId(long userId);

        public ADCompanies GetCompanyDetails(string companyGuid);
    }
}
