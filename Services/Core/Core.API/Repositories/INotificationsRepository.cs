using Common.Models.Core.Entities;
using Core.API.Entities;
using System.Collections.Generic;

namespace Core.API.Repositories
{
    public interface INotificationsRepository
    {
        public List<AppActivities> GetActiveNotifications();

        public List<NotificationUserMappings> GetNotificationMappings(string userGuid, string companyGuid);

        public void RemoveNotificationMappings(List<NotificationUserMappings> notificationUserMappings);

        public void SaveNotificationMappings(List<NotificationUserMappings> notificationUserMappings);

        public void SaveDbChanges();
    }
}
