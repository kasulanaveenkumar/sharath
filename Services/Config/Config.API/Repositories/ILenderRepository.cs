using System.Collections.Generic;

namespace Config.API.Repositories
{
    public interface ILenderRepository
    {
        public void BeginTransaction();

        public void CommitTransaction();

        public void RollbackTransaction();

        public Entities.Companies GetCompanyDetailsByGuid(string companyGuid);

        public void SaveCompanyVisibility(Entities.Companies company);

        public List<Entities.Companies> GetBrokerCompanies();

        public List<Models.BrokerCompanyResponse> GetBrokerCompanies(List<Entities.Companies> brokerCompanies, long companyId);

        public List<Entities.Companies> GetBrokerCompanyByGuid(string companyGuid);

        public List<Entities.Companies> GetLenderCompanyByGuid(string companyGuid);

        public List<Entities.ADUsers> GetBrokerUsersByCompany(string companyGuid);

        public List<Entities.LenderBlockedBrokerUsers> GetBlockedBrokerUsersList(long brokerCompanyId, long lenderCompanyId);

        public List<Models.LenderBlockedBrokerUsersResponse> GetBlockedBrokerUsers(List<Entities.ADUsers> brokerUsers,
                                                                                   List<Entities.LenderBlockedBrokerUsers> blockedUsersList);

        public void AddBlockedBrokerUsers(Entities.LenderBlockedBrokerUsers lenderBlockedBrokerUsers);

        public void RemoveBlockedBrokerUsers(Entities.LenderBlockedBrokerUsers lenderBlockedBrokerUsers);

        public List<Models.Lender.TemplateSets> GetMappedAssets(string companyGuid);

        public List<Models.StateOptionsResponse> GetStateoption();

        public Entities.TemplateSets GetTemplateSetDetailByGuid(string templateSetGuid);

        public List<Models.Lender.TemplateSetPlanDetailsResponse> GetTemplateSetPlans(long templateSetId, bool isAppliedAllState, string companyGuid);

        public List<Models.Lender.TemplateSetPlanDetailsResponse> GetTemplateSetDefaultPlans(long templateSetId);

        public List<Models.Lender.AssetDocumentsListResponse> GetTemplateDocuments(long templateId);

        public List<Models.Lender.AssetImagesListResponse> GetDocumentImages(long documentId, int docCategoryId);

        public List<Entities.TemplateSetCustomPlanMappings> GetTemplateSetCustomPlanMappings(long templateSetId, long lenderCompanyId);

        public void AddTemplateSetCustomPlanMappings(Entities.TemplateSetCustomPlanMappings customPlanMappings);

        public void UpdateTemplateSetCustomPlanMappings(Entities.TemplateSetCustomPlanMappings customPlanMappings);

        public List<Entities.TemplateSetCustomDocMappings> GetCustomDocuments(List<long> planIds);

        public void AddTemplateSetCustomDocMappings(Entities.TemplateSetCustomDocMappings customDocMappings);

        public void UpdateTemplateSetCustomDocMappings(Entities.TemplateSetCustomDocMappings customDocMappings);

        public List<Entities.TemplateSetCustomImageMappings> GetCustomImages(List<long> planIds);

        public void AddTemplateSetCustomImageMappings(Entities.TemplateSetCustomImageMappings customImageMappings);

        public void UpdateTemplateSetCustomImageMappings(Entities.TemplateSetCustomImageMappings customImageMappings);

        public List<Entities.SP.Usp_GetInspectionPlanDetails> GetTemplatePlanData(string templateGuid, string companyGuid);

        public void SaveDbChanges();
    }
}
