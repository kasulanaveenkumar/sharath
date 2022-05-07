using Config.API.Entities;
using Config.API.Models;
using Config.API.Models.Lender;
using System.Collections.Generic;

namespace Config.API.Services
{
    public interface ILenderService
    {
        public int GetCompanyVisibility(Companies companyDetails);

        public int SaveCompanyVisibility(Companies companyDetails, SaveCompanyVisibilityRequest model);

        public Companies GetLenderCompanyDetailsByGuid(string lendercompanyGuid);

        public List<BrokerCompanyResponse> GetBrokerCompanies(string companyGuid);

        public void UpdateLenderVisibility(Companies companyDetails, List<SaveLenderVisibilityRequest> model);

        public List<LenderBlockedBrokerUsersResponse> GetBlockedBrokerUsers(string brokerCompanyGuid, string lenderCompanyGuid);

        public void SaveBlockedBrokerUsers(LenderBlockedBrokerUsersRequest model, string lenderCompanyGuid);

        public TemplateSetandStates GetMappedAssetsStates(string companyGuid);

        public InspectionPlanDetailReponse GetAssetDocumentDetails(Entities.TemplateSets templateDetails, string templateGuid, 
                                                                   bool isAppliedAllStates, bool isUseDbValue, string companyGuid);

        public void SaveInspectionPlans(Entities.TemplateSets templateDetails, InspectionPlanDetailRequest model, string companyGuid);

        public InspectionPlanDetailReponse GetTemplatePlanDetails(string templateGuid, string companyGuid);
    }
}
