using Common.Models.Core.Entities;
using Core.API.Models.Data;
using System.Collections.Generic;

namespace Core.API.Services
{
    public interface IDataService
    {
        public void SaveCompanyDetails(SaveCompanyDetailsRequest model);

        public void SaveUsers(List<SaveUsersRequest> model);

        public void SaveUserNotificationMappings(List<NotificationMappingsRequest> model);

        public ADCompanies GetCompanyDetails(string companyGuid);

        public Entities.ADUsers GetUserDetailsByUserId(long userId);
    }
}
