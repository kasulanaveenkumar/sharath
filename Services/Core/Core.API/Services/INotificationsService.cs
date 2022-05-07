using System.Collections.Generic;

namespace Core.API.Services
{
    public interface INotificationsService
    {
        public string GetInvalidNotificationGuids(List<Models.NotificationsRequest> model);

        public List<Models.NotificationsResponse> GetNotificationsMappings(string userGuid, string companyGuid);

        public void SaveNotificationMappings(List<Models.NotificationsRequest> model, string userGuid, string companyGuid);
    }
}
