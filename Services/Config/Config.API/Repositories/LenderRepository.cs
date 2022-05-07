using System.Collections.Generic;
using System.Linq;
using Common.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Config.API.Repositories
{
    public class LenderRepository : ILenderRepository
    {
        private readonly Entities.ConfigContext dbContext;

        #region Constructor
        public LenderRepository(Entities.ConfigContext context)
        {
            dbContext = context;
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

        #region Get Company Detail By Guid

        public Entities.Companies GetCompanyDetailsByGuid(string companyGuid)
        {
            // Get Company Detail By Guid
            var response = (from c in dbContext.Companies
                            where c.CompanyGuid == companyGuid
                            select c).FirstOrDefault();

            return response;
        }

        #endregion

        #region Save Company Visibility

        public void SaveCompanyVisibility(Entities.Companies company)
        {
            dbContext.Entry(company).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

        #endregion

        #region Get Broker Companies

        public List<Entities.Companies> GetBrokerCompanies()
        {
            // Get Broker Companies
            var responses = (from c in dbContext.Companies.Where(c => c.CompanyTypeId == (int)Enums.CompanyTypes.Broker)
                             select c).ToList();

            return responses;
        }

        public List<Models.BrokerCompanyResponse> GetBrokerCompanies(List<Entities.Companies> brokerCompanies, long companyId)
        {
            // Get Broker Companies
            var responses = (from c in brokerCompanies
                             join blm in dbContext.BrokerLenderMappings.Where(l => l.LenderCompanyId == companyId)
                             on c.Id equals blm.BrokerCompanyId
                             into BrokerDetails
                             from broker in BrokerDetails.DefaultIfEmpty()
                             select new Models.BrokerCompanyResponse()
                             {
                                 CompanyId = c.Id,
                                 CompanyGuid = c.CompanyGuid,
                                 CompanyName = c.CompanyName,
                                 ABN = c.ABN,
                                 Mobile = "812281101",
                                 State = c.State,
                                 NumberofBrokers = dbContext.ADUsers.Count(bu => bu.CompanyGuid == c.CompanyGuid && bu.UserTypeId == (int)Common.Models.Enums.UserTypes.Broker),
                                 NumberofActiveBrokers = dbContext.ADUsers.Count(bu => bu.CompanyGuid == c.CompanyGuid && bu.UserTypeId == (int)Common.Models.Enums.UserTypes.Broker && bu.IsActive == true),
                                 IsVisible = broker != null
                                           ? true
                                           : false
                             }).ToList();

            return responses;
        }

        #endregion

        #region Get Broker Company By Guid

        public List<Entities.Companies> GetBrokerCompanyByGuid(string companyGuid)
        {
            // Get Broker Companies
            var responses = (from c in dbContext.Companies
                             .Where(c => c.CompanyTypeId == (int)Enums.CompanyTypes.Broker &&
                                         c.CompanyGuid == companyGuid)
                             select c).ToList();

            return responses;
        }

        #endregion

        #region Get Lender Company By Guid

        public List<Entities.Companies> GetLenderCompanyByGuid(string companyGuid)
        {
            // Get Lender Companies
            var responses = (from c in dbContext.Companies
                             .Where(c => c.CompanyTypeId == (int)Enums.CompanyTypes.Lender &&
                                         c.CompanyGuid == companyGuid)
                             select c).ToList();

            return responses;
        }

        #endregion

        #region Get Broker Users By Company

        public List<Entities.ADUsers> GetBrokerUsersByCompany(string companyGuid)
        {
            // Get BrokerUsers By Company
            var brokerUsers = dbContext.ADUsers
                              .Where(bu => bu.CompanyGuid == companyGuid && 
                                     bu.UserTypeId == (int)Common.Models.Enums.UserTypes.Broker)
                              .ToList();

            return brokerUsers;
        }

        #endregion

        #region Get Blocked BrokerUsers List

        public List<Entities.LenderBlockedBrokerUsers> GetBlockedBrokerUsersList(long brokerCompanyId, long lenderCompanyId)
        {
            // Get Lender Blocked Broker Users
            var responses = (from lbbu in dbContext.LenderBlockedBrokerUsers
                             where lbbu.BrokerCompanyId == brokerCompanyId &&
                                   lbbu.LenderCompanyId == lenderCompanyId
                             select lbbu).ToList();

            return responses;
        }

        #endregion

        #region Get Blocked BrokerUsers

        public List<Models.LenderBlockedBrokerUsersResponse> GetBlockedBrokerUsers(List<Entities.ADUsers> brokerUsers,
                                                                                   List<Entities.LenderBlockedBrokerUsers> blockedUsersList)
        {
            // Get Mapped Broker Users
            var responses = (from bu in brokerUsers
                             join blockedUser in blockedUsersList
                             on bu.UserGuid equals blockedUser.UserGuid
                             into BlockedUsers
                             from user in BlockedUsers.DefaultIfEmpty()
                             select new Models.LenderBlockedBrokerUsersResponse()
                             {
                                 UserGuid = bu.UserGuid,
                                 Name = bu.Name,
                                 Surname = bu.SurName,
                                 Email = bu.Email,
                                 Mobile = bu.Mobile,
                                 AverageInpsectionsPerMonth = 0,
                                 IsAllowed = (user != null ? false : true)
                             }).ToList();

            return responses;
        }

        #endregion

        #region Add Lender Blocked Broker Users 

        public void AddBlockedBrokerUsers(Entities.LenderBlockedBrokerUsers lenderBlockedBrokerUsers)
        {
            dbContext.LenderBlockedBrokerUsers.Add(lenderBlockedBrokerUsers);
        }

        #endregion

        #region Remove Lender Blocked Broker Users 

        public void RemoveBlockedBrokerUsers(Entities.LenderBlockedBrokerUsers lenderBlockedBrokerUsers)
        {
            dbContext.LenderBlockedBrokerUsers.Remove(lenderBlockedBrokerUsers);
        }

        #endregion

        #region Get Mapped Assets

        public List<Models.Lender.TemplateSets> GetMappedAssets(string companyGuid)
        {
            // Get Asset Mapped To Lenders
            var assets = (from c in dbContext.Companies
                          join map in dbContext.LenderTemplateMappings
                          on c.Id equals map.CompanyId
                          where c.CompanyGuid == companyGuid
                          select map.TemplateId).ToList();
            var responses = (from t in dbContext.TemplateSets
                             where assets.Contains(t.Id)
                             select new Models.Lender.TemplateSets()
                             {
                                 TemplateName = t.Name,
                                 TemplateSetGuid = t.TemplateSetGuid
                             }).ToList();

            return responses;
        }

        #endregion

        #region Get State Option

        public List<Models.StateOptionsResponse> GetStateoption()
        {
            // Get State Option
            var responses = (from st in dbContext.States
                             select new Models.StateOptionsResponse()
                             {
                                 StateID = st.Id,
                                 StateCode = st.StateCode
                             }).ToList();

            return responses;
        }

        #endregion

        #region Get TemplateSet Detail By Guid

        public Entities.TemplateSets GetTemplateSetDetailByGuid(string templateSetGuid)
        {
            // Get TemplateSet Detail By Guid
            var response = (from t in dbContext.TemplateSets
                            where t.TemplateSetGuid == templateSetGuid
                            select t).FirstOrDefault();

            return response;
        }

        #endregion

        #region Get TemplateSet Plans

        public List<Models.Lender.TemplateSetPlanDetailsResponse> GetTemplateSetPlans(long templateSetId, bool isAppliedAllState, string companyGuid)
        {
            var responses = new List<Models.Lender.TemplateSetPlanDetailsResponse>();

            // Get TemplateSet Plans
            var templatePlans = dbContext.TemplateSetPlans
                                .Where(t => t.TemplateSetId == templateSetId && t.IsActive == true)
                                .OrderBy(p => p.PlanLevel)
                                .ToList();

            // Get Company Details By Guid
            var companyDetails = GetCompanyDetailsByGuid(companyGuid);

            // Get Custom Plan Documents for all plans
            var customPlanMappings = GetTemplateSetCustomPlanMappings(templateSetId, companyDetails.Id);

            if (customPlanMappings.Count() == 0)
            {
                responses = templatePlans.Select(
                    t => new Models.Lender.TemplateSetPlanDetailsResponse()
                    {
                        TemplatePlanId = t.Id,
                        TemplatePlanGuid = t.PlanGuid,
                        TemplatePlanName = t.PlanName,
                        TemplatePlanDescription = t.PlanDescription,
                        BasePrice = t.Price,
                        IsActivated = t.IsDefaultActivated,
                        TemplatePlanLevel = t.PlanLevel,
                        IsAppliedAllState = true,
                        StateId = 0
                    }).ToList();
            }
            else
            {
                responses = (from tp in templatePlans
                             join cpm in customPlanMappings
                             on tp.Id equals cpm.PlanId
                             //where cpm.IsAppliedAllState == isAppliedAllState
                             select new Models.Lender.TemplateSetPlanDetailsResponse()
                             {
                                 TemplatePlanId = cpm.Id,
                                 TemplatePlanGuid = tp.PlanGuid,
                                 TemplatePlanName = tp.PlanName,
                                 TemplatePlanDescription = tp.PlanDescription,
                                 BasePrice = tp.Price,
                                 IsActivated = cpm.IsActive,
                                 TemplatePlanLevel = tp.PlanLevel,
                                 IsAppliedAllState = cpm.IsAppliedAllState,
                                 StateId = cpm.StateId
                             }).ToList();
            }

            return responses;
        }

        public List<Models.Lender.TemplateSetPlanDetailsResponse> GetTemplateSetDefaultPlans(long templateSetId)
        {
            // Get TemplateSet Plans
            var templatePlans = dbContext.TemplateSetPlans
                                .Where(t => t.TemplateSetId == templateSetId && t.IsActive == true)
                                .OrderBy(p => p.PlanLevel)
                                .ToList();

            var responses = templatePlans.Select(
                t => new Models.Lender.TemplateSetPlanDetailsResponse()
                {
                    TemplatePlanId = t.Id,
                    TemplatePlanGuid = t.PlanGuid,
                    TemplatePlanName = t.PlanName,
                    TemplatePlanDescription = t.PlanDescription,
                    BasePrice = t.Price,
                    IsActivated = t.IsDefaultActivated,
                    TemplatePlanLevel = t.PlanLevel,
                    IsAppliedAllState = true,
                    StateId = 0
                }).ToList();

            return responses;
        }

        #endregion

        #region Get Template Documents

        public List<Models.Lender.AssetDocumentsListResponse> GetTemplateDocuments(long templateId)
        {
            // Get Template Documents
            var responses = (from tdm in dbContext.TemplateSetDocMappings
                             join td in dbContext.TemplateDocuments
                             on tdm.TemplateDocumentId equals td.Id
                             where tdm.TemplateSetId == templateId
                             select new Models.Lender.AssetDocumentsListResponse()
                             {
                                 Id = td.Id,
                                 Name = td.Name,
                                 Description = td.Description,
                                 WarningMessage = td.WarningMessage,
                                 CategoryId = td.TemplateDocCategoryId,
                                 Position = tdm.Position,
                                 HasAdditionalData = tdm.HasAdditionalData,
                                 MinimumPlanLevelToInclude = td.MinimumPlanLevelToInclude
                             }).OrderBy(t => t.Position).ToList();

            return responses;
        }

        #endregion

        #region Get Document Images

        public List<Models.Lender.AssetImagesListResponse> GetDocumentImages(long documentId, int docCategoryId)
        {
            // Get Document Images
            var responses = (from ti in dbContext.TemplateImages
                             join tim in dbContext.TemplateDocImageMappings
                             on ti.Id equals tim.TemplateImageId
                             where tim.TemplateDocumentId == documentId &&
                                   ti.TemplateDocCategoryId == docCategoryId
                             select new Models.Lender.AssetImagesListResponse()
                             {
                                 Id = ti.Id,
                                 Name = ti.Name,
                                 Description = ti.Description,
                                 WarningMessage = ti.WarningMessage,
                                 ImageType = ti.ImageType,
                                 AllowSkip = ti.AllowSkip
                             }).ToList();

            return responses;
        }

        #endregion

        #region Get TemplateSet CustomPlan Mappings

        public List<Entities.TemplateSetCustomPlanMappings> GetTemplateSetCustomPlanMappings(long templateSetId, long lenderCompanyId)
        {
            var responses = (from tcpm in dbContext.TemplateSetCustomPlanMappings
                             where tcpm.TemplateSetId == templateSetId &&
                                   tcpm.LenderCompanyId == lenderCompanyId
                             select tcpm).ToList();

            return responses;
        }

        #endregion

        #region Add TemplateSet CustomPlan Mappings

        public void AddTemplateSetCustomPlanMappings(Entities.TemplateSetCustomPlanMappings customPlanMappings)
        {
            dbContext.TemplateSetCustomPlanMappings.Add(customPlanMappings);
        }

        #endregion

        #region Update TemplateSet CustomPlan Mappings

        public void UpdateTemplateSetCustomPlanMappings(Entities.TemplateSetCustomPlanMappings customPlanMappings)
        {
            dbContext.Entry(customPlanMappings).State = EntityState.Modified;
        }

        #endregion

        #region Get Custom documents for all plan

        public List<Entities.TemplateSetCustomDocMappings> GetCustomDocuments(List<long> planIds)
        {
            var responses = dbContext.TemplateSetCustomDocMappings.Where(d => planIds.Contains(d.CustomPlanId)).ToList();

            return responses;
        }

        #endregion

        #region Add TemplateSet CustomDoc Mappings

        public void AddTemplateSetCustomDocMappings(Entities.TemplateSetCustomDocMappings customDocMappings)
        {
            dbContext.TemplateSetCustomDocMappings.Add(customDocMappings);
        }

        #endregion

        #region Update TemplateSet CustomDoc Mappings

        public void UpdateTemplateSetCustomDocMappings(Entities.TemplateSetCustomDocMappings customDocMappings)
        {
            dbContext.Entry(customDocMappings).State = EntityState.Modified;
        }

        #endregion

        #region Get Custom images for all plan

        public List<Entities.TemplateSetCustomImageMappings> GetCustomImages(List<long> planIds)
        {
            var responses = dbContext.TemplateSetCustomImageMappings.Where(d => planIds.Contains(d.CustomPlanId)).ToList();

            return responses;
        }

        #endregion

        #region Add TemplateSet CustomImage Mappings

        public void AddTemplateSetCustomImageMappings(Entities.TemplateSetCustomImageMappings customImageMappings)
        {
            dbContext.TemplateSetCustomImageMappings.Add(customImageMappings);
        }

        #endregion

        #region Update TemplateSet CustomImage Mappings

        public void UpdateTemplateSetCustomImageMappings(Entities.TemplateSetCustomImageMappings customImageMappings)
        {
            dbContext.Entry(customImageMappings).State = EntityState.Modified;
        }

        #endregion

        #region Save Db Changes

        public void SaveDbChanges()
        {
            dbContext.SaveChanges();
        }

        #endregion

        public List<Entities.SP.Usp_GetInspectionPlanDetails> GetTemplatePlanData(string templateGuid, string companyGuid)
        {
            var param = new SqlParameter[] 
                            {
                                new SqlParameter("LenderCompanyGuid", companyGuid),
                                new SqlParameter("TemplateSetGuid", templateGuid)
                            };

            var data = dbContext.GetInspectionPlanDetails.FromSqlRaw("Usp_GetInspectionPlanDetails @LenderCompanyGuid, @TemplateSetGuid", param);
            return data.ToList();
        }
    }
}
