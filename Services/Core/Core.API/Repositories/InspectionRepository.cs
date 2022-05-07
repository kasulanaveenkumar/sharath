using Common.AzureBlobUtility.Helper;
using Common.AzureBlobUtility.Models;
using Common.Extensions;
using Core.API.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Common.Models.Core.Entities;
using Core.API.Entities.SP;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Core.API.Repositories
{
    public class InspectionRepository : IInspectionRepository
    {
        private readonly CoreContext dbContext;
        private readonly AzureBlobHelper azureBlobHelper;

        #region Constructor

        public InspectionRepository(CoreContext context)
        {
            dbContext = context;

            // Get AccountName value from appsettings.json File
            // Get AccountKey value from appsettings.json File
            // Get Container value from appsettings.json File
            var accountName = Startup.AppConfiguration.GetSection("AppSettings").GetSection("AzureStorageAccountName").Value;
            var accountKey = Startup.AppConfiguration.GetSection("AppSettings").GetSection("AzureStorageAccountKey").Value;
            var containerName = Startup.AppConfiguration.GetSection("AppSettings").GetSection("ContainerNameForAppImages").Value;
            azureBlobHelper = new AzureBlobHelper(accountName, accountKey, containerName);
        }

        #endregion

        public void BeginTransaction()
        {
            dbContext.Database.BeginTransaction();
        }

        public void CommitTransaction()
        {
            dbContext.Database.CommitTransaction();
        }

        public void RollbackTransaction()
        {
            dbContext.Database.RollbackTransaction();
        }

        #region Add Application

        public Applications AddApplication(Applications newApp)
        {
            dbContext.Applications.Add(newApp);
            dbContext.SaveChanges();

            return newApp;
        }

        #endregion

        #region Add Users

        public void AddAppUsers(List<AppUsers> appusers)
        {
            dbContext.AppUsers.AddRange(appusers.ToArray());
            dbContext.SaveChanges();
        }

        #endregion

        #region Add owner Stakeholder

        public void AddOwnerStakeholder(AppStakeholders ownerappStakeholder)
        {
            dbContext.AppStakeholders.Add(ownerappStakeholder);
            dbContext.SaveChanges();
        }

        #endregion

        #region Add StakeHolders

        public void AddStakeHolders(List<AppStakeholders> stakeholders)
        {
            dbContext.AppStakeholders.AddRange(stakeholders.ToArray());
            dbContext.SaveChanges();
        }

        #endregion

        #region Add App Activity Logs

        public void AddAppActivityLogs(AppActivityLogs appActivityLogs)
        {
            dbContext.AppActivityLogs.Add(appActivityLogs);
            dbContext.SaveChanges();
        }

        #endregion

        #region Get All Inspections List

        public List<Usp_GetInspections> GetAllInspectionsList(Models.BrokerInspectionsRequest model,
                                                              string userGuid, string companyGuid,
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

            var filterText = string.IsNullOrEmpty(model.FilterText)
                           ? (object)DBNull.Value
                           : model.FilterText;
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

        #region Get Completed Inspections List

        public List<Usp_GetInspections> GetCompletedInspectionsList(Models.BrokerInspectionsRequest model,
                                                                    string userGuid, string companyGuid,
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

            var completedStatus = (int)Models.Enums.ApplicationStatus.Completed;
            var statusFilterParam = new SqlParameter { ParameterName = "StatusFilter", SqlDbType = SqlDbType.Int, Value = completedStatus };

            var isRequestedFromCompletedParam = new SqlParameter { ParameterName = "IsRequestedFromCompleted", SqlDbType = SqlDbType.Bit, Value = 1 };

            var filterText = string.IsNullOrEmpty(model.FilterText)
                           ? (object)DBNull.Value
                           : model.FilterText;
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

        #region Get Lenders

        public List<Models.Lenders> GetLenders()
        {
            // Get Lender Companies
            var lenderCompanies = (from c in dbContext.ADCompanies
                                   where c.CompanyTypeId == (int)Common.Models.Enums.CompanyTypes.Lender
                                   orderby c.CompanyName
                                   select c).ToList();

            // Get Lenders
            var responses = (from c in lenderCompanies
                             join l in dbContext.LenderConfigurations
                             on c.CompanyGuid equals l.LenderCompanyGuid
                             into LendersDetails
                             from lender in LendersDetails.DefaultIfEmpty()
                             select new Models.Lenders()
                             {
                                 LenderGuid = c.CompanyGuid,
                                 LenderName = c.CompanyName,
                                 IsPayer = c.IsPayer,
                                 LenderPrefix = lender != null
                                              ? lender.LenderRefPrefix
                                              : "",
                                 IsAllowAwaitedRef = lender != null
                                                   ? lender.IsAllowAwaitedRef
                                                   : false,
                                 IsForceLenderRefFormat = lender != null
                                                        ? lender.IsForceLenderRefFormat
                                                        : false
                             }).ToList();

            return responses;
        }

        public List<Models.Lenders> GetLenders(string companyGuid)
        {
            var lenders = dbContext.ADCompanies
                          .Where(c => c.CompanyGuid == companyGuid &&
                                      c.CompanyTypeId == (int)Common.Models.Enums.CompanyTypes.Lender)
                          .OrderBy(c => c.CompanyName)
                          .ToList();

            // Get Lenders
            var responses = (from c in lenders
                             join l in dbContext.LenderConfigurations
                             on c.CompanyGuid equals l.LenderCompanyGuid
                             into LendersDetails
                             from lender in LendersDetails.DefaultIfEmpty()
                             select new Models.Lenders()
                             {
                                 LenderGuid = c.CompanyGuid,
                                 LenderName = c.CompanyName,
                                 IsPayer = c.IsPayer,
                                 LenderPrefix = lender != null
                                              ? lender.LenderRefPrefix
                                              : "",
                                 IsAllowAwaitedRef = lender != null
                                                   ? lender.IsAllowAwaitedRef
                                                   : false,
                                 IsForceLenderRefFormat = lender != null
                                                        ? lender.IsForceLenderRefFormat
                                                        : false,
                                 IsMapped = true
                             }).ToList();

            return responses;
        }

        public List<Models.Lenders> GetLenders(List<Models.Data.LenderWorkWithResponse> lenders)
        {
            // Get Lenders
            var responses = (from l in lenders
                             join lc in dbContext.LenderConfigurations
                             on l.LenderGUID equals lc.LenderCompanyGuid
                             into LendersDetails
                             from lender in LendersDetails.DefaultIfEmpty()
                             select new Models.Lenders()
                             {
                                 LenderGuid = l.LenderGUID,
                                 LenderName = l.LenderName,
                                 IsPayer = l.IsPayer,
                                 LenderPrefix = lender != null
                                              ? lender.LenderRefPrefix
                                              : "",
                                 IsAllowAwaitedRef = lender != null
                                                   ? lender.IsAllowAwaitedRef
                                                   : false,
                                 IsForceLenderRefFormat = lender != null
                                                        ? lender.IsForceLenderRefFormat
                                                        : false,
                                 IsMapped = l.IsMapped
                             })
                             .ToList();

            return responses;
        }

        #endregion

        #region Get All Lenders

        public List<Models.Lenders> GetAllLenders()
        {
            // Get All Lenders
            var responses = (from c in dbContext.ADCompanies
                             where c.CompanyTypeId == (int)Common.Models.Enums.CompanyTypes.Lender
                             select new Models.Lenders()
                             {
                                 LenderGuid = c.CompanyGuid,
                                 LenderName = c.CompanyName
                             }).ToList();

            return responses;
        }

        #endregion

        #region Get All Assets

        public List<Models.TemplateSets> GetAllAssets()
        {
            // Get All Assets
            var responses = (from t in dbContext.ADTemplateSets
                             orderby t.TemplateName
                             select new Models.TemplateSets()
                             {
                                 TemplateGuid = t.TemplateSetGUID,
                                 TemplateName = t.TemplateName
                             }).ToList();

            return responses;
        }

        #endregion

        #region Get Application Statuses

        public List<Models.ApplicationStatus> GetApplicationStatuses()
        {
            var applicationStatuses = new List<Models.ApplicationStatus>();

            // Get Application Statuses
            var responses = ExtensionMethods.GetEnumValuesAndDescriptions<Models.Enums.ApplicationStatus>().ToList();
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

        #region Get All States

        public List<Models.States> GetAllStates()
        {
            // Get All States
            var responses = (from t in dbContext.ADStates
                             orderby t.StateCode
                             select new Models.States()
                             {
                                 StateId = t.StateID,
                                 StateCode = t.StateCode
                             }).ToList();

            return responses;
        }

        #endregion

        #region Get Inspection Details

        public Models.InspectionDetails GetInspectionDetails(long inspectionId, List<Models.Lenders> lenders)
        {
            // Get Mapped Inspections
            var mappedInspections = (from a in dbContext.Applications
                                     join seller in dbContext.AppUsers
                                     on new { Id = a.Id, Role = (short)Models.Enums.Role.Seller }
                                     equals new { Id = seller.ApplicationId, Role = seller.Role }
                                     join buyer in dbContext.AppUsers
                                     on new { Id = a.Id, Role = (short)Models.Enums.Role.Buyer }
                                     equals new { Id = buyer.ApplicationId, Role = buyer.Role }
                                     where a.Id == inspectionId
                                     select new Models.InspectionDetails()
                                     {
                                         InspectionId = a.Id,
                                         Buyer = new Models.AppUser()
                                         {
                                             Name = buyer.Name,
                                             SurName = buyer.SurName,
                                             Email = buyer.Email,
                                             PhoneNumber = buyer.PhoneNumber
                                         },
                                         Seller = new Models.AppUser()
                                         {
                                             Name = seller.Name,
                                             SurName = seller.SurName,
                                             Email = seller.Email,
                                             PhoneNumber = seller.PhoneNumber
                                         },
                                         LenderGuid = a.LenderCompanyGuid,
                                         LenderReference = a.RefNumber,
                                         StateCode = a.StateCode,
                                         CreatedBy = a.CreatedBy,
                                         CreatedTime = a.CreatedTime,
                                         BrokerCompanyGuid = a.BrokerCompanyGuid,
                                         TemplateSetGuid = a.TemplateSetGuid,
                                         TemplateSetPlanGuid = a.TemplateSetPlanGuid,
                                         LastActivity = (a.UpdatedTime == null)
                                                      ? a.CreatedTime
                                                      : a.UpdatedTime,
                                         ApplicationStatus = a.ApplicationStatus,
                                         WebAppShortLink = a.WebAppShortLink,
                                         LastNotifiedTime = a.LastNotifiedTime,
                                         DVSStatus = a.DVSStatus.HasValue
                                                   ? a.DVSStatus
                                                   : null,
                                         DeviceDetails = a.DeviceDetails
                                     }).ToList();

            // Get Inspection Details
            var response = (from m in mappedInspections
                            join l in lenders
                            on m.LenderGuid equals l.LenderGuid
                            join ts in dbContext.ADTemplateSets
                            on m.TemplateSetGuid equals ts.TemplateSetGUID
                            join b in dbContext.ADCompanies
                            .Where(c => c.CompanyTypeId == (int)Common.Models.Enums.CompanyTypes.Broker)
                            on m.BrokerCompanyGuid equals b.CompanyGuid
                            join s in dbContext.ADStates
                            on m.StateCode equals s.StateCode
                            join u in dbContext.ADUsers
                            on m.CreatedBy equals u.UserId
                            into InspectionDatas
                            from i in InspectionDatas.DefaultIfEmpty()
                            select new Models.InspectionDetails()
                            {
                                InspectionId = m.InspectionId,
                                Buyer = m.Buyer,
                                Seller = m.Seller,
                                LenderGuid = m.LenderGuid,
                                LenderName = l.LenderName,
                                LenderReference = m.LenderReference,
                                AssetType = ts.TemplateName,
                                CompanyName = b.CompanyName,
                                StateId = s.StateID,
                                StateCode = s.StateCode,
                                CreatedBy = m.CreatedBy,
                                CreatedUser = i != null
                                            ? string.Join(" ", i.Name, i.SurName)
                                            : "",
                                CreatedTime = m.CreatedTime,
                                TemplateSetGuid = m.TemplateSetGuid,
                                TemplateSetPlanGuid = m.TemplateSetPlanGuid,
                                LastActivity = m.LastActivity,
                                BrokerEmail = i != null
                                            ? i.Email
                                            : "",
                                ApplicationStatus = m.ApplicationStatus,
                                LenderPrefix = l.LenderPrefix,
                                IsAllowAwaitedRef = l.IsAllowAwaitedRef,
                                IsForceLenderRefFormat = l.IsForceLenderRefFormat,
                                WebAppShortLink = m.WebAppShortLink,
                                LastNotifiedTime = m.LastNotifiedTime,
                                DVSStatus = m.DVSStatus,
                                DeviceDetails = m.DeviceDetails
                            }).FirstOrDefault();

            return response;
        }

        public Applications GetInspectionDetails(long inspectionId)
        {
            // Get Inspection Details
            var response = (from a in dbContext.Applications
                            where a.Id == inspectionId
                            select a).FirstOrDefault();

            return response;
        }

        public Applications GetInspectionDetails(string inspectionGuid)
        {
            // Get Inspection Details
            var response = (from a in dbContext.Applications
                            where a.InspectionGuid == inspectionGuid
                            select a).FirstOrDefault();

            return response;
        }

        public List<AppStakeholders> GetInspectionDetails(long inspectionId, string userGuid)
        {
            // Get Inspection Details
            var responses = (from ash in dbContext.AppStakeholders
                             where ash.ApplicationId == inspectionId && ash.UserGuid == userGuid
                             select ash).ToList();

            return responses;
        }


        public int GetUserMappedInspectionDetailsCount(string userGuid, string companyGuid)
        {
            // Get User Mapped Inspection Details Count
            var response = (from a in dbContext.Applications
                            join ash in dbContext.AppStakeholders
                            on a.Id equals ash.ApplicationId
                            where a.BrokerCompanyGuid == companyGuid &&
                            ash.UserGuid == userGuid
                            select a.Id).Count();


            return response;
        }

        #endregion

        #region Get Activity Logs

        public List<Models.ActivityLogs> GetActivityLogs(long inspectionId)
        {
            // Get Mapped Activities
            var responses = (from al in dbContext.AppActivityLogs
                             join a in dbContext.AppActivities
                             on al.AppActivityId equals a.Id
                             where al.ApplicationId == inspectionId
                             select new Models.ActivityLogs()
                             {
                                 UserType = al.UserType,
                                 UserGuid = al.UserGuid,
                                 Action = a.Description,
                                 ProcessedTime = al.ProcessedTime,
                                 IsWebAppUser = al.IsWebAppUser
                             }).ToList();

            // Get User Types
            var userTypes = (from ut in dbContext.ADUserTypes
                             select ut).ToList();

            // Get Seller Details mapped to Applications
            var sellerDetails = (from au in dbContext.AppUsers
                                 where au.ApplicationId == inspectionId &&
                                       au.Role == (int)Models.Enums.Role.Seller
                                 select au).FirstOrDefault();

            var userGuids = responses.Select(u => u.UserGuid).ToList();

            // Get Users by UserGuid
            var users = (from u in dbContext.ADUsers
                         where userGuids.Contains(u.UserGuid)
                         select u).ToList();

            responses.ForEach(
                a =>
                {
                    if (a.IsWebAppUser)
                    {
                        a.Role = Common.Models.Enums.UserTypes.Seller.ToString();

                        // Checking whether Seller Details exist
                        if (sellerDetails != null)
                        {
                            a.Name = sellerDetails.Name;
                            a.Surname = sellerDetails.SurName;
                        }
                    }
                    else
                    {
                        var userType = userTypes.FirstOrDefault(ut => ut.UserTypeId == a.UserType);
                        a.Role = userType != null
                               ? userType.Name
                               : "";

                        // Checking whether User Details exist
                        var userDetails = users.FirstOrDefault(u => u.UserGuid == a.UserGuid);
                        if (userDetails != null)
                        {
                            a.Name = userDetails.Name;
                            a.Surname = userDetails.SurName;
                        }
                    }
                });

            return responses;
        }

        #endregion

        #region Get CSV Download String

        public string GetCSVFileDownloadString(long inspectionId)
        {
            // Get Activities Mapped to Inspection
            var activityLogs = GetActivityLogs(inspectionId);

            // Get Download Logs
            var downloadLogs = activityLogs.Select(al =>
                                    new Models.DownloadActivityLogs()
                                    {
                                        Role = al.Role,
                                        Name = al.Name,
                                        Surname = al.Surname,
                                        Action = al.Action,
                                        ProcessedTime = al.ProcessedTime.ToString("dd/MM/yyyy - HH:mm")
                                    });

            var csvDatas = JsonSerializer.Serialize(downloadLogs);

            var response = csvDatas.JsonToCsv();

            return response;
        }

        #endregion

        #region Get Inspection Progress

        public List<Models.InspectionProgress> GetInspectionProgress(long inspectionId)
        {
            // Get AppActivites
            var appActivities = dbContext.AppActivities
                                .Where(a => a.Id <= (int)Models.Enums.ApplicationStatus.Completed)
                                .Select(a => new AppActivities()
                                {
                                    Id = a.Id,
                                    Description = a.Description
                                })
                                .ToList();

            // Get AppActivityLogs By InspectionId
            var activityLogs = dbContext.AppActivityLogs
                               .Where(a => a.ApplicationId == inspectionId)
                               .GroupBy(al => new { al.AppActivityId })
                               .Select(g => new AppActivityLogs()
                               {
                                   AppActivityId = g.Key.AppActivityId,
                                   ProcessedTime = g.Max(al => al.ProcessedTime)
                               })
                               .ToList();

            // Get Inspection Progresses
            var responses = (from a in appActivities
                             join al in activityLogs
                             on a.Id equals al.AppActivityId
                             into InspectionProgresses
                             from ip in InspectionProgresses.DefaultIfEmpty()
                             select new Models.InspectionProgress()
                             {
                                 StatusId = a.Id,
                                 Status = a.Description,
                                 IsProgressed = ip != null
                                              ? true
                                              : false,
                                 ProcessedTime = ip != null
                                               ? ip.ProcessedTime
                                               : null
                             })
                             .OrderBy(i => i.StatusId)
                             .ThenBy(i => i.ProcessedTime)
                             .ToList();

            // Rename Rejected Status to Processed
            var rejectedDetails = responses.FirstOrDefault(ip => ip.StatusId == (int)Models.Enums.ApplicationStatus.Rejected);
            if (rejectedDetails != null)
            {
                rejectedDetails.Status = "Inspection Processed";
            }

            // Assign Completed Status Processed Time to Rejected Status Processed Time
            var completedDetails = responses.FirstOrDefault(ip => ip.StatusId == (int)Models.Enums.ApplicationStatus.Completed);
            if (completedDetails != null)
            {
                // Checking whether Processed Time exist
                if (completedDetails.ProcessedTime != null)
                {
                    rejectedDetails.ProcessedTime = completedDetails.ProcessedTime;
                }
            }

            return responses;
        }

        #endregion

        #region Get Active Reminders

        public List<Models.Reminders> GetActiveReminders()
        {
            // Get Active Reminders
            var responses = (from r in dbContext.Remainders
                             where r.IsActive == true
                             select new Models.Reminders()
                             {
                                 Id = r.Id,
                                 RemainderGuid = r.RemainderGuid,
                                 RemainderTemplate = r.RemainderTemplate,
                                 IsActive = r.IsActive
                             }).ToList();

            return responses;
        }

        #endregion

        #region Get App Documents

        public List<AppDocuments> GetAppDocuments(long inspectionId)
        {
            // Get App Documents
            var responses = (from d in dbContext.AppDocuments
                             where d.ApplicationId == inspectionId
                             select d).ToList();

            return responses;
        }

        #endregion

        #region Get Template Documents List

        public List<Models.TemplateDocument> GetTemplateDocumentsList(long inspectionId)
        {
            var responses = new List<Models.TemplateDocument>();

            // Get App Documents
            var appDocuments = GetAppDocuments(inspectionId);

            // Get App Images
            var appImages = (from i in dbContext.AppImages
                             where i.ApplicationId == inspectionId
                             select i).ToList();

            short docIndex = 1;

            appDocuments.ForEach(
                document =>
                {
                    var appImageList = new List<Models.TemplateImage>();

                    // Get Mapped App Images
                    var mappedAppImages = (from appImage in appImages
                                           where appImage.AppDocumentId == document.Id
                                           select appImage).ToList();
                    short imageIndex = 1;

                    mappedAppImages.ForEach(
                        i =>
                        {
                            // Creating App Image List
                            appImageList.Add(new Models.TemplateImage()
                            {
                                ImageName = i.ImageName,
                                DocGroup = i.DocGroup,
                                ImageType = i.ImageType,
                                Position = imageIndex++,
                                IsMandatory = true,
                                IsDefaultSelected = true
                            });
                        });

                    responses.Add(
                        new Models.TemplateDocument()
                        {
                            DocumentName = document.Name,
                            ImageDetails = appImageList,
                            Position = docIndex++
                        });
                });

            return responses;
        }

        #endregion

        #region Get Inspection Documents List

        public List<Models.AppDocuments> GetInspectionDocumentsList(long inspectionId, LenderConfigurations lenderConfig)
        {
            var responses = new List<Models.AppDocuments>();

            // Get App Documents
            var appDocuments = GetAppDocuments(inspectionId);

            // Get App Images
            var appImages = (from i in dbContext.AppImages
                             where i.ApplicationId == inspectionId
                             select i).ToList();

            // Get Mapped Reasons
            var mappedReasons = (from r in dbContext.Reasons
                                 join rm in dbContext.ReasonMappings
                                 on r.Id equals rm.ReasonId
                                 select new
                                 {
                                     ReasonId = r.Id,
                                     ImageType = rm.ImageType,
                                     Description = r.Description,
                                     ReasonType = rm.ReasonType
                                 }).ToList();

            // Get App Image Reasons
            var appImageReasons = (from air in dbContext.AppImageReasons
                                   where air.ApplicationId == inspectionId
                                   select air).ToList();

            appDocuments.ForEach(
                document =>
                {
                    var appImageList = new List<Models.AppImages>();

                    // Get Mapped App Images
                    var mappedAppImages = appImages.Where(ai => ai.AppDocumentId == document.Id)
                                          .OrderBy(ai => ai.Id).ToList();

                    mappedAppImages.ForEach(
                        i =>
                        {
                            // Get Rejected Reasons
                            var rejectedReasonsList = (from mr in mappedReasons.Where(mr => mr.ImageType == i.ImageType &&
                                                                                      mr.ReasonType == (int)Models.Enums.ReasonTypes.RejectReason)
                                                       join air in appImageReasons.Where(air => air.AppImageId == i.Id &&
                                                                                                air.ReasonType == (int)Models.Enums.ReasonTypes.RejectReason)
                                                       on new { ImageType = mr.ImageType, ReasonId = mr.ReasonId }
                                                       equals new { ImageType = i.ImageType, ReasonId = air.ReasonId }
                                                       into rejectedReasons
                                                       from r in rejectedReasons.DefaultIfEmpty()
                                                       select new Models.AppImageSelectedReasons()
                                                       {
                                                           Description = mr.Description,
                                                           ReasonId = mr.ReasonId,
                                                           IsSelected = rejectedReasons.Count() > 0 ? true : false
                                                       }).ToList();

                            // Add Other Rejected Reason
                            if (!string.IsNullOrEmpty(i.OtherRejectReason))
                            {
                                rejectedReasonsList.Add(
                                    new Models.AppImageSelectedReasons()
                                    {
                                        Description = i.OtherRejectReason,
                                        ReasonId = 0,
                                        IsSelected = true
                                    });
                            }
                            else
                            {
                                rejectedReasonsList.Add(
                                    new Models.AppImageSelectedReasons()
                                    {
                                        Description = "",
                                        ReasonId = 0,
                                        IsSelected = false
                                    });
                            }

                            // Get Flag Reasons
                            var flagReasonsList = (from mr in mappedReasons.Where(mr => mr.ImageType == i.ImageType &&
                                                                                        mr.ReasonType == (int)Models.Enums.ReasonTypes.FlagReason)
                                                   join air in appImageReasons.Where(air => air.AppImageId == i.Id &&
                                                                                            air.ReasonType == (int)Models.Enums.ReasonTypes.FlagReason)
                                                   on new { ImageType = mr.ImageType, ReasonId = mr.ReasonId }
                                                   equals new { ImageType = i.ImageType, ReasonId = air.ReasonId }
                                                   into flagReasons
                                                   from r in flagReasons.DefaultIfEmpty()
                                                   select new Models.AppImageSelectedReasons()
                                                   {
                                                       Description = mr.Description,
                                                       ReasonId = mr.ReasonId,
                                                       IsSelected = flagReasons.Count() > 0
                                                                  ? true
                                                                  : false
                                                   }).ToList();

                            // Add Other Flag Reason
                            if (!string.IsNullOrEmpty(i.OtherFlagReason))
                            {
                                flagReasonsList.Add(
                                    new Models.AppImageSelectedReasons()
                                    {

                                        Description = i.OtherFlagReason,
                                        ReasonId = 0,
                                        IsSelected = true
                                    });
                            }
                            else
                            {
                                flagReasonsList.Add(
                                    new Models.AppImageSelectedReasons()
                                    {
                                        Description = "",
                                        ReasonId = 0,
                                        IsSelected = false
                                    });
                            }

                            if (CanAddImageToList(i, lenderConfig))
                            {
                                // Creating App Image List
                                appImageList.Add(
                                    new Models.AppImages()
                                    {
                                        ImageId = i.Id,
                                        ImageName = i.ImageName,
                                        ImageDescription = i.ImageName,
                                        DocGroup = i.DocGroup,
                                        ImageType = i.ImageType,
                                        InternalStatus = i.ImageInternalStatus,
                                        ImageStatus = i.ImageStatus,
                                        IsAccepted = (i.ImageStatus == (int)Models.Enums.DocImageStatus.Completed),
                                        IsRejected = (i.IsBypassRequested == false &&
                                                      i.ImageStatus == (int)Models.Enums.DocImageStatus.Rejected),
                                        IsImageRejected = (i.ImageStatus == (int)Models.Enums.DocImageStatus.Rejected),
                                        FlagReasons = flagReasonsList,
                                        RejectReasons = rejectedReasonsList,
                                        IsBypassRequested = i.IsBypassRequested,
                                        BypassReason = i.BypassReason,
                                        ProcessedTime = i.UpdatedTime,
                                        ImageData = i.ImageData,
                                        IsSkipped = i.IsSkipped,
                                        Extension = i.Extension
                                    });
                            }
                        });

                    // Check Rego Paper
                    if (appImageList.Count(i => (i.ImageType == (int)Models.Enums.ImageTypes.RegoPaperFront ||
                                                 i.ImageType == (int)Models.Enums.ImageTypes.RegoPaperBack)) == 1)
                    {
                        var rego = appImageList.Where(i => i.ImageType == (int)Models.Enums.ImageTypes.RegoPaperFront).FirstOrDefault();
                        if (rego != null)
                            rego.ImageName = rego.DocGroup == (int)Models.Enums.DocGroup.TrailerRego
                                           ? "Trailer Rego Paper"
                                           : "Rego Paper";
                    }

                    // Check images for rego paper vs renewal notice document
                    if (appImageList.Count(x => x.DocGroup == (int)Models.Enums.DocGroup.VehicleRegoPapers) > 0)
                    {
                        if (appImageList.Where(x => x.DocGroup == (int)Models.Enums.DocGroup.VehicleRegoPapers).ToList()
                                              .All(x => x.IsSkipped == false && x.Extension == null))
                        {
                            appImageList.RemoveAll(x => x.ImageType == (int)Models.Enums.ImageTypes.RenewalNotice ||
                                                                x.ImageType == (int)Models.Enums.ImageTypes.RenewalNotice_1);
                        }
                        else
                        {
                            appImageList.RemoveAll(x => (x.ImageType == (int)Models.Enums.ImageTypes.RenewalNotice ||
                                                                 x.ImageType == (int)Models.Enums.ImageTypes.RenewalNotice_1 ||
                                                                 x.ImageType == (int)Models.Enums.ImageTypes.RegoPaperBack ||
                                                                 x.ImageType == (int)Models.Enums.ImageTypes.RegoPaperFront) &&
                                                                 x.IsSkipped == false && x.Extension == null);
                        }
                    }

                    // Check images for bank statement vs payout letter document
                    if (appImageList.Count(x => x.DocGroup == (int)Models.Enums.DocGroup.BankStatements) > 0)
                    {
                        if (appImageList.Where(x => x.DocGroup == (int)Models.Enums.DocGroup.BankStatements).ToList()
                                              .All(x => x.IsSkipped == false && x.Extension == null))
                        {
                            appImageList.RemoveAll(x => x.ImageType == (int)Models.Enums.ImageTypes.PayoutLetter_BankStatement_1 ||
                                                                x.ImageType == (int)Models.Enums.ImageTypes.PayoutLetter_BankStatement_2);
                        }
                        else
                        {
                            appImageList.RemoveAll(x => (x.ImageType == (int)Models.Enums.ImageTypes.PayoutLetter_BankStatement_1 ||
                                                                 x.ImageType == (int)Models.Enums.ImageTypes.PayoutLetter_BankStatement_2 ||
                                                                 x.ImageType == (int)Models.Enums.ImageTypes.BankStatements) &&
                                                                 x.IsSkipped == false && x.Extension == null);
                        }
                    }

                    // Check images for roadworthycertificate
                    if (appImageList.Count(x => x.DocGroup == (int)Models.Enums.DocGroup.RoadworthyCertificate) > 0)
                    {
                        if (appImageList.Where(x => x.ImageType == (int)Models.Enums.ImageTypes.Gas_Certificate).ToList()
                                              .All(x => x.IsSkipped == false && x.Extension == null))
                        {
                            appImageList.RemoveAll(x => x.ImageType == (int)Models.Enums.ImageTypes.Gas_Certificate &&
                                                                x.IsSkipped == false && x.Extension == null);
                        }
                    }

                    if (appImageList.Count() > 0)
                    {
                        responses.Add(
                            new Models.AppDocuments()
                            {
                                DocId = document.Id,
                                DocumentName = document.Name,
                                ImageDetails = appImageList,
                                DocStatus = (Models.Enums.DocImageStatus)Enum.ToObject(typeof(Models.Enums.DocImageStatus), document.DocStatus),
                                IsShowDocument = appImageList.Count(i => i.ImageType == (int)Models.Enums.ImageTypes.BankStatements &&
                                                                         i.Extension == ".pdf") == 0
                            });
                    }
                });

            return responses;
        }

        private bool CanAddImageToList(AppImages img, LenderConfigurations lenderConfig)
        {
            // Regoback is available only for QLD & VIC
            if (img.ImageType == (int)Models.Enums.ImageTypes.RegoPaperBack &&
                img.Extension == null)
                return false;


            if (img.ImageType == (int)Models.Enums.ImageTypes.BankStatements ||
                img.ImageType == (int)Models.Enums.ImageTypes.PayoutLetter_BankStatement_1 ||
                img.ImageType == (int)Models.Enums.ImageTypes.PayoutLetter_BankStatement_2)
            {
                if (lenderConfig != null &&
                    !lenderConfig.AllowViewBS)
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region Get Broker Users

        public List<Models.BrokerUsers> GetBrokerUsers(string userGuid, string companyGuid, long inspectionId)
        {
            // Get Active Users
            var activeUsers = (from u in dbContext.ADUsers
                               where u.CompanyGuid == companyGuid &&
                                     u.UserGuid != userGuid
                               select new Models.BrokerUsers()
                               {
                                   Name = u.Name,
                                   SurName = u.SurName,
                                   Email = u.Email,
                                   Mobile = u.Mobile,
                                   UserGuid = u.UserGuid
                               }).ToList();

            // Get Broker Users
            var responses = (from u in activeUsers
                             join ash in dbContext.AppStakeholders
                             on new { Id = inspectionId, Guid = u.UserGuid }
                             equals new { Id = ash.ApplicationId, Guid = ash.UserGuid }
                             into usersList
                             from ul in usersList.DefaultIfEmpty()
                             select new Models.BrokerUsers()
                             {
                                 Name = !string.IsNullOrEmpty(u.Name)
                                      ? u.Name
                                      : "",
                                 SurName = !string.IsNullOrEmpty(u.SurName)
                                         ? u.SurName
                                         : "",
                                 Email = u.Email,
                                 Mobile = u.Mobile,
                                 UserGuid = u.UserGuid,
                                 IsSelected = ul != null 
                                            ? true 
                                            : false
                             }).ToList();

            return responses;
        }

        #endregion

        #region Get Template Plans

        public List<Models.InspectionPlansResponse> GetTemplatePlans(Models.InspectionPlansRequest model, string token)
        {
            var response = new List<Models.InspectionPlansResponse>();

            // Get Template Plans
            using (var client = new HttpClient())
            {
                var requestUri = "api/v1/Inspection/templateplans";
                var configApiUrl = Startup.AppConfiguration.GetSection("BaseURL").GetSection("ConfigApiURL").Value;
                response = ExtensionMethods<Models.InspectionPlansResponse>.GetSerializedDatas(client, configApiUrl, requestUri, token, model).Result;
            }

            return response;
        }

        #endregion

        #region Add App Documents and Images

        public void AddAppDocumentsImages(List<Models.TemplateDocument> templateDocuments, long applicationId)
        {
            templateDocuments.OrderBy(d => d.Position).ToList().ForEach(
                doc =>
                {
                    var newDoc = new AppDocuments();
                    newDoc.ApplicationId = applicationId;
                    newDoc.Name = doc.DocumentName;
                    newDoc.UpdatedDate = DateTime.UtcNow;
                    newDoc.DocStatus = (int)Models.Enums.DocStatus.Pending;
                    newDoc.IsAdditionalDataMandatory = doc.IsAdditionalDataMandatory;
                    newDoc.IsAdditionalDataRequired = doc.IsAdditionalDataRequired;
                    dbContext.AppDocuments.Add(newDoc);
                    dbContext.SaveChanges();

                    var images = templateDocuments.Where(d => d.DocumentName == doc.DocumentName).Select(d => d.ImageDetails).FirstOrDefault();
                    images.OrderBy(d => d.Position).ToList().ForEach(
                        img =>
                        {
                            dbContext.AppImages.Add(
                                new AppImages()
                                {
                                    ApplicationId = applicationId,
                                    AppDocumentId = newDoc.Id,
                                    ImageName = img.ImageName,
                                    DocGroup = img.DocGroup,
                                    ImageType = img.ImageType,
                                    ImageStatus = (int)Models.Enums.DocImageStatus.Pending,
                                    ImageInternalStatus = (int)Models.Enums.DocInternalStatus.Pending,
                                    IsSkippable = img.IsMandatory
                                });
                            dbContext.SaveChanges();
                        });
                });
        }

        #endregion

        #region Update Template Or Plan

        public void UpdateTemplateOrPlan(List<Models.TemplateDocument> templateDocuments, long applicationId)
        {
            // Remove App Documents mapped to Application Id
            var appDocuments = dbContext.AppDocuments.Where(d => d.ApplicationId == applicationId).ToList();
            dbContext.AppDocuments.RemoveRange(appDocuments);

            // Remove App Images mapped to Application Id
            var appImages = dbContext.AppImages.Where(i => i.ApplicationId == applicationId).ToList();
            dbContext.AppImages.RemoveRange(appImages);

            // Add App Documents and Images for Application Id
            AddAppDocumentsImages(templateDocuments, applicationId);
        }

        #endregion

        #region Get Inspection Statuses

        public List<Models.ApplicationStatus> GetInspectionStatuses(bool isSupportTeam = false)
        {
            var responses = new List<Models.ApplicationStatus>();

            if (isSupportTeam)
            {
                // Adding Default Item
                responses.Add(
                    new Models.ApplicationStatus()
                    {
                        StatusId = -1,
                        Description = "All"
                    });
            }

            // Adding Inspection Statuses
            var appStatusList = ExtensionMethods.GetEnumValuesAndDescriptions<Models.Enums.ApplicationStatus>().ToList();
            appStatusList.ForEach(
                status =>
                {
                    responses.Add(
                        new Models.ApplicationStatus()
                        {
                            StatusId = status.Value,
                            Description = status.Key
                        });
                });

            return responses;
        }

        #endregion

        #region Get Buyer List for Update

        public List<AppUsers> GetBuyerListForUpdate(List<AppUsers> appUsers, Models.AppUser model)
        {
            var responses = new List<AppUsers>();

            // Get Buyer Details
            var buyerDetails = appUsers.Where(au => au.Role == (int)Models.Enums.Role.Buyer).FirstOrDefault();
            if (buyerDetails != null)
            {
                // Add Buyer Details for update
                buyerDetails.Name = model.Name;
                buyerDetails.SurName = model.SurName;
                buyerDetails.Email = model.Email;
                buyerDetails.PhoneNumber = model.PhoneNumber;
                responses.Add(buyerDetails);
            }

            return responses;
        }

        #endregion

        #region Get Seller List for Update

        public List<AppUsers> GetSellerListForUpdate(List<AppUsers> appUsers, Models.AppUser model)
        {
            var responses = new List<AppUsers>();

            // Get Seller Details
            var sellerDetails = appUsers.Where(au => au.Role == (int)Models.Enums.Role.Seller).FirstOrDefault();
            if (sellerDetails != null)
            {
                // Add Seller Details for update
                sellerDetails.Name = model.Name;
                sellerDetails.SurName = model.SurName;
                sellerDetails.Email = model.Email;
                sellerDetails.PhoneNumber = model.PhoneNumber;
                responses.Add(sellerDetails);
            }

            return responses;
        }

        #endregion

        #region Update App Users

        public void UpdateAppUsers(List<AppUsers> appUsers)
        {
            dbContext.AppUsers.UpdateRange(appUsers);
            dbContext.SaveChanges();
        }

        #endregion

        #region Get App Users

        public List<AppUsers> GetAppUsers(long applicationId)
        {
            // Get App Users
            var responses = (from au in dbContext.AppUsers
                             where au.ApplicationId == applicationId
                             select au).ToList();

            return responses;
        }

        #endregion

        #region Update Inspection

        public void UpdateInspection(Applications application, Models.EditInspectionRequest model, long userId)
        {
            application.RefNumber = model.LenderRef;
            application.TemplateSetPlanGuid = model.TemplateSetPlanGuid;
            application.UpdatedBy = userId;
            application.UpdatedTime = DateTime.UtcNow;

            dbContext.Entry(application).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

        public void UpdateInspection(Applications application, long userId, bool isInspectionRejected = false)
        {
            if (isInspectionRejected)
            {
                application.RejectionCount = application.RejectionCount + 1;
            }
            application.UpdatedBy = userId;
            application.UpdatedTime = DateTime.UtcNow;

            dbContext.Entry(application).State = EntityState.Modified;
        }

        #endregion

        #region Save Shared Users

        public void SaveSharedUsers(List<string> usersToShare, long inspectionId)
        {
            // Get Users List
            var responses = (from ash in dbContext.AppStakeholders
                             where ash.ApplicationId == inspectionId
                             select ash).ToList();

            // Get New Users
            var newUsers = usersToShare.Except(responses.Select(u => u.UserGuid).ToList()).ToList();
            if (newUsers != null &&
                newUsers.Count() > 0)
            {
                newUsers.ForEach(
                    userGuid =>
                    {
                        // Get User Detail by Guid
                        var userToInsert = responses.FirstOrDefault(u => u.UserGuid == userGuid);
                        if (userToInsert == null)
                        {
                            // Add New User Details
                            var newUser = new AppStakeholders();
                            newUser.ApplicationId = inspectionId;
                            newUser.IsOwner = 0;
                            newUser.UserGuid = userGuid;
                            dbContext.AppStakeholders.Add(newUser);
                            dbContext.SaveChanges();
                        }
                    });
            }

            // Get Existing Users
            var existingUsers = responses.Select(a => a.UserGuid).Except(usersToShare.ToList()).ToList();

            // Checking whether Existing Users exist
            if (existingUsers != null &&
                existingUsers.Count() > 0)
            {
                existingUsers.ForEach(
                    userGuid =>
                    {
                        // Get User Detail by Guid
                        var userToDelete = responses.FirstOrDefault(u => u.UserGuid == userGuid);

                        // Checking whether User Detail exist
                        if (userToDelete != null)
                        {
                            if (userToDelete.IsOwner == 0)
                            {
                                // Remove Existing User Details
                                dbContext.AppStakeholders.Remove(userToDelete);
                                dbContext.SaveChanges();
                            }
                        }
                    });
            }
        }

        #endregion

        #region Get Core Configs Value

        public List<Models.CoreConfigsResponse> GetCoreConfigsValue(string name)
        {
            var responses = new List<Models.CoreConfigsResponse>();

            // Get Core Configs Value
            var coreConfigsDetails = (from c in dbContext.CoreConfigs
                                      where c.Name == name
                                      select new Models.CoreConfigsResponse()
                                      {
                                          Value = c.Value
                                      }).FirstOrDefault();
            if (coreConfigsDetails != null)
            {
                responses = coreConfigsDetails.Value.Split('~').ToList()
                            .Select(configValue =>
                            new Models.CoreConfigsResponse()
                            {
                                Value = configValue
                            }).ToList();
            }

            return responses;
        }

        #endregion

        #region Get Reminder Datas By Inspection Id

        public Models.ReminderResponse GetReminderDatasByInspectionId(long inspectionId)
        {
            var response = new Models.ReminderResponse();

            // Get Inspection Details
            var lenders = GetLenders();
            var inspectionDetails = GetInspectionDetails(inspectionId, lenders);

            // Get App Documents
            var appDocuments = GetAppDocuments(inspectionId);

            response.Buyer = inspectionDetails.Buyer;
            response.Seller = inspectionDetails.Seller;
            response.LenderName = inspectionDetails.LenderName;
            response.CompanyName = inspectionDetails.CompanyName;
            response.BrokerName = inspectionDetails.CreatedUser;
            response.BrokerEmail = inspectionDetails.BrokerEmail;
            response.AssetType = inspectionDetails.AssetType;
            response.InspectionId = inspectionDetails.InspectionId;
            response.InspectionStatus = ((Models.Enums.ApplicationStatus)inspectionDetails.ApplicationStatus).ToString();
            response.PendingDocuments = (from d in appDocuments
                                         where d.DocStatus == (int)Models.Enums.DocStatus.Pending
                                         select d.Name).ToList();
            response.RejectedDocuments = (from d in appDocuments
                                          where d.DocStatus == (int)Models.Enums.DocStatus.Rejected
                                          select d.Name).ToList();
            response.CreatedTime = inspectionDetails.CreatedTime;
            response.WebAppShortLink = inspectionDetails.WebAppShortLink;

            return response;
        }

        #endregion

        #region Get Reminder Message

        public string GetReminderMessage(Models.ReminderResponse reminderDatas, List<Models.CoreConfigsResponse> coreConfigValues, string message, long inspectionId)
        {
            var sbMessage = new StringBuilder(message);

            // Get Reminder Message
            if (reminderDatas.Seller != null &&
                !string.IsNullOrEmpty(reminderDatas.Seller.Name) &&
                !string.IsNullOrEmpty(reminderDatas.Seller.SurName))
            {
                sbMessage.Replace(coreConfigValues[0].Value, string.Join(" ", reminderDatas.Seller.Name, reminderDatas.Seller.SurName));
            }
            if (reminderDatas.Buyer != null &&
                !string.IsNullOrEmpty(reminderDatas.Buyer.Name) &&
                !string.IsNullOrEmpty(reminderDatas.Buyer.SurName))
            {
                sbMessage.Replace(coreConfigValues[1].Value, string.Join(" ", reminderDatas.Buyer.Name, reminderDatas.Buyer.SurName));
            }
            if (!string.IsNullOrEmpty(reminderDatas.CompanyName))
            {
                sbMessage.Replace(coreConfigValues[2].Value, reminderDatas.CompanyName);
            }
            sbMessage.Replace(coreConfigValues[3].Value, reminderDatas.InspectionId.ToString());
            if (!string.IsNullOrEmpty(reminderDatas.WebAppShortLink))
            {
                sbMessage.Replace(coreConfigValues[4].Value, "<a href='" + reminderDatas.WebAppShortLink + "'>" + reminderDatas.WebAppShortLink + "</a>");
            }
            if (reminderDatas.PendingDocuments != null &&
                reminderDatas.PendingDocuments.Count() > 0)
            {
                sbMessage.Replace(coreConfigValues[5].Value, string.Join(", ", reminderDatas.PendingDocuments));
            }
            if (reminderDatas.RejectedDocuments != null &&
                reminderDatas.RejectedDocuments.Count() > 0)
            {
                sbMessage.Replace(coreConfigValues[6].Value, string.Join(", ", reminderDatas.RejectedDocuments));
            }
            sbMessage.Replace(coreConfigValues[7].Value, reminderDatas.CreatedTime.ToString("dd/MM/yyyy HH:mm:ss"));
            var totalDays = string.Join(" ", Math.Floor((double)(DateTime.UtcNow.Date - reminderDatas.CreatedTime.Date).TotalDays).ToString(), "Days");
            sbMessage.Replace(coreConfigValues[8].Value, totalDays);
            if (!string.IsNullOrEmpty(reminderDatas.BrokerName))
            {
                sbMessage.Replace(coreConfigValues[9].Value, reminderDatas.BrokerName);
            }

            return sbMessage.ToString();
        }

        #endregion

        #region Cancel Inspection

        public void CancelInspection(Applications application, long userId)
        {
            application.ApplicationStatus = (int)Models.Enums.ApplicationStatus.Cancelled;
            application.UpdatedBy = userId;
            application.UpdatedTime = DateTime.UtcNow;

            dbContext.Entry(application).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

        #endregion

        #region Get Lender Configuration By Guid

        public LenderConfigurations GetLenderConfigurationByGuid(string lenderGuid)
        {
            // Get Lender Configuration By Guid
            var response = (from l in dbContext.LenderConfigurations
                            where l.LenderCompanyGuid == lenderGuid
                            select l).FirstOrDefault();

            return response;
        }

        #endregion

        #region Update Lender Configuration

        public void UpdateLenderConfiguration(LenderConfigurations lenderConfiguration)
        {
            dbContext.Entry(lenderConfiguration).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

        #endregion

        #region Get AppImage Details by Id

        public AppImages GetAppImageDetailsById(long imageId)
        {
            // Get AppImage Details by Id
            var response = (from ai in dbContext.AppImages
                            where ai.Id == imageId
                            select ai).FirstOrDefault();

            return response;
        }

        public AppImages GetAppImageDetailsById(long imageId, long applicationId)
        {
            // Get AppImage Details by Id
            var response = (from ai in dbContext.AppImages
                            where ai.Id == imageId &&
                                  ai.ApplicationId == applicationId
                            select ai).FirstOrDefault();

            return response;
        }

        #endregion

        #region Update App Images

        public void UpdateAppImages(AppImages appImage)
        {
            dbContext.Entry(appImage).State = EntityState.Modified;
        }

        #endregion

        #region Update App Documents

        public void UpdateAppDocuments(AppDocuments appDocument)
        {
            dbContext.Entry(appDocument).State = EntityState.Modified;
        }

        #endregion

        #region Get Image Data

        public Models.InspectionGetImageResponse GetImageData(AppImages appImage)
        {
            var response = new Models.InspectionGetImageResponse();

            //Upload Blob to Azure Storage Container
            var blobName = string.Empty;
            var imageProperties = new ImageProperties();
            azureBlobHelper.GetBlobFromAzureStorageContainer(appImage.FilePath, ref blobName, out imageProperties);

            // Get Image Data
            response.ImageData = blobName;
            response.ImageProperties = imageProperties;

            return response;
        }

        #endregion

        #region Get AppImages By Inspection Id

        public List<AppImages> GetAppImagesByInspectionId(long applicationId)
        {
            // Get AppImages By Inspection Id
            var responses = (from ai in dbContext.AppImages
                             where ai.ApplicationId == applicationId
                             select ai).ToList();

            return responses;
        }

        #endregion

        #region Get AppImageReasons by Inspection Id

        public List<AppImageReasons> GetAppImageReasonsByInspectionId(long inspectionId)
        {
            // Get AppImageReasons by Inspection Id
            var responses = dbContext.AppImageReasons.Where(air => air.ApplicationId == inspectionId).ToList();

            return responses;
        }

        #endregion

        #region Add AppImageReasons

        public void AddAppImageReasons(List<AppImageReasons> appImageReasons)
        {
            dbContext.AppImageReasons.AddRange(appImageReasons);
        }

        #endregion

        #region Delete AppImageReasons

        public void DeleteAppImageReasons(List<AppImageReasons> appImageReasons)
        {
            dbContext.AppImageReasons.RemoveRange(appImageReasons);
        }

        #endregion

        #region Save Db Changes

        public void SaveDbChanges()
        {
            dbContext.SaveChanges();
        }

        #endregion

        #region Get Notifications List

        public IQueryable<Models.NotificationsList> GetNotificationsList(string userGuid, string companyGuid)
        {
            // Get Notifications Details
            var responses = (from al in dbContext.AppActivityLogs
                             .Where(al => (al.AppActivityId == (int)Models.Enums.AppActivityLogs.InspectionCreated &&
                                           al.UserGuid == userGuid ||
                                           al.AppActivityId != (int)Models.Enums.AppActivityLogs.InspectionCreated &&
                                           al.AppActivityId != (int)Models.Enums.AppActivityLogs.InspectionShared ||
                                           al.AppActivityId == (int)Models.Enums.AppActivityLogs.InspectionShared &&
                                           al.UserGuid == userGuid))
                             join a in dbContext.Applications
                              .Where(a => a.BrokerCompanyGuid == companyGuid)
                             on al.ApplicationId equals a.Id
                             join seller in dbContext.AppUsers
                             on new { Id = a.Id, Role = (short)Models.Enums.Role.Seller }
                             equals new { Id = seller.ApplicationId, Role = seller.Role }
                             join buyer in dbContext.AppUsers
                             on new { Id = a.Id, Role = (short)Models.Enums.Role.Buyer }
                             equals new { Id = buyer.ApplicationId, Role = buyer.Role }
                             join ts in dbContext.ADTemplateSets
                             on a.TemplateSetGuid equals ts.TemplateSetGUID
                             join num in dbContext.NotificationUserMappings
                             .Where(num => num.UserGuid == userGuid &&
                                           num.CompanyGuid == companyGuid)
                             on al.AppActivityId equals num.AppActivityId
                             join act in dbContext.AppActivities
                             on num.AppActivityId equals act.Id
                             select new Models.NotificationsList()
                             {
                                 InspectionId = a.Id,
                                 Buyer = buyer != null
                                       ? buyer.Name + " " + buyer.SurName
                                       : "",
                                 Seller = seller != null
                                        ? seller.Name + " " + seller.SurName
                                        : "",
                                 AssetType = ts.TemplateName,
                                 ProcessedTime = al.ProcessedTime,
                                 NotificationType = act.Description,
                                 InspectionStatus = act.Id
                             }).AsQueryable();

            return responses;

            //var responses = new List<Models.NotificationsList>();

            //// Get Mapped Inspections
            //var mappedInspections = (from a in dbContext.Applications
            //                         join ash in dbContext.AppStakeholders
            //                         on a.Id equals ash.ApplicationId
            //                         join seller in dbContext.AppUsers
            //                         on new { Id = a.Id, Role = (short)Models.Enums.Role.Seller }
            //                         equals new { Id = seller.ApplicationId, Role = seller.Role }
            //                         join buyer in dbContext.AppUsers
            //                         on new { Id = a.Id, Role = (short)Models.Enums.Role.Buyer }
            //                         equals new { Id = buyer.ApplicationId, Role = buyer.Role }
            //                         join ts in dbContext.ADTemplateSets
            //                         on a.TemplateSetGuid equals ts.TemplateSetGUID
            //                         where ash.UserGuid == userGuid &&
            //                               a.BrokerCompanyGuid == companyGuid
            //                         select new Models.InspectionDetails()
            //                         {
            //                             InspectionId = a.Id,
            //                             Buyer = new Models.AppUser()
            //                             {
            //                                 Name = buyer.Name,
            //                                 SurName = buyer.SurName,
            //                                 Email = buyer.Email,
            //                                 PhoneNumber = buyer.PhoneNumber
            //                             },
            //                             Seller = new Models.AppUser()
            //                             {
            //                                 Name = seller.Name,
            //                                 SurName = seller.SurName,
            //                                 Email = seller.Email,
            //                                 PhoneNumber = seller.PhoneNumber
            //                             },
            //                             AssetType = ts.TemplateName
            //                         }).ToList();

            //// Get Inspection Details
            //var inspectionsDetails = (from m in mappedInspections
            //                          select new Models.NotificationsList()
            //                          {
            //                              InspectionId = m.InspectionId,
            //                              Buyer = string.Join(" ", m.Buyer.Name, m.Buyer.SurName),
            //                              Seller = string.Join(" ", m.Seller.Name, m.Seller.SurName),
            //                              AssetType = m.AssetType
            //                          }).ToList();

            //inspectionsDetails.ForEach(
            //    inspection =>
            //    {
            //        // Get Notifications Details
            //        var result = (from al in dbContext.AppActivityLogs
            //                      .Where(al => (al.ApplicationId == inspection.InspectionId &&
            //                                   (al.AppActivityId == (int)Models.Enums.AppActivityLogs.InspectionCreated &&
            //                                    al.UserGuid == userGuid ||
            //                                    al.AppActivityId != (int)Models.Enums.AppActivityLogs.InspectionCreated &&
            //                                    al.AppActivityId != (int)Models.Enums.AppActivityLogs.InspectionShared ||
            //                                    al.AppActivityId == (int)Models.Enums.AppActivityLogs.InspectionShared &&
            //                                    al.UserGuid == userGuid)))
            //                      join num in dbContext.NotificationUserMappings
            //                      .Where(num => num.UserGuid == userGuid &&
            //                                    num.CompanyGuid == companyGuid)
            //                      on al.AppActivityId equals num.AppActivityId
            //                      join a in dbContext.AppActivities
            //                      on num.AppActivityId equals a.Id
            //                      select new Models.NotificationsList()
            //                      {
            //                          InspectionId = inspection.InspectionId,
            //                          Buyer = inspection.Buyer,
            //                          Seller = inspection.Seller,
            //                          AssetType = inspection.AssetType,
            //                          ProcessedTime = al.ProcessedTime,
            //                          NotificationType = a.Description,
            //                          InspectionStatus = a.Id
            //                      }).ToList();

            //        responses.AddRange(result);
            //    });

            //responses.AddRange(result);
        }

        #endregion

        #region Get Notifications Count

        public int GetNotificationsCount(string userGuid, string companyGuid)
        {
            // Get Notifications Count
            var response = (from al in dbContext.AppActivityLogs
                            .Where(al => (al.AppActivityId == (int)Models.Enums.AppActivityLogs.InspectionCreated &&
                                          al.UserGuid == userGuid ||
                                          al.AppActivityId != (int)Models.Enums.AppActivityLogs.InspectionCreated &&
                                          al.AppActivityId != (int)Models.Enums.AppActivityLogs.InspectionShared ||
                                          al.AppActivityId == (int)Models.Enums.AppActivityLogs.InspectionShared &&
                                          al.UserGuid == userGuid) && al.IsNotified == false)
                            join a in dbContext.Applications
                            .Where(a => a.BrokerCompanyGuid == companyGuid)
                            on al.ApplicationId equals a.Id
                            join num in dbContext.NotificationUserMappings
                            .Where(num => num.UserGuid == userGuid &&
                                          num.CompanyGuid == companyGuid)
                            on al.AppActivityId equals num.AppActivityId
                            join act in dbContext.AppActivities
                            on num.AppActivityId equals act.Id
                            select al).Count();

            return response;
        }

        #endregion

        #region Update Notification Status

        public void UpdateNotificationStatus(string userGuid, string companyGuid)
        {
            var notifications = (from al in dbContext.AppActivityLogs
                                 .Where(al => (al.AppActivityId == (int)Models.Enums.AppActivityLogs.InspectionCreated &&
                                               al.UserGuid == userGuid ||
                                               al.AppActivityId != (int)Models.Enums.AppActivityLogs.InspectionCreated &&
                                               al.AppActivityId != (int)Models.Enums.AppActivityLogs.InspectionShared ||
                                               al.AppActivityId == (int)Models.Enums.AppActivityLogs.InspectionShared &&
                                               al.UserGuid == userGuid) && al.IsNotified == false)
                                 join a in dbContext.Applications
                                 .Where(a => a.BrokerCompanyGuid == companyGuid)
                                 on al.ApplicationId equals a.Id
                                 join num in dbContext.NotificationUserMappings
                                 .Where(num => num.UserGuid == userGuid &&
                                               num.CompanyGuid == companyGuid)
                                 on al.AppActivityId equals num.AppActivityId
                                 join act in dbContext.AppActivities
                                 on num.AppActivityId equals act.Id
                                 select al).ToList();

            notifications.ForEach(
                notification =>
                {
                    notification.IsNotified = true;
                });

            dbContext.AppActivityLogs.UpdateRange(notifications);
            SaveDbChanges();
        }

        #endregion

        #region Save Payment Log

        public void SavePaymentLog(PaymentLogs paymentLogs)
        {
            dbContext.PaymentLogs.Add(paymentLogs);
            dbContext.SaveChanges();
        }

        #endregion

        #region Get TemplateSet Plan Price

        public decimal GetTemplateSetPlanPrice(string templateSetGuid, string planGuid, string lenderGuid, bool isLenderPayer)
        {
            ADTemplateSetPlans plan = null;

            if (!string.IsNullOrEmpty(planGuid))
            {
                plan = (from tsp in dbContext.ADTemplateSetPlans
                        where tsp.TemplateGuid == templateSetGuid &&
                              tsp.PlanGuid == planGuid
                        select tsp).FirstOrDefault();
            }
            else
            {
                plan = (from tsp in dbContext.ADTemplateSetPlans
                        where tsp.TemplateGuid == templateSetGuid &&
                              tsp.IsDefaultActivated == true
                        select tsp).FirstOrDefault();
            }


            if (isLenderPayer)
            {
                var lenderPlans = (from lp in dbContext.ADTemplateSetLenderPlans
                                   where lp.TemplateGuid == plan.TemplateGuid &&
                                         lp.PlanGuid == plan.PlanGuid &&
                                         lp.LenderCompanyGuid == lenderGuid &&
                                         lp.IsActive == true
                                   select lp).FirstOrDefault();

                if (lenderPlans != null)
                {
                    return lenderPlans.Price;
                }
            }

            return plan.Price;
        }

        #endregion

        #region Update Last NotifiedTime

        public void UpdateLastNotifiedTime(long inspectionId, long userId)
        {
            var inspectionDetails = GetInspectionDetails(inspectionId);
            if (inspectionDetails != null)
            {
                inspectionDetails.LastNotifiedTime = DateTime.UtcNow;
                inspectionDetails.UpdatedBy = userId;
                inspectionDetails.UpdatedTime = DateTime.UtcNow;

                dbContext.Entry(inspectionDetails).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        #endregion

        #region Save DVS Checks

        public void SaveDVSChecks(DVSChecks dvsChecks)
        {
            dbContext.DVSChecks.Add(dvsChecks);
            dbContext.SaveChanges();
        }

        #endregion

        #region Update DVS Status

        public void UpdateDVSStatus(long inspectionId, int dvsStatus, long userId)
        {
            var inspectionDetails = GetInspectionDetails(inspectionId);
            if (inspectionDetails != null)
            {
                inspectionDetails.DVSStatus = dvsStatus;
                inspectionDetails.UpdatedBy = userId;
                inspectionDetails.UpdatedTime = DateTime.UtcNow;

                dbContext.Entry(inspectionDetails).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        #endregion

        #region Get TemplateSet Details By Guid

        public ADTemplateSets GetTemplateSetDetailsByGuid(string templateSetGuid)
        {
            var response = (from adt in dbContext.ADTemplateSets
                            where adt.TemplateSetGUID == templateSetGuid
                            select adt).FirstOrDefault();

            return response;
        }

        #endregion

        #region Get Lender Details By Guid

        public ADCompanies GetLenderDetailsByGuid(string lenderGuid)
        {
            var response = (from adc in dbContext.ADCompanies
                            where adc.CompanyGuid == lenderGuid
                            select adc).FirstOrDefault();

            return response;
        }

        #endregion

        #region Verify Inspection Permission

        public bool VerifyInspectionPermission(long inspectionId, string userGuid)
        {
            var response = (from ash in dbContext.AppStakeholders
                            where ash.ApplicationId == inspectionId &&
                                  ash.UserGuid == userGuid
                            select ash.Id).Count() > 0;

            return response;
        }

        #endregion

        #region Get Illion Details By CompanyId

        public IllionIntegrationDetails GetIllionDetailsByCompanyGuid(string companyGuid)
        {
            var response = (from i in dbContext.IllionIntegrationDetails
                            where i.CompanyGuid == companyGuid
                            select i).FirstOrDefault();

            return response;
        }

        #endregion

        #region Save Illion Details

        public void SaveIllionDetails(IllionIntegrationDetails illionIntegrationDetails)
        {
            if (illionIntegrationDetails.Id == 0)
            {
                dbContext.IllionIntegrationDetails.Add(illionIntegrationDetails);
            }
            else
            {
                dbContext.Entry(illionIntegrationDetails).State = EntityState.Modified;
            }

            dbContext.SaveChanges();
        }

        #endregion
    }
}