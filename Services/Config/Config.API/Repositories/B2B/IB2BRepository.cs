using Config.API.Entities;
using Config.API.Entities.SP;
using Config.API.Models;
using Config.API.Models.Asset;
using System.Collections.Generic;

namespace Config.API.Repositories.B2B
{
    public interface IB2BRepository
    {
        public List<BrokerLenderMappings> GetBrokerLenderMappings(long companyId);

        public List<Companies> GetLenderCompanies();

        public List<BrokerTemplateMappings> GetBrokerTemplateMappings(long companyId);

        public List<TemplateSets> GetTemplateSets();

        public Companies GetCompanyByGuid(string companyGuid);

        public List<StateOptionsResponse> GetStates();

        public TemplateSets GetTemplateSetDetailByGuid(string templateSetGuid);

        public List<Usp_GetNewInspectionDocuments> GetAssetDocDetails(AssetDocListRequest model, string userGuid, string companyGuid);

        public TemplateDocNoLenderPreferences GetTemplateDocNoLenderPreference(long templateSetId, string userGuid, string companyGuid);
    }
}
