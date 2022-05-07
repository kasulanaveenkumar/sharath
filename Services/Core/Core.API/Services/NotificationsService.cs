using Core.API.Models;
using Core.API.Repositories;
using System.Collections.Generic;
using System.Linq;
using Core.API.Entities;
using Common.Models.Core.Entities;

namespace Core.API.Services
{
    public class NotificationsService : INotificationsService
    {
        private readonly INotificationsRepository notificationsRepository;

        #region Constructor

        public NotificationsService(INotificationsRepository repository)
        {
            this.notificationsRepository = repository;
        }

        #endregion

        #region Get Invalid NotificationGuids

        public string GetInvalidNotificationGuids(List<NotificationsRequest> model)
        {
            var response = "";
            var activeNotificationsList = notificationsRepository.GetActiveNotifications().Select(n => n.NotificationGuid).ToList();
            var userNotifications = model.Select(n => n.NotificationsGuid).ToList();

            var result = activeNotificationsList.Except(userNotifications).ToList();
            if (result != null &&
                result.Count() > 0)
            {
                response = string.Join(" , ", result);
            }

            return response;
        }

        #endregion

        #region Get NotificationMappings

        public List<NotificationsResponse> GetNotificationsMappings(string userGuid, string companyGuid)
        {
            var responses = new List<NotificationsResponse>();

            // Get Active Notifications
            var notifications = notificationsRepository.GetActiveNotifications();

            // Get Notifications Mapped to User
            var mappedNotifications = notificationsRepository.GetNotificationMappings(userGuid, companyGuid);

            notifications.ForEach(
                notification =>
                {
                    responses.Add(
                        new NotificationsResponse()
                        {
                            NotificationsGuid = notification.NotificationGuid,
                            EventDescription = notification.NotificationDescription,
                            IsSelected = mappedNotifications.Count(n => n.AppActivityId == notification.Id) > 0
                        });
                });

            return responses;
        }

        #endregion

        #region Save NotificationMappings 

        public void SaveNotificationMappings(List<NotificationsRequest> model, string userGuid, string companyGuid)
        {
            // Remove NotificationMappings
            var notificationsToDelete = notificationsRepository.GetNotificationMappings(userGuid, companyGuid);
            notificationsRepository.RemoveNotificationMappings(notificationsToDelete);

            // Add NotificationMappings
            var notificationUserMappings = new List<NotificationUserMappings>();
            var notifications = notificationsRepository.GetActiveNotifications();
            var selectedNotifications = model.Where(n => n.IsSelected == true).ToList();
            selectedNotifications.ForEach(
                notification =>
                {
                    var notificationDetail = notifications.Where(n => n.NotificationGuid == notification.NotificationsGuid).FirstOrDefault();
                    if (notificationDetail != null)
                    {
                        notificationUserMappings.Add(
                            new NotificationUserMappings()
                            {
                                AppActivityId = notificationDetail.Id,
                                UserGuid = userGuid,
                                CompanyGuid = companyGuid
                            });
                    }
                });
            notificationsRepository.SaveNotificationMappings(notificationUserMappings);
            notificationsRepository.SaveDbChanges();
        }

        #endregion
    }
}
