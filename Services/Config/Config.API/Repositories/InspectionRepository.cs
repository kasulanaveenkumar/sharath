using Config.API.Entities;
using Config.API.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Config.API.Models.Asset;
using Config.API.Entities.SP;

namespace Config.API.Repositories
{
    public class InspectionRepository : IInspectionRepository
    {
        private readonly ConfigContext dbContext;

        #region Constructor

        public InspectionRepository(Entities.ConfigContext context)
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

        #region Get Lender Detail By Guid

        public Companies GetLenderDetailByGuid(string lenderGuid)
        {
            // Get Lender Detail By Guid
            var response = (from c in dbContext.Companies
                            where c.CompanyGuid == lenderGuid
                            select c).FirstOrDefault();

            return response;
        }

        #endregion

        #region Get TemplateSet Detail By Guid

        public TemplateSets GetTemplateSetDetailByGuid(string templateSetGuid)
        {
            // Get TemplateSet Detail By Guid
            var response = (from t in dbContext.TemplateSets
                            where t.TemplateSetGuid == templateSetGuid
                            select t).FirstOrDefault();

            return response;
        }

        #endregion

        #region Get Plan Detail By Guid

        public TemplateSetPlans GetPlanDetailByGuid(string planGuid)
        {
            // Get TemplateSet Detail By Guid
            var response = (from t in dbContext.TemplateSetPlans
                            where t.PlanGuid == planGuid
                            select t).FirstOrDefault();

            return response;
        }

        #endregion

        #region Get State Detail By Id

        public States GetStateDetailById(long stateId)
        {
            var response = (from s in dbContext.States
                            where s.Id == stateId
                            select s
                           ).FirstOrDefault();

            return response;
        }

        #endregion

        #region Get Template Plans

        public List<InspectionPlansResponse> GetTemplatePlans(long lenderCompanyId, long templateSetId, long stateId)
        {
            var responses = (from t in dbContext.TemplateSetPlans
                             join tm in dbContext.TemplateSetCustomPlanMappings 
                             on t.Id equals tm.PlanId
                             where tm.LenderCompanyId == lenderCompanyId &&
                                   tm.TemplateSetId == templateSetId &&
                                   ((tm.IsAppliedAllState == true && 
                                     tm.StateId == 0) ||
                                     tm.StateId == stateId) &&
                                   tm.IsActive == true
                             select new InspectionPlansResponse()
                             {
                                 PlanGuid = t.PlanGuid,
                                 PlanName = t.PlanName,
                                 MaxDocument = t.MaxDocument,
                                 Price = t.Price,
                                 LoanAmount = t.LoanAmount,
                                 PlanLevel = t.PlanLevel
                             })
                             .Distinct()
                             .OrderBy(p => p.PlanLevel)
                             .ToList();

            return responses;
        }

        #endregion

        #region Get Template Id

        public long GetTemplateId(string templateGuid)
        {
            // Get Template Id
            var response = (from ts in dbContext.TemplateSets
                            where ts.TemplateSetGuid == templateGuid
                            select ts.Id).FirstOrDefault();

            return response;
        }

        #endregion

        #region Get Template Documents

        public List<DocumentsListResponse> GetTemplateDocuments(long templateId)
        {
            // Get Template Documents
            var responses = (from tdm in dbContext.TemplateSetDocMappings
                             join td in dbContext.TemplateDocuments
                             on tdm.TemplateDocumentId equals td.Id
                             where tdm.TemplateSetId == templateId
                             select new DocumentsListResponse()
                             {
                                 Id = td.Id,
                                 Name = td.Name,
                                 Position = tdm.Position,
                                 CategoryId = td.TemplateDocCategoryId,
                                 AdditionalPrice = td.AdditionalPrice
                             })
                             .OrderBy(t => t.Position)
                             .ToList();

            return responses;
        }

        #endregion

        #region Get Plan Documents

        public List<TemplateSetPlanDocMappings> GetPlanDocuments(string planGuid)
        {
            // Get Plan Documents
            var responses = (from tp in dbContext.TemplateSetPlans
                             join tpdm in dbContext.TemplateSetPlanDocMappings
                             on tp.Id equals tpdm.TemplateSetPlanId
                             where tp.PlanGuid == planGuid
                             select tpdm).ToList();

            return responses;
        }

        #endregion

        #region Get Document Images

        public List<AssetImageListResponse> GetDocumentImages(long documentId, int docCategoryId, bool isIncludeNoLenderPreference)
        {
            // Get Document Images
            var responses = (from ti in dbContext.TemplateImages
                             join tim in dbContext.TemplateDocImageMappings
                             on ti.Id equals tim.TemplateImageId
                             where tim.TemplateDocumentId == documentId &&
                                   ti.TemplateDocCategoryId == docCategoryId
                             select new AssetImageListResponse()
                             {
                                 ImageName = ti.Name,
                                 ImageType = ti.ImageType,
                                 Description = ti.Description,
                                 DocGroup = ti.DocGroup,
                                 Position = tim.Position,
                                 IsMandatory = true,
                                 IsDefaultSelected = true,
                                 IsCheckboxDisabled = false,
                                 WarningMessage = isIncludeNoLenderPreference ? ti.WarningMessage : string.Empty // For NoLender Show Warning Message
                             })
                             .OrderBy(i => i.Position)
                             .ToList();

            return responses;
        }

        #endregion

        #region Get TemplateDocNoLenderPreference

        public TemplateDocNoLenderPreferences GetTemplateDocNoLenderPreference(long templateSetId, string userGuid, string companyGuid)
        {
            // Get TemplateDocNoLenderPreference
            var response = dbContext.TemplateDocNoLenderPreferences.FirstOrDefault(t => t.TemplateSetId == templateSetId &&
                                                                                        t.UserGuid == userGuid &&
                                                                                        t.CompanyGuid == t.CompanyGuid);

            return response;
        }

        #endregion

        #region Update TemplateDocNoLenderPreference

        public void UpdateTemplateDocNoLenderPreference(TemplateDocNoLenderPreferences templateDocNoLenderPreferences)
        {
            dbContext.Entry(templateDocNoLenderPreferences).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

        #endregion

        #region Save TemplateDocNoLenderPreference

        public void SaveTemplateDocNoLenderPreference(TemplateDocNoLenderPreferences templateDocNoLenderPreferences)
        {
            dbContext.TemplateDocNoLenderPreferences.Add(templateDocNoLenderPreferences);
            dbContext.SaveChanges();
        }

        #endregion

        #region Get AssetDoc Details

        public List<Usp_GetNewInspectionDocuments> GetAssetDocDetails(AssetDocListRequest model, string userGuid, string companyGuid)
        {
            var param = new SqlParameter[]
            {
                new SqlParameter("LenderCompanyGuid", model.LenderGuid),
                new SqlParameter("TemplateSetGuid", model.TemplateGuid),
                new SqlParameter("PlanGuid", model.planGuid),
                new SqlParameter("StateId", model.StateId)
            };

            var response = dbContext.GetNewInspectionDocuments.FromSqlRaw<Entities.SP.Usp_GetNewInspectionDocuments>
                           (
                             "usp_GetNewInspectionDocuments @LenderCompanyGuid, @TemplateSetGuid, @PlanGuid, @StateId",
                             param
                           )
                           .ToList();
            return response;
        }

        #endregion
    }
}
