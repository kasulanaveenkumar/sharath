using Common.Models.Core.Entities;
using Core.API.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Core.API.Repositories
{
    public class NotificationsRepository : INotificationsRepository
    {
        private readonly CoreContext dbContext;

        #region Constructor

        public NotificationsRepository(CoreContext context)
        {
            dbContext = context;
        }

        #endregion

        #region Get Active Notifications

        public List<AppActivities> GetActiveNotifications()
        {
            // Get Active Notifications
            var responses = dbContext.AppActivities.Where(a => a.IsEnabledForNotifications == true).ToList();

            return responses;
        }

        #endregion

        #region Get NotificationMappings

        public List<NotificationUserMappings> GetNotificationMappings(string userGuid, string companyGuid)
        {
            // Get Notification Mappings
            var responses = dbContext.NotificationUserMappings
                            .Where(num => num.UserGuid == userGuid && 
                                   num.CompanyGuid == companyGuid)
                            .ToList();

            return responses;
        }

        #endregion

        #region Remove NotificationMappings

        public void RemoveNotificationMappings(List<NotificationUserMappings> notificationUserMappings)
        {
            dbContext.NotificationUserMappings.RemoveRange(notificationUserMappings);
            dbContext.SaveChanges();
        }

        #endregion

        #region Save NotificationMappings

        public void SaveNotificationMappings(List<NotificationUserMappings> notificationUserMappings)
        {
            dbContext.NotificationUserMappings.AddRange(notificationUserMappings);
        }

        #endregion

        #region Save Db Changes

        public void SaveDbChanges()
        {
            dbContext.SaveChanges();
        }

        #endregion
    }
}
