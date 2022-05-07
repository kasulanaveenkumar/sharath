using Common.Extensions;
using Core.API.Entities;
using Core.API.Entities.SP;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Core.API.Repositories
{
    public class LenderRepository : ILenderRepository
    {
        private readonly CoreContext dbContext;

        #region Constructor

        public LenderRepository(CoreContext context)
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

        #region Get All Brokers

        public List<Models.Brokers> GetAllBrokers()
        {
            // Get All Brokers
            var responses = (from c in dbContext.ADCompanies
                             where c.CompanyTypeId == (int)Common.Models.Enums.CompanyTypes.Broker
                             select new Models.Brokers()
                             {
                                 CompanyGuid = c.CompanyGuid,
                                 CompanyName = c.CompanyName
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

        #region Get Inspections List

        public List<Usp_GetInspections> GetInspectionsList(Models.LenderInspectionsRequest model, string userGuid, string companyGuid)
        {
            var userGuidParam = new SqlParameter { ParameterName = "UserGuid", SqlDbType = SqlDbType.VarChar, Value = userGuid };
            var companyGuidParam = new SqlParameter { ParameterName = "CompanyGuid", SqlDbType = SqlDbType.VarChar, Value = companyGuid };

            var assetFilter = string.IsNullOrEmpty(model.AssetFilter) ? (object)DBNull.Value : model.AssetFilter;
            var assetFilterParam = new SqlParameter { ParameterName = "AssetFilter", SqlDbType = SqlDbType.VarChar, Value = assetFilter };

            var lenderFilter = string.IsNullOrEmpty(model.BrokerFilter) ? (object)DBNull.Value : model.BrokerFilter;
            var lenderFilterParam = new SqlParameter { ParameterName = "LenderFilter", SqlDbType = SqlDbType.VarChar, Value = lenderFilter };

            var statusFilterParam = new SqlParameter { ParameterName = "StatusFilter", SqlDbType = SqlDbType.Int, Value = model.StatusFilter };

            var filterText = string.IsNullOrEmpty(model.FilterText) ? (object)DBNull.Value : model.FilterText;
            var filterTextParam = new SqlParameter { ParameterName = "FilterText", SqlDbType = SqlDbType.VarChar, Value = filterText };

            var sortColumnParam = new SqlParameter { ParameterName = "SortColumn", SqlDbType = SqlDbType.VarChar, Value = model.SortColumn };
            var sortDirectionParam = new SqlParameter { ParameterName = "SortDirection", SqlDbType = SqlDbType.VarChar, Value = model.SortDirection };

            var skipDataParam = new SqlParameter { ParameterName = "SkipData", SqlDbType = SqlDbType.Int, Value = model.SkipData };
            var limitDataParam = new SqlParameter { ParameterName = "LimitData", SqlDbType = SqlDbType.Int, Value = model.LimitData };

            var isNewUserParam = new SqlParameter { ParameterName = "IsNewUser", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output };

            var result = dbContext.InspectionsList.FromSqlRaw<Usp_GetInspections>
                         (
                           "Usp_GetInspections @UserGuid, @CompanyGuid, @AssetFilter, @LenderFilter, @StatusFilter, @FilterText," +
                           "@SortColumn, @SortDirection, @SkipData, @LimitData, @IsNewUser OUTPUT",
                           userGuidParam, companyGuidParam, assetFilterParam, lenderFilterParam, statusFilterParam, filterTextParam,
                           sortColumnParam, sortDirectionParam, skipDataParam, limitDataParam, isNewUserParam
                         )
                         .ToList();

            return result;
            //bool includeSuspendedInspections = false;
            //bool includeBypassRequestedInspections = false;

            //List<int> lstInspectionStatus = new List<int>();

            //if (model.StatusFilter == (int)Enums.ApplicationStatus.Created ||
            //    model.StatusFilter == (int)Enums.ApplicationStatus.Started ||
            //    model.StatusFilter == (int)Enums.ApplicationStatus.Submitted ||
            //    model.StatusFilter == (int)Enums.ApplicationStatus.Rejected ||
            //    model.StatusFilter == (int)Enums.ApplicationStatus.Completed ||
            //    model.StatusFilter == (int)Enums.ApplicationStatus.Delayed ||
            //    model.StatusFilter == (int)Enums.ApplicationStatus.Cancelled)
            //{
            //    lstInspectionStatus.Add(model.StatusFilter);
            //}
            //else if (model.StatusFilter == (int)Enums.ApplicationStatus.Suspended)
            //{
            //    lstInspectionStatus.Add((int)Enums.ApplicationStatus.Created);
            //    lstInspectionStatus.Add((int)Enums.ApplicationStatus.Started);
            //    lstInspectionStatus.Add((int)Enums.ApplicationStatus.Submitted);
            //    lstInspectionStatus.Add((int)Enums.ApplicationStatus.Rejected);
            //    lstInspectionStatus.Add((int)Enums.ApplicationStatus.Completed);
            //    lstInspectionStatus.Add((int)Enums.ApplicationStatus.Delayed);
            //    lstInspectionStatus.Add((int)Enums.ApplicationStatus.Cancelled);

            //    includeSuspendedInspections = true;
            //}
            //else if (model.StatusFilter == (int)Enums.ApplicationStatus.ByPassRequested)
            //{
            //    lstInspectionStatus.Add((int)Enums.ApplicationStatus.Rejected);

            //    includeBypassRequestedInspections = true;
            //}

            //// Get User Mapped Inspections
            //var userMappedInspections = (from a in dbContext.Applications
            //                             join ash in dbContext.AppStakeholders
            //                             on new { Id = a.Id, IsOwner = 1 }
            //                             equals new { Id = ash.ApplicationId, IsOwner = (int)ash.IsOwner }
            //                             join seller in dbContext.AppUsers
            //                             on new { Id = a.Id, Role = (short)Enums.Role.Seller }
            //                             equals new { Id = seller.ApplicationId, Role = seller.Role }
            //                             join buyer in dbContext.AppUsers
            //                             on new { Id = a.Id, Role = (short)Enums.Role.Buyer }
            //                             equals new { Id = buyer.ApplicationId, Role = buyer.Role }
            //                             join ts in dbContext.ADTemplateSets
            //                             on a.TemplateSetGuid equals ts.TemplateSetGUID
            //                             join b in dbContext.ADCompanies
            //                             .Where(a => a.CompanyTypeId == (int)Common.Models.Enums.CompanyTypes.Broker)
            //                             on a.BrokerCompanyGuid equals b.CompanyGuid
            //                             join u in dbContext.ADUsers
            //                             on a.CreatedBy equals u.UserId
            //                             where a.LenderCompanyGuid == companyGuid &&
            //                                 ((string.IsNullOrEmpty(model.AssetFilter) ||
            //                                   a.TemplateSetGuid == model.AssetFilter) &&
            //                                 (string.IsNullOrEmpty(model.BrokerFilter) ||
            //                                  a.BrokerCompanyGuid == model.BrokerFilter) &&
            //                                 (
            //                                    // No status filter applied
            //                                    (model.StatusFilter == 0 &&
            //                                            a.IsSuspended == false) ||
            //                                    // With status filter
            //                                    (model.StatusFilter > 0 &&
            //                                    (model.StatusFilter != (int)Enums.ApplicationStatus.ByPassRequested ||
            //                                     model.StatusFilter != (int)Enums.ApplicationStatus.Suspended) &&
            //                                        lstInspectionStatus.Contains(a.ApplicationStatus) &&
            //                                            a.IsSuspended == includeSuspendedInspections &&
            //                                            a.IsBypassRequested == includeBypassRequestedInspections ||
            //                                     model.StatusFilter == (int)Enums.ApplicationStatus.ByPassRequested &&
            //                                           (a.IsSuspended == false && a.IsBypassRequested == true) ||
            //                                     model.StatusFilter == (int)Enums.ApplicationStatus.Suspended &&
            //                                           ((a.IsSuspended == true && a.IsBypassRequested == false) ||
            //                                            (a.IsSuspended == true && a.IsBypassRequested == true)))
            //                                 ))
            //                             select new Models.LenderInspectionsList()
            //                             {
            //                                 BuyerName = string.Join(" ", buyer.Name, buyer.SurName),
            //                                 SellerName = string.Join(" ", seller.Name, seller.SurName),
            //                                 TemplateSetGuid = a.TemplateSetGuid,
            //                                 BrokerCompanyGuid = a.BrokerCompanyGuid,
            //                                 InspectionId = ash.ApplicationId,
            //                                 Status = a.ApplicationStatus == (int)Enums.ApplicationStatus.Completed
            //                                        ? ((Enums.ApplicationStatus)(int)Enums.ApplicationStatus.Completed).GetEnumDescriptionAttributeValue()
            //                                        : ((Enums.ApplicationStatus)a.ApplicationStatus).GetEnumDescriptionAttributeValue(),
            //                                 ApplicationStatus = a.ApplicationStatus,
            //                                 CompanyName = b.CompanyName,
            //                                 AssetType = ts.TemplateName,
            //                                 CreatedBy = a.CreatedBy,
            //                                 UpdatedTime = a.UpdatedTime,
            //                                 IsSuspended = a.IsSuspended,
            //                                 IsBypassRequested = a.IsBypassRequested
            //                             }).ToList();

            //// Get Inspections List
            //var responses = (from m in userMappedInspections
            //                 //join u in AdditionalDataContext.Context.Users
            //                 //on m.CreatedBy equals u.UserId
            //                 //into LenderInspections
            //                 //from lender in LenderInspections.DefaultIfEmpty()
            //                 select new Models.LenderInspectionsList()
            //                 {
            //                     InspectionId = m.InspectionId,
            //                     CompanyName = m.CompanyName,
            //                     //BrokerName = lender != null
            //                     //           ? string.Join(" ", lender.Name, lender.Surname)
            //                     //           : "",
            //                     AssetType = m.AssetType,
            //                     BuyerName = m.BuyerName,
            //                     SellerName = m.SellerName,
            //                     UpdatedTime = m.UpdatedTime,
            //                     Status = model.StatusFilter == (int)Enums.ApplicationStatus.Suspended
            //                            ? ((Enums.ApplicationStatus)(int)Enums.ApplicationStatus.Suspended).GetEnumDescriptionAttributeValue()
            //                            : m.Status,
            //                     ApplicationStatus = m.ApplicationStatus,
            //                     IsSuspended = m.IsSuspended,
            //                     IsBypassRequested = m.IsBypassRequested
            //                 }).AsQueryable();

            //responses = (from i in responses
            //             where ((string.IsNullOrEmpty(model.FilterText)) ||
            //             (i.InspectionId.ToString().ToLower().Contains(model.FilterText.ToLower()) ||
            //              i.CompanyName.ToLower().Contains(model.FilterText.ToLower()) ||
            //              i.BuyerName.ToLower().Contains(model.FilterText.ToLower()) ||
            //              i.SellerName.ToLower().Contains(model.FilterText.ToLower())))
            //             select new Models.LenderInspectionsList()
            //             {
            //                 InspectionId = i.InspectionId,
            //                 CompanyName = i.CompanyName,
            //                 //BrokerName = i.BrokerName,
            //                 AssetType = i.AssetType,
            //                 BuyerName = i.BuyerName,
            //                 SellerName = i.SellerName,
            //                 UpdatedTime = i.UpdatedTime,
            //                 Status = i.Status,
            //                 ApplicationStatus = i.ApplicationStatus,
            //                 IsSuspended = i.IsSuspended,
            //                 IsBypassRequested = i.IsBypassRequested
            //             }).AsQueryable();

            //return responses;
        }

        #endregion

        #region Get Completed Inspections List

        public List<Usp_GetInspections> GetCompletedInspectionsList(Models.LenderInspectionsRequest model, string userGuid, string companyGuid)
        {
            var userGuidParam = new SqlParameter { ParameterName = "UserGuid", SqlDbType = SqlDbType.VarChar, Value = userGuid };
            var companyGuidParam = new SqlParameter { ParameterName = "CompanyGuid", SqlDbType = SqlDbType.VarChar, Value = companyGuid };

            var assetFilter = string.IsNullOrEmpty(model.AssetFilter) ? (object)DBNull.Value : model.AssetFilter;
            var assetFilterParam = new SqlParameter { ParameterName = "AssetFilter", SqlDbType = SqlDbType.VarChar, Value = assetFilter };

            var lenderFilter = string.IsNullOrEmpty(model.BrokerFilter) ? (object)DBNull.Value : model.BrokerFilter;
            var lenderFilterParam = new SqlParameter { ParameterName = "LenderFilter", SqlDbType = SqlDbType.VarChar, Value = lenderFilter };

            var statusFilterParam = new SqlParameter { ParameterName = "StatusFilter", SqlDbType = SqlDbType.Int, Value = (int)Models.Enums.ApplicationStatus.Completed };

            var filterText = string.IsNullOrEmpty(model.FilterText) ? (object)DBNull.Value : model.FilterText;
            var filterTextParam = new SqlParameter { ParameterName = "FilterText", SqlDbType = SqlDbType.VarChar, Value = filterText };

            var sortColumnParam = new SqlParameter { ParameterName = "SortColumn", SqlDbType = SqlDbType.VarChar, Value = model.SortColumn };
            var sortDirectionParam = new SqlParameter { ParameterName = "SortDirection", SqlDbType = SqlDbType.VarChar, Value = model.SortDirection };

            var skipDataParam = new SqlParameter { ParameterName = "SkipData", SqlDbType = SqlDbType.Int, Value = model.SkipData };
            var limitDataParam = new SqlParameter { ParameterName = "LimitData", SqlDbType = SqlDbType.Int, Value = model.LimitData };

            var isNewUserParam = new SqlParameter { ParameterName = "IsNewUser", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output };

            var result = dbContext.InspectionsList.FromSqlRaw<Usp_GetInspections>
                         (
                           "Usp_GetInspections @UserGuid, @CompanyGuid, @AssetFilter, @LenderFilter, @StatusFilter, @FilterText," +
                           "@SortColumn, @SortDirection, @SkipData, @LimitData, @IsNewUser OUTPUT",
                           userGuidParam, companyGuidParam, assetFilterParam, lenderFilterParam, statusFilterParam, filterTextParam,
                           sortColumnParam, sortDirectionParam, skipDataParam, limitDataParam, isNewUserParam
                         )
                         .ToList();

            return result;
            //bool includeSuspendedInspections = false;
            //bool includeBypassRequestedInspections = false;

            //List<int> lstInspectionStatus = new List<int>();

            //if (model.StatusFilter == (int)Enums.ApplicationStatus.Created ||
            //    model.StatusFilter == (int)Enums.ApplicationStatus.Started ||
            //    model.StatusFilter == (int)Enums.ApplicationStatus.Submitted ||
            //    model.StatusFilter == (int)Enums.ApplicationStatus.Rejected ||
            //    model.StatusFilter == (int)Enums.ApplicationStatus.Completed ||
            //    model.StatusFilter == (int)Enums.ApplicationStatus.Delayed ||
            //    model.StatusFilter == (int)Enums.ApplicationStatus.Cancelled)
            //{
            //    lstInspectionStatus.Add(model.StatusFilter);
            //}
            //else if (model.StatusFilter == (int)Enums.ApplicationStatus.Suspended)
            //{
            //    lstInspectionStatus.Add((int)Enums.ApplicationStatus.Created);
            //    lstInspectionStatus.Add((int)Enums.ApplicationStatus.Started);
            //    lstInspectionStatus.Add((int)Enums.ApplicationStatus.Submitted);
            //    lstInspectionStatus.Add((int)Enums.ApplicationStatus.Rejected);
            //    lstInspectionStatus.Add((int)Enums.ApplicationStatus.Completed);
            //    lstInspectionStatus.Add((int)Enums.ApplicationStatus.Delayed);
            //    lstInspectionStatus.Add((int)Enums.ApplicationStatus.Cancelled);

            //    includeSuspendedInspections = true;
            //}
            //else if (model.StatusFilter == (int)Enums.ApplicationStatus.ByPassRequested)
            //{
            //    lstInspectionStatus.Add((int)Enums.ApplicationStatus.Rejected);

            //    includeBypassRequestedInspections = true;
            //}

            //// Get User Mapped Inspections
            //var userMappedInspections = (from a in dbContext.Applications
            //                             join ash in dbContext.AppStakeholders
            //                             on new { Id = a.Id, IsOwner = 1 }
            //                             equals new { Id = ash.ApplicationId, IsOwner = (int)ash.IsOwner }
            //                             join seller in dbContext.AppUsers
            //                             on new { Id = a.Id, Role = (short)Enums.Role.Seller }
            //                             equals new { Id = seller.ApplicationId, Role = seller.Role }
            //                             join ts in dbContext.ADTemplateSets
            //                             on a.TemplateSetGuid equals ts.TemplateSetGUID
            //                             join b in dbContext.ADCompanies
            //                             .Where(a => a.CompanyTypeId == (int)Common.Models.Enums.CompanyTypes.Broker)
            //                             on a.BrokerCompanyGuid equals b.CompanyGuid
            //                             join u in dbContext.ADUsers
            //                             on a.CreatedBy equals u.UserId
            //                             where a.LenderCompanyGuid == companyGuid &&
            //                                 ((string.IsNullOrEmpty(model.AssetFilter) || a.TemplateSetGuid == model.AssetFilter) &&
            //                                 (string.IsNullOrEmpty(model.BrokerFilter) || a.BrokerCompanyGuid == model.BrokerFilter) &&
            //                                 (
            //                                    // No status filter applied
            //                                    (model.StatusFilter == 0 &&
            //                                        (a.ApplicationStatus == (int)Enums.ApplicationStatus.Completed &&
            //                                            a.IsSuspended == false)) ||
            //                                    // With status filter
            //                                    (model.StatusFilter > 0 &&
            //                                    (model.StatusFilter != (int)Enums.ApplicationStatus.ByPassRequested ||
            //                                     model.StatusFilter != (int)Enums.ApplicationStatus.Suspended) &&
            //                                        lstInspectionStatus.Contains(a.ApplicationStatus) &&
            //                                            a.IsSuspended == includeSuspendedInspections &&
            //                                            a.IsBypassRequested == includeBypassRequestedInspections ||
            //                                     model.StatusFilter == (int)Enums.ApplicationStatus.ByPassRequested &&
            //                                           (a.IsSuspended == false && a.IsBypassRequested == true) ||
            //                                     model.StatusFilter == (int)Enums.ApplicationStatus.Suspended &&
            //                                           ((a.IsSuspended == true && a.IsBypassRequested == false) ||
            //                                            (a.IsSuspended == true && a.IsBypassRequested == true)))
            //                                 ))
            //                             select new Models.LenderCompletedInspectionsList()
            //                             {
            //                                 SellerName = string.Join(" ", seller.Name, seller.SurName),
            //                                 TemplateSetGuid = a.TemplateSetGuid,
            //                                 BrokerCompanyGuid = a.BrokerCompanyGuid,
            //                                 LenderRef = a.RefNumber,
            //                                 InspectionId = ash.ApplicationId,
            //                                 BypassStatus = a.IsBypassRequested == true
            //                                              ? "Yes"
            //                                              : "No",
            //                                 UpdatedTime = a.UpdatedTime,
            //                                 Status = a.ApplicationStatus == (int)Enums.ApplicationStatus.Completed
            //                                        ? ((Enums.ApplicationStatus)(int)Enums.ApplicationStatus.Completed).GetEnumDescriptionAttributeValue()
            //                                        : ((Enums.ApplicationStatus)a.ApplicationStatus).GetEnumDescriptionAttributeValue(),
            //                                 ApplicationStatus = a.ApplicationStatus,
            //                                 AssetType = ts.TemplateName,
            //                                 CreatedBy = a.CreatedBy,
            //                                 IsSuspended = a.IsSuspended,
            //                                 IsBypassRequested = a.IsBypassRequested
            //                             }).ToList();

            //// Get Inspections List
            //var responses = (from m in userMappedInspections
            //                 //join u in AdditionalDataContext.Context.Users
            //                 //on m.CreatedBy equals u.UserId
            //                 //into LenderCompletedInspections
            //                 //from lender in LenderCompletedInspections.DefaultIfEmpty()
            //                 select new Models.LenderCompletedInspectionsList()
            //                 {
            //                     SellerName = m.SellerName,
            //                     AssetType = m.AssetType,
            //                     //BrokerName = lender != null
            //                     //           ? string.Join(" ", lender.Name, lender.Surname)
            //                     //           : "",
            //                     LenderRef = m.LenderRef,
            //                     InspectionId = m.InspectionId,
            //                     UpdatedTime = m.UpdatedTime,
            //                     BypassStatus = m.BypassStatus,
            //                     Status = model.StatusFilter == (int)Enums.ApplicationStatus.Suspended
            //                            ? ((Enums.ApplicationStatus)(int)Enums.ApplicationStatus.Suspended).GetEnumDescriptionAttributeValue()
            //                            : m.Status,
            //                     ApplicationStatus = m.ApplicationStatus,
            //                     IsSuspended = m.IsSuspended,
            //                     IsBypassRequested = m.IsBypassRequested
            //                 }).AsQueryable();

            //responses = (from i in responses
            //             where ((string.IsNullOrEmpty(model.FilterText)) ||
            //             (i.InspectionId.ToString().Contains(model.FilterText) ||
            //              i.BrokerName.Contains(model.FilterText) ||
            //              i.SellerName.Contains(model.FilterText)))
            //             select new Models.LenderCompletedInspectionsList()
            //             {
            //                 SellerName = i.SellerName,
            //                 AssetType = i.AssetType,
            //                 BrokerName = i.BrokerName,
            //                 LenderRef = i.LenderRef,
            //                 InspectionId = i.InspectionId,
            //                 UpdatedTime = i.UpdatedTime,
            //                 BypassStatus = i.BypassStatus,
            //                 Status = i.Status,
            //                 ApplicationStatus = i.ApplicationStatus,
            //                 IsSuspended = i.IsSuspended,
            //                 IsBypassRequested = i.IsBypassRequested
            //             }).AsQueryable();

            //return responses;
        }

        #endregion
    }
}
