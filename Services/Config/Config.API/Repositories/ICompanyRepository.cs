using Config.API.Entities;
using Config.API.Entities.SP;
using Config.API.Models;
using System.Collections.Generic;

namespace Config.API.Repositories
{
    public interface ICompanyRepository
    {
        public void BeginTransaction();

        public void CommitTransaction();

        public void RollbackTransaction();

        public List<Companies> GetBrokerCompanies();

        public List<CompaniesListResponse> GetCompaniesList();

        public List<AllCompaniesResponse> GetAllBrokers();

        public void SaveCompanyDetails(Companies company);

        public void UpdateCompanyDetails(Companies companyDetails);

        public void UpdateCompanyDatas(Companies companyDetails);

        public List<Companies> GetLenderCompanies();

        public Companies GetCompanyByGuid(string companyGuid);

        public Companies GetCompanyByName(string name, string companyGuid);

        public List<Usp_GetLendersList> GetLendersList();

        public Companies GetLenderByName(string lenderName, string lenderCompanyGuid);

        public List<TemplateSets> GetTemplateSets();

        public List<BrokerTemplateMappings> GetBrokerTemplateMappings(long companyId);

        public void AddBrokerTemplateMappings(long companyId, long templateId);

        public void RemoveBrokerTemplateMappings(long companyId);

        public List<BrokerLenderMappings> GetBrokerLenderMappings(long companyId);

        public void AddBrokerLenderMappings(long brokerCompanyId, long lenderCompanyId);

        public void RemoveBrokerLenderMappings(long brokerCompanyId);

        public List<LenderTemplateMappings> GetLenderTemplateMappings(long companyId);

        public void AddLenderTemplateMappings(long companyId, long templateId);

        public void RemoveLenderTemplateMappings(long companyId);

        public void RemoveMappedLenders(long lenderCompanyId);

        public List<long> GetMappedTemplateSets(List<string> assetsWorkWith);

        public List<long> GetMappedLenders(List<string> lendersWorkWith);

        public List<Models.CompanyContacts> GetCompanyContacts(long companyId);

        public void AddCompanyContacts(long companyId, Models.CompanyContacts contact);

        public void RemoveCompanyContacts(long companyId);

        public void SaveDbChanges();

        public List<Companies> GetCompaniesByDomain(string domain);

        public List<AssetsWorkWithResponse> GetAllAssets(bool isSupportTeam = false);

        public List<LendersWorkWithResponse> GetAllLenders(bool isSupportTeam, bool isAddEditCompany);

        public List<StateOptionsResponse> GetStateoption();

        public string GetInvalidUsersEmail(List<string> usersList, long userTypeId);

        public string GetInvalidAssetsWorkWith(List<string> assetsWorkWith);

        public string GetInvalidLendersWorkWith(List<string> lendersWorkWith);

        public ADUsers GetUserDetailsByEmail(string email);

        public List<ADUsers> GetUserDetailsByCompanyGuid(string companyGuid);

        public string GetInvalidCompanyContactTypes(List<Models.CompanyContacts> companyContacts);

        public List<StaCollectorDetails> GetStaCollectorDetails();

        public List<LenderVisibilityMappings> GetLenderVisibilityMappings(long companyId);

        public void UpdateStaCollectorDetails(List<StaCollectorDetails> companyInfos);

        public void SaveChanges();
    }
}
