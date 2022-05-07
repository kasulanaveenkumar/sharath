using Config.API.Entities;
using Config.API.Models;
using Config.API.Models.Asset;
using System.Collections.Generic;

namespace Config.API.Services
{
    public interface IInspectionService
    {
        public Companies GetLenderDetailByGuid(string lenderGuid);

        public TemplateSets GetTemplateSetDetailByGuid(string templateSetGuid);

        public TemplateSetPlans GetPlanDetailByGuid(string planGuid);

        public States GetStateDetailById(long stateId);

        public List<InspectionPlansResponse> GetTemplatePlans(long lenderCompanyId, long templateSetId, long stateId);

        public TemplateDetails GetAssetDocDetails(AssetDocListRequest model, string userGuid, string companyGuid);

        public void SaveNoLenderPreference(TemplateSets templateSetDetail, SaveNoLenderPreferenceRequest model,
                                           string userGuid, string companyGuid);
    }
}
