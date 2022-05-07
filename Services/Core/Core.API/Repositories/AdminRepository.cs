using Common.Extensions;
using Core.API.Entities;
using Core.API.Entities.SP;
using Core.API.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TimeZoneConverter;

namespace Core.API.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly CoreContext dbContext;

        #region Constructor

        public AdminRepository(CoreContext context)
        {
            dbContext = context;
        }

        #endregion

        #region Get All Assets

        public List<Models.TemplateSets> GetAllAssets()
        {
            // Get All Assets
            var responses = (from templateSet in dbContext.ADTemplateSets
                             orderby templateSet.TemplateName
                             select new Models.TemplateSets()
                             {
                                 TemplateGuid = templateSet.TemplateSetGUID,
                                 TemplateName = templateSet.TemplateName
                             }).ToList();

            return responses;
        }

        #endregion

        #region Get All Lenders

        public List<Models.Lenders> GetAllLenders()
        {
            // Get All Lenders
            var responses = (from c in dbContext.ADCompanies
                             where c.CompanyTypeId == (int)Common.Models.Enums.CompanyTypes.Lender
                             orderby c.CompanyName
                             select new Models.Lenders()
                             {
                                 LenderGuid = c.CompanyGuid,
                                 LenderName = c.CompanyName
                             }).ToList();

            return responses;
        }

        #endregion

        #region Get Application Statuses

        public List<Models.ApplicationStatus> GetApplicationStatuses()
        {
            var applicationStatuses = new List<ApplicationStatus>();

            // Get Application Statuses
            var responses = ExtensionMethods.GetEnumValuesAndDescriptions<Enums.ApplicationStatus>().ToList();
            responses.ForEach(
                status =>
                {
                    applicationStatuses.Add(
                        new Models.ApplicationStatus()
                        {
                            StatusId = status.Value,
                            Description = status.Key
                        });
                });

            return applicationStatuses;
        }

        #endregion

        #region Get Review Inspections List

        public List<Usp_GetInspections> GetReviewInspectionsList(AdminInspectionsRequest model, string userGuid, string companyGuid,
                                                                 out long recordsCount)
        {
            recordsCount = 0;

            var userGuidParam = new SqlParameter { ParameterName = "UserGuid", SqlDbType = SqlDbType.VarChar, Value = userGuid };
            var companyGuidParam = new SqlParameter { ParameterName = "CompanyGuid", SqlDbType = SqlDbType.VarChar, Value = companyGuid };

            var assetFilter = string.IsNullOrEmpty(model.AssetFilter)
                            ? (object)DBNull.Value
                            : model.AssetFilter;
            var assetFilterParam = new SqlParameter { ParameterName = "AssetFilter", SqlDbType = SqlDbType.VarChar, Value = assetFilter };

            var lenderFilter = string.IsNullOrEmpty(model.LenderFilter)
                             ? (object)DBNull.Value
                             : model.LenderFilter;
            var lenderFilterParam = new SqlParameter { ParameterName = "LenderFilter", SqlDbType = SqlDbType.VarChar, Value = lenderFilter };

            var statusFilterParam = new SqlParameter { ParameterName = "StatusFilter", SqlDbType = SqlDbType.Int, Value = model.StatusFilter };

            var isRequestedFromCompletedParam = new SqlParameter { ParameterName = "IsRequestedFromCompleted", SqlDbType = SqlDbType.Bit, Value = 0 };

            var filterText = string.IsNullOrEmpty(model.FilterText) ? (object)DBNull.Value : model.FilterText;
            var filterTextParam = new SqlParameter { ParameterName = "FilterText", SqlDbType = SqlDbType.VarChar, Value = filterText };

            var sortColumnParam = new SqlParameter { ParameterName = "SortColumn", SqlDbType = SqlDbType.VarChar, Value = model.SortColumn };
            var sortDirectionParam = new SqlParameter { ParameterName = "SortDirection", SqlDbType = SqlDbType.VarChar, Value = model.SortDirection };

            var skipDataParam = new SqlParameter { ParameterName = "SkipData", SqlDbType = SqlDbType.Int, Value = model.SkipData };
            var limitDataParam = new SqlParameter { ParameterName = "LimitData", SqlDbType = SqlDbType.Int, Value = model.LimitData };

            var isNewUserParam = new SqlParameter { ParameterName = "IsNewUser", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output };

            var result = dbContext.InspectionsList.FromSqlRaw<Usp_GetInspections>
                         (
                           "Usp_GetInspections @UserGuid, @CompanyGuid, @AssetFilter, @LenderFilter, @StatusFilter, @IsRequestedFromCompleted, @FilterText," +
                           "@SortColumn, @SortDirection, @SkipData, @LimitData, @IsNewUser OUTPUT",
                           userGuidParam, companyGuidParam, assetFilterParam, lenderFilterParam, statusFilterParam, isRequestedFromCompletedParam, filterTextParam,
                           sortColumnParam, sortDirectionParam, skipDataParam, limitDataParam, isNewUserParam
                         )
                         .ToList();

            if (result != null &&
                result.Count() > 0)
            {
                recordsCount = result.FirstOrDefault().TotalRecords;
            }

            return result;
        }

        #endregion

        #region Calcute SLA Duration

        public int CalcuteSLADuration(DateTime uploadedTime)
        {
            TimeSpan businessStartTime = new TimeSpan(9, 0, 0);
            TimeSpan businessEndTime = new TimeSpan(19, 0, 0);

            var tz = TZConvert.GetTimeZoneInfo("AUS Eastern Standard Time");
            DateTime currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
            DateTime revisedUploadedTime = TimeZoneInfo.ConvertTimeFromUtc(uploadedTime, tz);

            // Calculation SLA Duration
            int duration = 0;
            while (revisedUploadedTime.Date <= currentTime.Date)
            {
                if (revisedUploadedTime.DayOfWeek != DayOfWeek.Saturday &&
                    revisedUploadedTime.DayOfWeek != DayOfWeek.Sunday)
                {
                    if (revisedUploadedTime.TimeOfDay > businessEndTime)
                    {
                        revisedUploadedTime = revisedUploadedTime.AddDays(1).Date + businessStartTime;
                        continue;
                    }

                    if (revisedUploadedTime.TimeOfDay < businessStartTime)
                    {
                        revisedUploadedTime = revisedUploadedTime.Date + businessStartTime;
                    }

                    int temp = 0;
                    if (revisedUploadedTime.Date < currentTime.Date ||
                        currentTime.TimeOfDay > businessEndTime)
                    {
                        temp = (int)businessEndTime.Subtract(revisedUploadedTime.TimeOfDay).TotalMinutes;
                    }
                    else
                    {
                        temp = (int)currentTime.TimeOfDay.Subtract(revisedUploadedTime.TimeOfDay).TotalMinutes;
                    }

                    if (temp > 0)
                    {
                        duration = duration + temp;
                    }
                }

                revisedUploadedTime = revisedUploadedTime.AddDays(1).Date + businessStartTime;
            }

            return duration;
        }

        #endregion

        #region Get Completed Inspections List

        public List<Usp_GetInspections> GetCompletedInspectionsList(AdminInspectionsRequest model, string userGuid, string companyGuid,
                                                                    out long recordsCount)
        {
            recordsCount = 0;

            var userGuidParam = new SqlParameter { ParameterName = "UserGuid", SqlDbType = SqlDbType.VarChar, Value = userGuid };
            var companyGuidParam = new SqlParameter { ParameterName = "CompanyGuid", SqlDbType = SqlDbType.VarChar, Value = companyGuid };

            var assetFilter = string.IsNullOrEmpty(model.AssetFilter) ? (object)DBNull.Value : model.AssetFilter;
            var assetFilterParam = new SqlParameter { ParameterName = "AssetFilter", SqlDbType = SqlDbType.VarChar, Value = assetFilter };

            var lenderFilter = string.IsNullOrEmpty(model.LenderFilter) ? (object)DBNull.Value : model.LenderFilter;
            var lenderFilterParam = new SqlParameter { ParameterName = "LenderFilter", SqlDbType = SqlDbType.VarChar, Value = lenderFilter };

            var completedStatus = (int)Enums.ApplicationStatus.Completed;
            var statusFilterParam = new SqlParameter { ParameterName = "StatusFilter", SqlDbType = SqlDbType.Int, Value = completedStatus };

            var isRequestedFromCompletedParam = new SqlParameter { ParameterName = "IsRequestedFromCompleted", SqlDbType = SqlDbType.Bit, Value = 1 };

            var filterText = string.IsNullOrEmpty(model.FilterText) ? (object)DBNull.Value : model.FilterText;
            var filterTextParam = new SqlParameter { ParameterName = "FilterText", SqlDbType = SqlDbType.VarChar, Value = filterText };

            var sortColumnParam = new SqlParameter { ParameterName = "SortColumn", SqlDbType = SqlDbType.VarChar, Value = model.SortColumn };
            var sortDirectionParam = new SqlParameter { ParameterName = "SortDirection", SqlDbType = SqlDbType.VarChar, Value = model.SortDirection };

            var skipDataParam = new SqlParameter { ParameterName = "SkipData", SqlDbType = SqlDbType.Int, Value = model.SkipData };
            var limitDataParam = new SqlParameter { ParameterName = "LimitData", SqlDbType = SqlDbType.Int, Value = model.LimitData };

            var isNewUserParam = new SqlParameter { ParameterName = "IsNewUser", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output };

            var result = dbContext.InspectionsList.FromSqlRaw<Usp_GetInspections>
                         (
                           "Usp_GetInspections @UserGuid, @CompanyGuid, @AssetFilter, @LenderFilter, @StatusFilter, @IsRequestedFromCompleted, @FilterText," +
                           "@SortColumn, @SortDirection, @SkipData, @LimitData, @IsNewUser OUTPUT",
                           userGuidParam, companyGuidParam, assetFilterParam, lenderFilterParam, statusFilterParam, isRequestedFromCompletedParam, filterTextParam,
                           sortColumnParam, sortDirectionParam, skipDataParam, limitDataParam, isNewUserParam
                         )
                         .ToList();

            if (result != null &&
                result.Count() > 0)
            {
                recordsCount = result.FirstOrDefault().TotalRecords;
            }

            return result;
        }

        #endregion

        #region Get Inspection Details By Id

        public Applications GetInspectionDetailsById(long inspectionId)
        {
            // Get Inspection Details By Id
            var response = dbContext.Applications.FirstOrDefault(a => a.Id == inspectionId);

            return response;
        }

        #endregion

        #region Suspend Inspection

        public void SuspendInspection(Applications application, long userId)
        {
            application.IsSuspended = true;
            application.UpdatedBy = userId;
            application.UpdatedTime = DateTime.UtcNow;

            dbContext.Entry(application).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

        #endregion

        #region Reactive Inspection

        public void ReactiveInspection(Applications application, long userId)
        {
            application.IsSuspended = false;
            application.UpdatedBy = userId;
            application.UpdatedTime = DateTime.UtcNow;

            // If application status is completed
            if (application.ApplicationStatus == (int)Enums.ApplicationStatus.Completed)
            {
                application.ApplicationStatus = (int)Enums.ApplicationStatus.Submitted;
            }

            dbContext.Entry(application).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

        #endregion

        #region Data Purge Process

        public void DataPurgeProcess(long inspectionId, long userId)
        {
            // Data Purge Process for AppImges
            var appImages = dbContext.AppImages.Where(ai => ai.ApplicationId == inspectionId).ToList();
            appImages.ForEach(
                ai =>
                {
                    ai.FileName = null;
                    ai.FilePath = null;
                    ai.Extension = null;
                    ai.SizeInKb = null;
                    ai.ImageData = null;
                    ai.UpdatedBy = userId;
                    ai.UpdatedTime = DateTime.UtcNow;
                    ai.ImageStatus = (int)Enums.DocImageStatus.Deleted;
                    dbContext.Entry(ai).State = EntityState.Modified;
                });

            dbContext.AppImages.UpdateRange(appImages);

            // Data Purge Process for Applications
            var application = dbContext.Applications.Where(a => a.Id == inspectionId).ToList();
            application.ForEach(
                a =>
                {
                    a.PurgeStatus = 1;
                    a.UpdatedBy = userId;
                    a.UpdatedTime = DateTime.UtcNow;
                    dbContext.Entry(a).State = EntityState.Modified;
                });

            dbContext.Applications.UpdateRange(application);

            // Data Purge Process for AppUsers
            var appUsers = dbContext.AppUsers.Where(au => au.ApplicationId == inspectionId).ToList();
            appUsers.ForEach(
                au =>
                {
                    au.Name = "";
                    au.SurName = ""; ;
                    au.Email = ""; ;
                    au.PhoneNumber = "";
                });

            dbContext.AppUsers.UpdateRange(appUsers);

            dbContext.SaveChanges();
        }

        #endregion

        #region Get Companies Dashboard Datas

        public Models.CompaniesDashboardResponse GetCompaniesDashboardDatas(GetCompanyDashboardDatasRequest model)
        {
            var response = new Models.CompaniesDashboardResponse();

            var lenderFilter = string.IsNullOrEmpty(model.LenderFilter) ? (object)DBNull.Value : model.LenderFilter;
            var lenderFilterParam = new SqlParameter { ParameterName = "LenderFilter", SqlDbType = SqlDbType.VarChar, Value = lenderFilter };

            var fromDateFilter = string.IsNullOrEmpty(model.FromDateFilter) ? (object)DBNull.Value : string.Join("-", model.FromDateFilter.Split('/').Reverse());
            var fromDateFilterParam = new SqlParameter { ParameterName = "FromDateFilter", SqlDbType = SqlDbType.Date, Value = fromDateFilter };

            var toDateFilter = string.IsNullOrEmpty(model.ToDateFilter) ? (object)DBNull.Value : string.Join("-", model.ToDateFilter.Split('/').Reverse());
            var toDateFilterParam = new SqlParameter { ParameterName = "ToDateFilter", SqlDbType = SqlDbType.Date, Value = toDateFilter };

            var result = dbContext.CompaniesDashboardDatas.FromSqlRaw<Usp_GetCompaniesDashboardDatas>
                         (
                           "Usp_GetCompaniesDashboardDatas @LenderFilter, @FromDateFilter, @ToDateFilter",
                           lenderFilterParam, fromDateFilterParam, toDateFilterParam
                         )
                         .ToList();

            // Inspection Status List
            var inspectionStatusList = new int[]
                                         {
                                             (int)Models.Enums.ApplicationStatus.Created,
                                             (int)Models.Enums.ApplicationStatus.Rejected,
                                             (int)Models.Enums.ApplicationStatus.Completed,
                                             (int)Models.Enums.ApplicationStatus.Cancelled
                                         }.ToList();

            // Inspection Status Description
            var createdStatus = Startup.AppConfiguration.GetSection("InspectionsStatus").GetSection("Created").Value;
            var rejectedStatus = Startup.AppConfiguration.GetSection("InspectionsStatus").GetSection("Rejected").Value;
            var completedStatus = Startup.AppConfiguration.GetSection("InspectionsStatus").GetSection("Completed").Value;
            var abandonedStatus = Startup.AppConfiguration.GetSection("InspectionsStatus").GetSection("Abandoned").Value;

            var inspectionDatasList = new List<InspectionDatas>();

            if (result != null &&
                result.Count() > 0)
            {
                var createdData = result.FirstOrDefault(i => i.InspectionsStatus == (int)Models.Enums.ApplicationStatus.Created);
                var rejectedData = result.FirstOrDefault(i => i.InspectionsStatus == (int)Models.Enums.ApplicationStatus.Rejected);
                var completedData = result.FirstOrDefault(i => i.InspectionsStatus == (int)Models.Enums.ApplicationStatus.Completed);
                var abandonedData = result.FirstOrDefault(i => i.InspectionsStatus == (int)Models.Enums.ApplicationStatus.Cancelled);

                inspectionStatusList.ForEach(
                    inspectionStatus =>
                    {
                        var statusDescription = "";
                        var inspectionsCount = 0;
                        var percentage = "";
                        var brokersInvolvedCount = 0;
                        var companiesInvolvedCount = 0;

                        switch (inspectionStatus)
                        {
                            // Inspections Created Status
                            case ((int)Models.Enums.ApplicationStatus.Created):
                                statusDescription = createdStatus;
                                inspectionsCount = createdData != null
                                                 ? createdData.InspectionsCount
                                                 : 0;
                                percentage = string.Join("", "100", "%");
                                brokersInvolvedCount = createdData != null
                                                     ? createdData.BrokersInvolved
                                                     : 0;
                                companiesInvolvedCount = createdData != null
                                                       ? createdData.CompaniesInvolved
                                                       : 0;
                                break;

                            // Inspections Rejected Status
                            case ((int)Models.Enums.ApplicationStatus.Rejected):
                                statusDescription = rejectedStatus;
                                inspectionsCount = rejectedData != null
                                                 ? rejectedData.InspectionsCount
                                                 : 0;
                                percentage = rejectedData != null
                                           ? string.Join("", Math.Floor((decimal.Parse(rejectedData.InspectionsCount.ToString()) /
                                                                         decimal.Parse(createdData.InspectionsCount.ToString())) * 100).ToString(), "%")
                                           : string.Join("", "0", "%");
                                brokersInvolvedCount = rejectedData != null
                                                     ? rejectedData.BrokersInvolved
                                                     : 0;
                                companiesInvolvedCount = rejectedData != null
                                                       ? rejectedData.CompaniesInvolved
                                                       : 0;
                                break;

                            // Inspections Completed Status
                            case ((int)Models.Enums.ApplicationStatus.Completed):
                                statusDescription = completedStatus;
                                inspectionsCount = completedData != null
                                                 ? completedData.InspectionsCount
                                                 : 0;
                                percentage = completedData != null
                                           ? string.Join("", Math.Floor((decimal.Parse(completedData.InspectionsCount.ToString()) /
                                                                         decimal.Parse(createdData.InspectionsCount.ToString())) * 100).ToString(), "%")
                                           : string.Join("", "0", "%");
                                brokersInvolvedCount = completedData != null
                                                     ? completedData.BrokersInvolved
                                                     : 0;
                                companiesInvolvedCount = completedData != null
                                                       ? completedData.CompaniesInvolved
                                                       : 0;
                                break;

                            // Inspections Cancelled Status
                            case ((int)Models.Enums.ApplicationStatus.Cancelled):
                                statusDescription = abandonedStatus;
                                inspectionsCount = abandonedData != null
                                                 ? abandonedData.InspectionsCount
                                                 : 0;
                                percentage = abandonedData != null
                                           ? string.Join("", Math.Floor((decimal.Parse(abandonedData.InspectionsCount.ToString()) /
                                                                         decimal.Parse(createdData.InspectionsCount.ToString())) * 100).ToString(), "%")
                                           : string.Join("", "0", "%");
                                brokersInvolvedCount = abandonedData != null
                                                     ? abandonedData.BrokersInvolved
                                                     : 0;
                                companiesInvolvedCount = abandonedData != null
                                                       ? abandonedData.CompaniesInvolved
                                                       : 0;
                                break;
                        }

                        inspectionDatasList.Add(
                            new InspectionDatas()
                            {
                                InspectionsStatus = statusDescription,
                                InspectionsCount = inspectionsCount,
                                Percentage = percentage,
                                BrokersInvolved = brokersInvolvedCount,
                                CompaniesInvolved = companiesInvolvedCount
                            });
                    });

                response.InspectionDatas.AddRange(inspectionDatasList);
                response.SellerResponseTime = result.FirstOrDefault().SellerResponseTime;
                response.ProcessingTime = result.FirstOrDefault().ProcessingTime;
            }
            else
            {
                inspectionStatusList.ForEach(
                    inspectionStatus =>
                    {
                        var statusDescription = "";
                        var inspectionsCount = 0;
                        var percentage = string.Join("", "0", "%");
                        var brokersInvolvedCount = 0;
                        var companiesInvolvedCount = 0;
                        switch (inspectionStatus)
                        {
                            // Inspections Created Status
                            case ((int)Models.Enums.ApplicationStatus.Created):
                                statusDescription = createdStatus;
                                break;

                            // Inspections Rejected Status
                            case ((int)Models.Enums.ApplicationStatus.Rejected):
                                statusDescription = rejectedStatus;
                                break;

                            // Inspections Completed Status
                            case ((int)Models.Enums.ApplicationStatus.Completed):
                                statusDescription = completedStatus;
                                break;

                            // Inspections Cancelled Status
                            case ((int)Models.Enums.ApplicationStatus.Cancelled):
                                statusDescription = abandonedStatus;
                                break;
                        }

                        inspectionDatasList.Add(
                            new InspectionDatas()
                            {
                                InspectionsStatus = statusDescription,
                                InspectionsCount = inspectionsCount,
                                Percentage = percentage,
                                BrokersInvolved = brokersInvolvedCount,
                                CompaniesInvolved = companiesInvolvedCount
                            });
                    });

                response.InspectionDatas.AddRange(inspectionDatasList);
                response.SellerResponseTime = "0h 0m";
                response.ProcessingTime = "0m";
            }

            return response;
        }

        private void AddInspectionsStatusDatas()
        {

        }

        #endregion
    }
}
