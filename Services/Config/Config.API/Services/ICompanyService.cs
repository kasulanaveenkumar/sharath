using Common.Payments.Models;
using Config.API.Entities;
using Config.API.Entities.SP;
using Config.API.Models;
using Config.API.Models.Broker;
using Config.API.Models.Payment;
using System.Collections.Generic;

namespace Config.API.Services
{
    public interface ICompanyService
    {
        public List<Companies> GetCompanySuggestions(OnboardCompanySuggestionRequest model);

        public List<CompaniesListResponse> GetCompaniesList();

        public List<AssetsWorkWithResponse> GetAllAssets(bool isSupportTeam = false);

        public List<LendersWorkWithResponse> GetAllLenders(bool isSupportTeam, bool isAddEditCompany);

        public Companies GetCompanyDetailsByGuid(string companyGuid);

        public OnboardDetailsResponse GetOnboardDetails(OnboardDetailsRequest model);

        public void OnboardBroker(BrokerRegisterRequest model, long userId, out string companyGuid, out string invalidCardDetails);

        public void SaveBrokerCompanyDetails(Companies companyDtls, Models.Broker.SaveCompanyDetailsRequest model,
                                             string companyGuid, long userId, string token, out bool isCompanyExists, out string result);

        public Models.Broker.SaveCompanyDetailsResponse GetBrokerCompanyDetails(Companies companyDetails);

        public int SaveAdminCompanyDetails(Models.SaveCompanyDetailsRequest model, string token, long userId,
                                           out bool isCompanyExists,
                                           out string result,
                                           out string errorMsg);

        public Models.SaveCompanyDetailsResponse GetAdminCompanyDetails(Companies companyDetails, string token);

        public void SaveBrokerAdminCompanyDetails(BrokerAdminCompanyRequest model, string companyGuid, string token, long userId);

        public BrokerAdminCompanyResponse GetBrokerAdminCompanyDetails(Companies companyDetails);

        public void SaveLenderCompanyDetails(Companies companyDetails, Models.Lender.LenderCompanyRequest model,
                                             string lenderCompanyGuid, string token,
                                             long userId, out string errorMessage);

        public Models.Lender.LenderCompanyResponse GetLenderCompanyDetails(Companies companyDetails, string lenderCompanyGuid, string token);

        public Models.Lender.OnboardDetailsResponse GetOnboardDetails();

        public void OnboardLender(Models.Lender.LenderRegisterRequest model, long userId, string token, out string companyGuid, out string result);

        public Companies GetCompanyByName(string name, string companyGuid);

        public List<StateOptionsResponse> GetStateoption();

        public List<LendersWorkWithResponse> GetLendersWorkWith(long companyId);

        public List<AssetsWorkWithResponse> GetAssetsWorkWith(long companyId, int userTypeId);

        public List<LendersListResponse> GetLendersList(LendersListRequest model);

        public UserProfileCompanyResponse GetUserProfileCompany(Entities.Companies companyDetails);

        public void SaveUserProfileCompany(UserProfileCompanyRequest model, string companyGuid, long userId);

        public LenderCompanyResponse GetLenderDetails(string lenderCompanyGuid, string token);

        public Companies GetLenderByName(string lenderName, string lenderCompanyGuid);

        public void SaveLenderDetails(LenderCompanyRequest model, long createdBy, string token,
                                      ref string lenderCompanyGuid, out string errorMessage);

        public List<Usp_GetLendersList> GetAllLendersDetails();

        public Companies GetCompanyDetails(string companyGuid);

        public string SaveCardDetails(Companies companyDetails, List<PaymentMethod> paymentMethods, long userId);

        public List<CardDetails> GetCardDetails(Companies companyDetails);

        public string GetCreditCardType(string cardNumber);

        public CardDetails ValidateCard(CardDetails model);

        public string GetValidatedUsersResult(List<BrokerUsers> users);

        public string GetInvalidUsersEmail(List<string> usersList, long userTypeId);

        public string GetInvalidAssetsWorkWith(List<string> assetsWorkWith);

        public string GetInvalidLendersWorkWith(List<string> lendersWorkWith);

        public List<ADUsers> GetUserDetailsByCompanyGuid(string companyGuid);

        public string GetInvalidCompanyContactTypes(List<Models.CompanyContacts> companyContacts);

        public void ImportBrokers(string token, out int importErrorCount);
    }
}
