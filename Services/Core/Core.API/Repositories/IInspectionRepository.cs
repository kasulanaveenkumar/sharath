using Common.Models.Core.Entities;
using Core.API.Entities;
using Core.API.Entities.SP;
using System.Collections.Generic;
using System.Linq;

namespace Core.API.Repositories
{
    public interface IInspectionRepository
    {
        public void BeginTransaction();

        public void CommitTransaction();

        public void RollbackTransaction();

        public Applications AddApplication(Applications newApp);

        public void AddStakeHolders(List<AppStakeholders> stakeholders);

        public void AddAppUsers(List<AppUsers> appUsers);

        public void AddOwnerStakeholder(AppStakeholders ownerStakeHoler);

        public void AddAppActivityLogs(AppActivityLogs appActivityLogs);

        public List<Usp_GetInspections> GetAllInspectionsList(Models.BrokerInspectionsRequest model,
                                                              string userGuid, string companyGuid,
                                                              out long recordsCount);

        public List<Usp_GetInspections> GetCompletedInspectionsList(Models.BrokerInspectionsRequest model,
                                                                    string userGuid, string companyGuid,
                                                                    out long recordsCount);

        public List<Models.Lenders> GetLenders();

        public List<Models.Lenders> GetLenders(string companyGuid);

        public List<Models.Lenders> GetLenders(List<Models.Data.LenderWorkWithResponse> lenders);

        public List<Models.Lenders> GetAllLenders();

        public List<Models.TemplateSets> GetAllAssets();

        public List<Models.ApplicationStatus> GetApplicationStatuses();

        public List<Models.States> GetAllStates();

        public Models.InspectionDetails GetInspectionDetails(long inspectionId, List<Models.Lenders> lenders);

        public Applications GetInspectionDetails(long inspectionId);

        public Applications GetInspectionDetails(string inspectionGuid);

        public List<AppStakeholders> GetInspectionDetails(long inspectionId, string userGuid);

        public int GetUserMappedInspectionDetailsCount(string userGuid, string companyGuid);

        public List<Models.ActivityLogs> GetActivityLogs(long inspectionId);

        public string GetCSVFileDownloadString(long inspectionId);

        public List<Models.InspectionProgress> GetInspectionProgress(long inspectionId);

        public List<Models.Reminders> GetActiveReminders();

        public List<Models.BrokerUsers> GetBrokerUsers(string userGuid, string companyGuid, long inspectionId);

        public List<Models.InspectionPlansResponse> GetTemplatePlans(Models.InspectionPlansRequest model, string token);

        public void AddAppDocumentsImages(List<Models.TemplateDocument> templateDocuments, long applicationId);

        public void UpdateTemplateOrPlan(List<Models.TemplateDocument> templateDocuments, long applicationId);

        public List<Models.ApplicationStatus> GetInspectionStatuses(bool isSupportTeam = false);

        public List<AppUsers> GetBuyerListForUpdate(List<AppUsers> appUsers, Models.AppUser model);

        public List<AppUsers> GetSellerListForUpdate(List<AppUsers> appUsers, Models.AppUser model);

        public void UpdateAppUsers(List<AppUsers> appUsers);

        public List<AppUsers> GetAppUsers(long applicationId);

        public List<AppDocuments> GetAppDocuments(long inspectionId);

        public List<Models.TemplateDocument> GetTemplateDocumentsList(long inspectionId);

        public List<Models.AppDocuments> GetInspectionDocumentsList(long inspectionId, LenderConfigurations lenderConfig);

        public void UpdateInspection(Applications application, Models.EditInspectionRequest model, long userId);

        public void UpdateInspection(Applications application, long userId, bool isInspectionRejected = false);

        public void SaveSharedUsers(List<string> usersToShare, long inspectionId);

        public List<Models.CoreConfigsResponse> GetCoreConfigsValue(string name);

        public Models.ReminderResponse GetReminderDatasByInspectionId(long inspectionId);

        public string GetReminderMessage(Models.ReminderResponse reminderDatas, List<Models.CoreConfigsResponse> coreConfigValues, 
                                         string message, long inspectionId);

        public void CancelInspection(Applications application, long userId);

        public LenderConfigurations GetLenderConfigurationByGuid(string lenderGuid);

        public void UpdateLenderConfiguration(LenderConfigurations lenderConfiguration);

        public AppImages GetAppImageDetailsById(long imageId);

        public AppImages GetAppImageDetailsById(long imageId, long applicationId);

        public void UpdateAppImages(AppImages appImage);

        public void UpdateAppDocuments(AppDocuments appDocument);

        public Models.InspectionGetImageResponse GetImageData(AppImages appImage);

        public List<AppImages> GetAppImagesByInspectionId(long applicationId);

        public List<AppImageReasons> GetAppImageReasonsByInspectionId(long inspectionId);

        public void AddAppImageReasons(List<AppImageReasons> appImageReasons);

        public void DeleteAppImageReasons(List<AppImageReasons> appImageReasons);

        public void SaveDbChanges();

        public IQueryable<Models.NotificationsList> GetNotificationsList(string userGuid, string companyGuid);

        public int GetNotificationsCount(string userGuid, string companyGuid);

        public void UpdateNotificationStatus(string userGuid, string companyGuid);

        public void SavePaymentLog(PaymentLogs paymentLogs);

        public decimal GetTemplateSetPlanPrice(string templateSetGuid, string planGuid, string lenderGuid, bool isLenderPayer);

        public void UpdateLastNotifiedTime(long inspectionId, long userId);

        public void SaveDVSChecks(DVSChecks dvsChecks);

        public void UpdateDVSStatus(long inspectionId, int dvsStatus, long userId);

        public ADTemplateSets GetTemplateSetDetailsByGuid(string templateSetGuid);

        public ADCompanies GetLenderDetailsByGuid(string lenderGuid);

        public bool VerifyInspectionPermission(long inspectionId, string userGuid);

        public IllionIntegrationDetails GetIllionDetailsByCompanyGuid(string companyGuid);

        public void SaveIllionDetails(IllionIntegrationDetails illionIntegrationDetails);
    }
}
