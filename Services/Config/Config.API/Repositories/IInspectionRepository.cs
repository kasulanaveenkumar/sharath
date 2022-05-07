using Config.API.Entities;
using Config.API.Entities.SP;
using Config.API.Models;
using Config.API.Models.Asset;
using System.Collections.Generic;

namespace Config.API.Repositories
{
    public interface IInspectionRepository
    {
        public void BeginTransaction();

        public void CommitTransaction();

        public void RollbackTransaction();

        public Companies GetLenderDetailByGuid(string lenderGuid);

        public TemplateSets GetTemplateSetDetailByGuid(string templateSetGuid);

        public TemplateSetPlans GetPlanDetailByGuid(string planGuid);

        public States GetStateDetailById(long stateId);

        public List<InspectionPlansResponse> GetTemplatePlans(long lenderCompanyId, long templateSetId, long stateId);

        public long GetTemplateId(string templateGuid);

        public List<DocumentsListResponse> GetTemplateDocuments(long templateId);

        public List<TemplateSetPlanDocMappings> GetPlanDocuments(string planGuid);

        public List<AssetImageListResponse> GetDocumentImages(long documentId, int docCategoryId, bool isIncludeNoLenderPreference);

        public TemplateDocNoLenderPreferences GetTemplateDocNoLenderPreference(long templateSetId, string userGuid, string companyGuid);

        public void UpdateTemplateDocNoLenderPreference(TemplateDocNoLenderPreferences templateDocNoLenderPreferences);

        public void SaveTemplateDocNoLenderPreference(TemplateDocNoLenderPreferences templateDocNoLenderPreferences);

        public List<Usp_GetNewInspectionDocuments> GetAssetDocDetails(AssetDocListRequest model, string userGuid, string companyGuid);
    }
}
