using Common.Models.Core.Entities;
using Core.API.Entities;
using Core.API.Models.Data;
using Core.API.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace Core.API.Services
{
    public class DataService : IDataService
    {
        private readonly IDataRepository dataRepository;
        private readonly INotificationsRepository notificationsRepository;

        #region Constructor

        public DataService(IDataRepository repository, INotificationsRepository notificationsRepository)
        {
            this.dataRepository = repository;
            this.notificationsRepository = notificationsRepository;
        }

        #endregion

        #region Save Company Details

        public void SaveCompanyDetails(SaveCompanyDetailsRequest model)
        {
            // Save Company Details
            dataRepository.SaveCompanyDetails(model);
        }

        #endregion

        #region Save Users

        public void SaveUsers(List<SaveUsersRequest> model)
        {
            // Save Users
            dataRepository.SaveUsers(model);
        }

        #endregion

        #region Save User NotificationMappings

        public void SaveUserNotificationMappings(List<NotificationMappingsRequest> model)
        {
            model.ForEach(
                userNotification =>
                {
                    // Remove NotificationMappings
                    var notificationsToDelete = notificationsRepository.GetNotificationMappings(userNotification.UserGuid, userNotification.CompanyGuid);
                    notificationsRepository.RemoveNotificationMappings(notificationsToDelete);

                    // Add NotificationMappings
                    var notificationUserMappings = new List<NotificationUserMappings>();
                    var notifications = notificationsRepository.GetActiveNotifications();
                    notifications.ForEach(
                        notification =>
                        {
                            var notificationDetail = notifications.Where(n => n.NotificationGuid == notification.NotificationGuid).FirstOrDefault();
                            if (notificationDetail != null)
                            {
                                notificationUserMappings.Add(
                                    new NotificationUserMappings()
                                    {
                                        AppActivityId = notificationDetail.Id,
                                        UserGuid = userNotification.UserGuid,
                                        CompanyGuid = userNotification.CompanyGuid
                                    });
                            }
                        });
                    notificationsRepository.SaveNotificationMappings(notificationUserMappings);
                });

            notificationsRepository.SaveDbChanges();
        }

        #endregion

        #region Get Company Details

        public ADCompanies GetCompanyDetails(string companyGuid)
        {
            var response = dataRepository.GetCompanyDetails(companyGuid);

            return response;
        }

        #endregion

        #region Get User Details

        public Entities.ADUsers GetUserDetailsByUserId(long userId)
        {
            var response = dataRepository.GetUserDetailsByUserId(userId);
            return response;
        }

        #endregion
    }
}
