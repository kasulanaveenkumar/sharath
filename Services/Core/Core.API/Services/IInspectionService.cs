using Common.Identity.Models.DVS;
using Common.Models.Core.Entities;
using Core.API.Entities;
using System;
using System.Collections.Generic;
using System.Net;

namespace Core.API.Services
{
    public interface IInspectionService
    {
        public Models.NewInspectionResponse NewInspectionDetails(string token, long userTypeId, string userGuid, string companyGuid);

        public Int64 CreateNewInspection(Models.NewInspectionRequest model, long userId, string token, string companyGuid, 
                                         string userGuid, long userTypeId, out string paymentFailedReason, out Exception errorMessage);

        public List<Models.ApplicationStatus> GetInspectionStatuses(bool isSupportTeam = false);

        public Models.AdminInspectionsFilterResponse GetInspectionsFilter();

        public Models.BrokerAllInspectionsResponse GetAllInspectionsList(Models.BrokerInspectionsRequest model,
                                                                         string userGuid, string companyGuid);

        public Models.BrokerCompletedInspectionsResponse GetCompletedInspectionsList(Models.BrokerInspectionsRequest model,
                                                                                     string userGuid, string companyGuid);

        public Models.EditInspectionResponse EditInspectionDetails(Models.EditInspectionRequest model, string token, string userGuid, string companyGuid);

        public int EditInspection(Applications inspection, Models.EditInspectionRequest model, string token, 
                                  long userId, string userGuid, int userTypeId,
                                  out string paymentFailedReason, out string applicationStatus);

        public string GetActivityLogsData(long inspectionId);

        public int SendReminder(Models.ReminderRequest model, long userId);

        public void UpdateBypassReason(AppImages imageDetail, Models.InspectionBypassRequest model, int userTypeId, 
                                       string userGuid, long userId);

        public void CancelInspection(long inspectionId, long userId);

        public Models.InspectionGetImageResponse GetImageData(AppImages imageDetails, long inspectionId, long imageId);

        public Models.ReviewDataResponse GetReviewData(long inspectionId);

        public int SaveReviewData(Models.ReviewInspectionSaveRequest model, string userGuid, long userId, out string errorMessage);

        public string RotateImage(AppImages imageDetails, Models.ReviewInspectionRotateImageRequest model, long userId);

        public int MoveImage(Models.ReviewInspectionMoveImageRequest model, long userId);

        public Models.NotificationsDetailsResponse GetNotificationsList(Models.NotificationsDetailsRequest model, string userGuid, string companyGuid);

        public int GetNotificationsCount(string userGuid, string companyGuid);

        public void UpdateNotificationStatus(string userGuid, string companyGuid);

        public int CheckDVS(CheckDVSRequest model, long userId);

        public void PingPPSR(Models.PPSRModel model, out string result, out HttpStatusCode httpStatusCode);

        public int GetUserMappedInspectionDetailsCount(string userGuid, string companyGuid);

        public int SendSubmissionReport(Applications inspectionDetails, Entities.ADUsers brokerDetails, AppUsers sellerDetails,
                                        ADCompanies lenderDetails, string inspectionGuid, bool isGenerateReport, bool isSendEmail,
                                        out string errorMessage);

        public string GetReport(ref long inspectionId, string inspectionGuid);

        public Applications GetInspectionDetailsById(long inspectionId);

        public Applications GetInspectionDetailsByGuid(string inspectionGuid);

        public AppImages GetAppImageDetail(long imageId);

        public AppImages GetAppImageDetail(long imageId, long inspectionId);

        public bool VerifyInspectionPermission(long inspectionId, string userGuid);

        public Models.GetIllionDetailsResponse GetIllionDetails(string companyGuid);

        public void SaveIllionDetails(string companyGuid, Models.UpdateIllionDetailsRequest model);

        public void SendInspectionSharedEmail(long userTypeId, long inspectionId, Models.TemplateSets asset,
                                              List<string> usersToShare, Entities.ADUsers userDetails);

        public List<AppUsers> GetAppUsers(long inspectionId);
    }
}