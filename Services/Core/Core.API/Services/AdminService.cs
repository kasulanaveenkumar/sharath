using Common.AzureBlobUtility.Helper;
using Common.Extensions;
using Core.API.Models;
using Core.API.Repositories;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using TimeZoneConverter;

namespace Core.API.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository adminRepository;
        private readonly AzureBlobHelper azureBlobHelper;

        #region Constructor

        public AdminService(IAdminRepository repository)
        {
            this.adminRepository = repository;

            // Get AccountName value from appsettings.json File
            // Get AccountKey value from appsettings.json File
            // Get Container value from appsettings.json File
            var accountName = Startup.AppConfiguration.GetSection("AppSettings").GetSection("AzureStorageAccountName").Value;
            var accountKey = Startup.AppConfiguration.GetSection("AppSettings").GetSection("AzureStorageAccountKey").Value;
            var containerName = Startup.AppConfiguration.GetSection("AppSettings").GetSection("ContainerNameForAppImages").Value;
            azureBlobHelper = new AzureBlobHelper(accountName, accountKey, containerName);
        }

        #endregion

        #region Get Inspections Filter

        public AdminInspectionsFilterResponse GetInspectionsFilter()
        {
            var response = new AdminInspectionsFilterResponse();

            // Get All Assets
            response.Assets = adminRepository.GetAllAssets();

            // Get All Lenders
            response.Lenders = adminRepository.GetAllLenders();

            // Get Application Statuses
            response.ApplicationStatuses = adminRepository.GetApplicationStatuses();

            return response;
        }

        #endregion

        #region Get Review Inspections List

        public AdminReviewInspectionsResponse GetReviewInspectionsList(AdminInspectionsRequest model, string userGuid, string companyGuid)
        {
            var response = new AdminReviewInspectionsResponse();

            // Get Review Inspections List
            long recordsCount = 0;
            var inspectionsList = adminRepository.GetReviewInspectionsList(model, userGuid, companyGuid, out recordsCount);
            var bypassRequestedStatus = ((Enums.ApplicationStatus)(int)Enums.ApplicationStatus.ByPassRequested).GetEnumDescriptionAttributeValue();
            var submittedStatus = ((Enums.ApplicationStatus)(int)Enums.ApplicationStatus.Submitted).GetEnumDescriptionAttributeValue();
            inspectionsList.ForEach(i =>
            {

                i.Status = ((Enums.ApplicationStatus)(int)i.ApplicationStatus).GetEnumDescriptionAttributeValue();
                i.ReverseTimer = i.UpdatedTime.HasValue
                               ? ((i.Status == submittedStatus || 
                                   i.Status == bypassRequestedStatus)
                                 ? 30 - adminRepository.CalcuteSLADuration(i.UpdatedTime.Value)
                                 : 0)
                               : 0;
            });

            response.InspectionsList = inspectionsList;
            response.TotalRecords = recordsCount;

            return response;
        }

        #endregion

        #region Get ReverseTimer Update Status

        public int GetReverseTimerUpdateStatus()
        {
            TimeSpan businessStartTime = new TimeSpan(9, 0, 0);
            TimeSpan businessEndTime = new TimeSpan(19, 0, 0);

            var tz = TZConvert.GetTimeZoneInfo("AUS Eastern Standard Time");
            DateTime currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);

            if (currentTime.TimeOfDay > businessStartTime &&
                currentTime.TimeOfDay < businessEndTime &&
                currentTime.DayOfWeek != DayOfWeek.Saturday &&
                currentTime.DayOfWeek != DayOfWeek.Sunday)
                return 1;
            else
                return 0;
        }

        #endregion

        #region Suspend Inspection

        public void SuspendInspection(long inspectionId, long userId)
        {
            // Get Inspection Details By Id
            var inspectionDetails = adminRepository.GetInspectionDetailsById(inspectionId);
            if (inspectionDetails != null)
            {
                // Suspend Inspection
                adminRepository.SuspendInspection(inspectionDetails, userId);
            }
        }

        #endregion

        #region Reactive Inspection

        public void ReactiveInspection(long inspectionId, long userId)
        {
            // Get Inspection Details By Id
            var inspectionDetails = adminRepository.GetInspectionDetailsById(inspectionId);
            if (inspectionDetails != null)
            {
                // Reactive Inspection
                adminRepository.ReactiveInspection(inspectionDetails, userId);
            }
        }

        #endregion

        #region Purge Inspection

        public void PurgeInspection(long inspectionId, long userId)
        {
            // Image Purge Process
            azureBlobHelper.DeleteBlobsInAzureStorageContainer(inspectionId);

            // Data Purge Process
            adminRepository.DataPurgeProcess(inspectionId, userId);
        }

        #endregion

        #region Get Completed Inspections List

        public AdminCompletednspectionsResponse GetCompletedInspectionsList(AdminInspectionsRequest model, string userGuid, string companyGuid)
        {
            var response = new AdminCompletednspectionsResponse();

            // Get Compeleted Inspections List
            long recordsCount = 0;
            var inspectionsList = adminRepository.GetCompletedInspectionsList(model, userGuid, companyGuid, out recordsCount);
            var bypassRequestedStatus = ((Enums.ApplicationStatus)(int)Enums.ApplicationStatus.ByPassRequested).GetEnumDescriptionAttributeValue();
            var submittedStatus = ((Enums.ApplicationStatus)(int)Enums.ApplicationStatus.Submitted).GetEnumDescriptionAttributeValue();
            inspectionsList.ForEach(i =>
            {

                i.Status = ((Enums.ApplicationStatus)(int)i.ApplicationStatus).GetEnumDescriptionAttributeValue();
                i.ReverseTimer = i.UpdatedTime.HasValue
                               ? ((i.Status == submittedStatus || 
                                   i.Status == bypassRequestedStatus)
                                 ? 30 - adminRepository.CalcuteSLADuration(i.UpdatedTime.Value)
                                 : 0)
                               : 0;
            });

            response.InspectionsList = inspectionsList;
            response.TotalRecords = recordsCount;

            return response;
        }

        #endregion

        #region Get Companies Dashboard Datas

        public CompaniesDashboardResponse GetCompaniesDashboardDatas(GetCompanyDashboardDatasRequest model)
        {
            var response = adminRepository.GetCompaniesDashboardDatas(model);

            return response;
        }

        #endregion
    }
}
