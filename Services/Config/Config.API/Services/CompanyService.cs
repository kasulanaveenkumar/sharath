using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net.Http;
using System.Net.Mail;
using System.Text.Json;
using Common.AzureBlobUtility.Helper;
using Common.AzureBlobUtility.Models;
using Common.Extensions;
using Common.Messages;
using Common.Models.ApiResponse;
using Common.Payments.Helper;
using Common.Payments.Models;
using Config.API.Entities;
using Config.API.Entities.SP;
using Config.API.Models;
using Config.API.Models.Broker;
using Config.API.Models.Data;
using Config.API.Models.Payment;
using Config.API.Repositories;

namespace Config.API.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository companyRepository;
        private readonly IDataRepository dataRepository;
        private readonly StripeIntegrationHelper stripeIntegrationHelper;
        private readonly AzureBlobHelper azureBlobHelper;

        #region Constructor

        public CompanyService(ICompanyRepository repository, IDataRepository dataRepository)
        {
            this.companyRepository = repository;
            this.dataRepository = dataRepository;

            stripeIntegrationHelper = new StripeIntegrationHelper(Startup.AppConfiguration.GetSection("AppSettings").GetSection("StripeApiKey").Value);

            // Get AccountName value from appsettings.json File
            // Get AccountKey value from appsettings.json File
            // Get Container value from appsettings.json File
            var accountName = Startup.AppConfiguration.GetSection("AppSettings").GetSection("AzureStorageAccountName").Value;
            var accountKey = Startup.AppConfiguration.GetSection("AppSettings").GetSection("AzureStorageAccountKey").Value;
            var containerName = Startup.AppConfiguration.GetSection("AppSettings").GetSection("ContainerNameForCompanyLogo").Value;
            azureBlobHelper = new AzureBlobHelper(accountName, accountKey, containerName);
        }

        #endregion

        #region Get Company Suggestions

        public List<Companies> GetCompanySuggestions(OnboardCompanySuggestionRequest model)
        {
            // Get Domain Name from Email
            var address = new MailAddress(model.Email);
            var domain = address.Host;

            // Get Company Suggestions
            var responses = companyRepository.GetCompaniesByDomain(domain);

            return responses;
        }

        #endregion

        #region Get Companies List

        public List<CompaniesListResponse> GetCompaniesList()
        {
            // Get Companies List
            var responses = companyRepository.GetCompaniesList();

            return responses;
        }

        #endregion

        #region Get All Assets

        public List<AssetsWorkWithResponse> GetAllAssets(bool isSupportTeam = false)
        {
            // Get All Assets
            var responses = companyRepository.GetAllAssets(isSupportTeam);

            return responses;
        }

        #endregion

        #region Get All Lenders

        public List<LendersWorkWithResponse> GetAllLenders(bool isSupportTeam, bool isAddEditCompany)
        {
            // Get All Lenders
            var responses = companyRepository.GetAllLenders(isSupportTeam, isAddEditCompany);

            return responses;
        }

        #endregion

        #region Get Company Details By Guid

        public Companies GetCompanyDetailsByGuid(string companyGuid)
        {
            var response = companyRepository.GetCompanyByGuid(companyGuid);

            return response;
        }

        #endregion

        #region Get Broker Onboard Details

        public OnboardDetailsResponse GetOnboardDetails(OnboardDetailsRequest model)
        {
            var response = new OnboardDetailsResponse();

            // Get Company Suggestions
            // Get Domain Name from Email
            var address = new MailAddress(model.Email);
            var domain = address.Host;
            response.CompanySuggestions = companyRepository.GetCompaniesByDomain(domain);

            // Get States
            response.States = companyRepository.GetStateoption();

            // Get Lenders
            response.Lenders = companyRepository.GetAllLenders(false, false);

            // Get Assets
            response.Assets = companyRepository.GetAllAssets();

            return response;
        }

        #endregion

        #region Broker Onboard-Save Company Details

        public void OnboardBroker(BrokerRegisterRequest model, long userId, out string companyGuid, out string invalidCardDetails)
        {
            companyRepository.BeginTransaction();

            var invalidCardData = string.Empty;

            //Add new company details if IsNewCompany= Yes
            //Select Existing company if IsNewCompany=No
            if (model.IsNewCompany)
            {
                // Save Company Details
                var newCompanyDetail = model.NewCompanyDetail;
                var company = new Companies();
                company.CompanyGuid = ExtensionMethods.GetNewGuid();
                company.CompanyName = newCompanyDetail.CompanyName;
                company.RegisteredName = newCompanyDetail.RegisteredName;
                company.ABN = newCompanyDetail.ABN;
                company.CompanyAddress = newCompanyDetail.CompanyAddress;
                company.City = newCompanyDetail.City;
                company.State = newCompanyDetail.State;
                company.ZIPCode = newCompanyDetail.ZIPCode;
                company.Website = newCompanyDetail.Website;
                company.Email = newCompanyDetail.Email;
                company.CompanyTypeId = (int)Enums.CompanyTypes.Broker;
                company.CreatedBy = userId;
                company.CreatedTime = DateTime.UtcNow;
                company.IsUseCompanyLogoForWebApp = false;
                companyRepository.SaveCompanyDetails(company);

                // Save Card Details
                if (model.PaymentMethods != null &&
                    model.PaymentMethods.Count() > 0)
                {
                    var companyDetails = companyRepository.GetCompanyByGuid(company.CompanyGuid);
                    invalidCardData = SaveCardDetails(companyDetails, model.PaymentMethods, userId);
                }

                if (!string.IsNullOrEmpty(invalidCardData))
                {
                    companyGuid = "";
                    invalidCardDetails = invalidCardData;

                    companyRepository.RollbackTransaction();
                }
                else
                {
                    companyRepository.SaveDbChanges();

                    // Map Asssets to Company
                    if (model.AssetsWorkWith != null &&
                        model.AssetsWorkWith.Count() > 0)
                    {
                        MapBrokerAssetsToCompany(model.AssetsWorkWith, company.Id);
                    }

                    // Map Lenders to Company
                    if (model.LendersWorkWith != null &&
                        model.LendersWorkWith.Count() > 0)
                    {
                        MapLendersToCompany(model.LendersWorkWith, company.Id);
                    }

                    companyRepository.CommitTransaction();

                    companyGuid = company.CompanyGuid;
                    invalidCardDetails = "";
                }
            }
            else
            {
                companyGuid = companyRepository.GetCompanyByGuid(model.ExistingCompanyGuid).CompanyGuid;
                invalidCardDetails = "";
            }
        }

        #endregion

        #region Save Broker Company Details

        public void SaveBrokerCompanyDetails(Companies companyDtls, Models.Broker.SaveCompanyDetailsRequest model,
                                             string companyGuid, long userId, string token,
                                             out bool isCompanyExists, out string result)
        {
            long companyId;

            // Get Company Details
            var companyDetails = companyRepository.GetCompanyByName(model.BrokerCompanyDetails.CompanyName, companyGuid);
            if (companyDetails != null)
            {
                isCompanyExists = true;
                result = "";
            }
            else
            {
                // Save Update Broker Company Details
                UpdateBrokerCompanyDetails(companyDtls, model, companyGuid, userId, token, out companyId);

                // Map Assets to Company
                if (model.AssetsWorkWith != null &&
                    model.AssetsWorkWith.Count() > 0)
                {
                    MapBrokerAssetsToCompany(model.AssetsWorkWith, companyId);
                }

                // Map Lenders to Company
                if (model.LendersWorkWith != null &&
                    model.LendersWorkWith.Count() > 0)
                {
                    MapLendersToCompany(model.LendersWorkWith, companyId);
                }

                isCompanyExists = false;
                result = companyGuid;
            }
        }

        #region Update Broker Company Details

        private void UpdateBrokerCompanyDetails(Companies companyDetails, Models.Broker.SaveCompanyDetailsRequest model,
                                                string companyGuid, long userId, string token, out long companyId)
        {
            // Update Company Details
            var brokerCompanyDetails = model.BrokerCompanyDetails;
            companyDetails.CompanyName = brokerCompanyDetails.CompanyName;
            companyDetails.RegisteredName = brokerCompanyDetails.RegisteredName;
            if (!string.IsNullOrEmpty(brokerCompanyDetails.ABN))
            {
                companyDetails.ABN = brokerCompanyDetails.ABN;
            }
            companyDetails.CompanyAddress = brokerCompanyDetails.CompanyAddress;
            companyDetails.City = brokerCompanyDetails.City;
            companyDetails.State = brokerCompanyDetails.State;
            companyDetails.ZIPCode = brokerCompanyDetails.ZIPCode;
            companyDetails.Website = brokerCompanyDetails.Website;
            if (!string.IsNullOrEmpty(brokerCompanyDetails.Email))
            {
                companyDetails.Email = brokerCompanyDetails.Email;
            }
            companyDetails.AllowOnlyInvitedUser = brokerCompanyDetails.AllowOnlyInvitedUser;
            // Upload Blob to Azure Storage Container
            if (!string.IsNullOrEmpty(model.CompanyLogo))
            {
                var blobName = string.Empty;
                azureBlobHelper.UploadBlobToAzureStorageContainer(model.CompanyLogo, companyGuid, ref blobName);
                companyDetails.CompanyLogoURL = blobName;
            }
            else
            {
                companyDetails.CompanyLogoURL = string.Empty;
            }

            companyDetails.CompanyDescription = model.CompanyDescription;
            companyDetails.UpdatedBy = userId;
            companyDetails.UpdatedTime = DateTime.UtcNow;
            companyRepository.UpdateCompanyDetails(companyDetails);
            SaveUpdateCompanyDetails(companyDetails, token);

            companyId = companyDetails.Id;
        }

        #endregion

        #endregion

        #region Get Broker Company Details

        public Models.Broker.SaveCompanyDetailsResponse GetBrokerCompanyDetails(Companies companyDetails)
        {
            var response = new Models.Broker.SaveCompanyDetailsResponse();

            // Get Company Details
            response.CompanyName = companyDetails.CompanyName;
            response.RegisteredName = companyDetails.RegisteredName;
            response.ABN = companyDetails.ABN != "Blank" 
                         ? companyDetails.ABN 
                         : "";
            response.CompanyAddress = companyDetails.CompanyAddress != "Blank" 
                                    ? companyDetails.CompanyAddress
                                    : "";
            response.ZipCode = companyDetails.ZIPCode != "Blank"
                             ? companyDetails.ZIPCode
                             : "";
            response.State = companyDetails.State != "Blank"
                           ? companyDetails.State
                           : "";
            response.City = companyDetails.City != "Blank"
                          ? companyDetails.City
                          : "";
            response.Website = companyDetails.Website;
            response.Email = companyDetails.Email;
            response.AllowOnlyInvitedUser = companyDetails.AllowOnlyInvitedUser;

            // Get Blob from Azure Storage Container
            if (!string.IsNullOrEmpty(companyDetails.CompanyLogoURL))
            {
                var base64Text = string.Empty;
                var imageProps = new ImageProperties();
                azureBlobHelper.GetBlobFromAzureStorageContainer(companyDetails.CompanyGuid, ref base64Text, out imageProps);
                response.CompanyLogo = base64Text;
            }
            else
            {
                response.CompanyLogo = null;
            }

            response.CompanyDescription = companyDetails.CompanyDescription;

            // Get Lenders Mapped to Company
            response.Lenders = GetLendersWorkWith(companyDetails.Id);

            // Get Assets Mapped To Broker
            response.Assets = GetAssetsMappedToBroker(companyDetails.Id);

            return response;
        }

        #endregion

        #region Save Admin Company Details

        public int SaveAdminCompanyDetails(Models.SaveCompanyDetailsRequest model, string token, long userId,
                                                  out bool isCompanyExists,
                                                  out string result,
                                                  out string errorMsg)
        {
            var response = 1;

            companyRepository.BeginTransaction();

            try
            {
                long companyId;
                string companyGuid = string.Empty;
                bool isDuplicateCompany = false;
                string savedResult = string.Empty;
                string validationErrorMsg = string.Empty;

                // Get Company Details
                var companyDetails = companyRepository.GetCompanyByName(model.CompanyDetails.CompanyName, model.ExistingCompanyGuid);
                if (companyDetails != null)
                {
                    isDuplicateCompany = true;
                    savedResult = "";
                    validationErrorMsg = "";
                }
                else
                {
                    // Save Update Company Details
                    SaveUpdateCompanyDetails(model, token, userId, out companyId, out companyGuid);

                    // Map Assets to Company
                    if (model.AssetsWorkWith != null &&
                        model.AssetsWorkWith.Count() > 0)
                    {
                        MapBrokerAssetsToCompany(model.AssetsWorkWith, companyId);
                    }

                    // Map Lenders to Company
                    if (model.LendersWorkWith != null &&
                        model.LendersWorkWith.Count() > 0)
                    {
                        MapLendersToCompany(model.LendersWorkWith, companyId);
                    }

                    // Save Broker Users
                    List<SaveUsersRequest> brokersList = null;
                    using (var client = new HttpClient())
                    {
                        var requestUri = "api/v1/Broker/saveusers";
                        var userApiUrl = Startup.AppConfiguration.GetSection("BaseURL").GetSection("UserApiURL").Value;
                        var brokerUserRequest = new BrokerUserRequest();
                        brokerUserRequest.CompanyGuid = companyGuid;
                        brokerUserRequest.SkipRegistrationMail = model.SkipRegistrationMail;
                        brokerUserRequest.BrokerUsers = model.BrokerUsers;
                        var responseData = ExtensionMethods<BrokerUserRequest>
                                           .PostJsonDatas(client, userApiUrl, requestUri, token, brokerUserRequest)
                                           .Result;
                        // Deserialize json data to Class
                        var apiPostResponseData = JsonSerializer.Deserialize<ApiPostResponseData>(responseData,
                            new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            });
                        if (!string.IsNullOrEmpty(apiPostResponseData.ErrorMessage))
                        {
                            companyRepository.RollbackTransaction();

                            validationErrorMsg = apiPostResponseData.ErrorMessage;

                            response = -1;
                        }
                        else
                        {
                            // Deserialize json data to List
                            brokersList = JsonSerializer.Deserialize<List<SaveUsersRequest>>(apiPostResponseData.Message.ToString(),
                                 new JsonSerializerOptions
                                 {
                                     PropertyNameCaseInsensitive = true
                                 });
                        }
                    }
                    if (brokersList != null &&
                        brokersList.Count() > 0)
                    {
                        using (var client = new HttpClient())
                        {
                            var requestUri = "api/v1/Data/saveusers";
                            var coreApiUrl = Startup.AppConfiguration.GetSection("BaseURL").GetSection("CoreApiURL").Value;
                            var responseData = ExtensionMethods<List<SaveUsersRequest>>
                                               .PostJsonDatas(client, coreApiUrl, requestUri, token, brokersList)
                                               .Result;
                            // Deserialize json data to Class
                            var apiPostResponseData = JsonSerializer.Deserialize<ApiPostResponseData>(responseData,
                            new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            });
                            if (!string.IsNullOrEmpty(apiPostResponseData.ErrorMessage))
                            {
                                companyRepository.RollbackTransaction();
                            }
                        }

                        companyRepository.CommitTransaction();

                        dataRepository.SaveUsers(brokersList);

                        isDuplicateCompany = false;
                        savedResult = companyGuid;
                        validationErrorMsg = "";
                    }
                }

                isCompanyExists = isDuplicateCompany;
                result = savedResult;
                errorMsg = validationErrorMsg;

                return response;
            }
            catch
            {
                companyRepository.RollbackTransaction();

                throw;
            }
        }

        #region Save Update Company Details

        private void SaveUpdateCompanyDetails(Models.SaveCompanyDetailsRequest model, string token, long userId,
                                              out long companyId, out string companyGuid)
        {
            if (model.IsNewCompany)
            {
                // Save Company Details
                Companies company = new Companies();
                company.CompanyGuid = ExtensionMethods.GetNewGuid();
                company.CompanyName = model.CompanyDetails.CompanyName;
                company.RegisteredName = model.CompanyDetails.RegisteredName;
                company.ABN = model.CompanyDetails.ABN;
                company.CompanyAddress = model.CompanyDetails.CompanyAddress;
                company.City = model.CompanyDetails.City;
                company.State = model.CompanyDetails.State;
                company.ZIPCode = model.CompanyDetails.ZIPCode;
                company.Website = model.CompanyDetails.Website;
                company.Email = model.CompanyDetails.Email;
                company.ExcemptPayment = model.CompanyDetails.ExcemptPayment;
                company.AllowOnlyInvitedUser = model.CompanyDetails.AllowOnlyInvitedUser;
                company.CompanyTypeId = (int)Enums.CompanyTypes.Broker;
                // Upload Blob to Azure Storage Container
                if (!string.IsNullOrEmpty(model.CompanyLogo))
                {
                    var blobName = string.Empty;
                    azureBlobHelper.UploadBlobToAzureStorageContainer(model.CompanyLogo, company.CompanyGuid, ref blobName);
                    company.CompanyLogoURL = blobName;
                }
                else
                {
                    company.CompanyLogoURL = string.Empty;
                }
                company.CompanyDescription = model.CompanyDescription;
                company.IsUseCompanyLogoForWebApp = false;
                company.CreatedBy = userId;
                company.CreatedTime = DateTime.UtcNow;
                companyRepository.SaveCompanyDetails(company);
                SaveUpdateCompanyDetails(company, token);

                companyId = company.Id;
                companyGuid = company.CompanyGuid;
            }
            else
            {
                // Update Company Details
                var companyDetails = companyRepository.GetCompanyByGuid(model.ExistingCompanyGuid);
                companyDetails.CompanyName = model.CompanyDetails.CompanyName;
                companyDetails.RegisteredName = model.CompanyDetails.RegisteredName;
                companyDetails.ABN = model.CompanyDetails.ABN;
                companyDetails.CompanyAddress = model.CompanyDetails.CompanyAddress;
                companyDetails.City = model.CompanyDetails.City;
                companyDetails.State = model.CompanyDetails.State;
                companyDetails.ZIPCode = model.CompanyDetails.ZIPCode;
                companyDetails.Website = model.CompanyDetails.Website;
                companyDetails.Email = model.CompanyDetails.Email;
                companyDetails.ExcemptPayment = model.CompanyDetails.ExcemptPayment;
                companyDetails.AllowOnlyInvitedUser = model.CompanyDetails.AllowOnlyInvitedUser;
                // Upload Blob to Azure Storage Container
                if (!string.IsNullOrEmpty(model.CompanyLogo))
                {
                    var blobName = string.Empty;
                    azureBlobHelper.UploadBlobToAzureStorageContainer(model.CompanyLogo, companyDetails.CompanyGuid, ref blobName);
                    companyDetails.CompanyLogoURL = blobName;
                }
                else
                {
                    companyDetails.CompanyLogoURL = string.Empty;
                }
                companyDetails.CompanyDescription = model.CompanyDescription;
                companyDetails.UpdatedBy = userId;
                companyDetails.UpdatedTime = DateTime.UtcNow;
                companyRepository.UpdateCompanyDetails(companyDetails);
                SaveUpdateCompanyDetails(companyDetails, token);

                companyId = companyDetails.Id;
                companyGuid = companyDetails.CompanyGuid;
            }
        }

        #endregion

        #endregion

        #region Get Admin Company Details

        public Models.SaveCompanyDetailsResponse GetAdminCompanyDetails(Companies companyDetails, string token)
        {
            var response = new Models.SaveCompanyDetailsResponse();

            // Get Company Details
            response.CompanyId = companyDetails.Id;
            response.CompanyName = companyDetails.CompanyName;
            response.RegisteredName = companyDetails.RegisteredName;
            response.ABN = companyDetails.ABN != "Blank"
                         ? companyDetails.ABN
                         : "";
            response.CompanyAddress = companyDetails.CompanyAddress != "Blank"
                                    ? companyDetails.CompanyAddress
                                    : "";
            response.ZipCode = companyDetails.ZIPCode != "Blank"
                             ? companyDetails.ZIPCode
                             : "";
            response.State = companyDetails.State != "Blank"
                           ? companyDetails.State
                           : "";
            response.City = companyDetails.City != "Blank"
                          ? companyDetails.City
                          : "";
            response.Website = companyDetails.Website;
            response.Email = companyDetails.Email;
            response.LiveStatus = "Yes";
            response.ExcemptPayment = companyDetails.ExcemptPayment;
            response.AllowOnlyInvitedUser = companyDetails.AllowOnlyInvitedUser;

            // Get Blob from Azure Storage Container
            if (!string.IsNullOrEmpty(companyDetails.CompanyLogoURL))
            {
                var base64Text = string.Empty;
                var imageProps = new ImageProperties();
                azureBlobHelper.GetBlobFromAzureStorageContainer(companyDetails.CompanyGuid, ref base64Text, out imageProps);
                response.CompanyLogo = base64Text;
            }
            else
            {
                response.CompanyLogo = null;
            }

            response.CompanyDescription = companyDetails.CompanyDescription;

            // Get Lenders Mapped to Company
            response.Lenders = GetLendersWorkWith(companyDetails.Id);

            // Get Assets Mapped To Broker
            response.Assets = GetAssetsMappedToBroker(companyDetails.Id);

            using (var client = new HttpClient())
            {
                var requestUri = "api/v1/Broker/getusers/" + companyDetails.CompanyGuid;
                var userApiUrl = Startup.AppConfiguration.GetSection("BaseURL").GetSection("UserApiURL").Value;
                var responseData = ExtensionMethods<List<BrokerUserResponse>>
                                   .GetDeserializedData(client, userApiUrl, requestUri, token)
                                   .Result;

                response.BrokerUsers = responseData;
            }

            return response;
        }

        #endregion

        #region Save Broker Admin Company Details

        public void SaveBrokerAdminCompanyDetails(BrokerAdminCompanyRequest model, string companyGuid, string token, long userId)
        {
            // Get Company Details by CompanyGuid
            var companyDetails = companyRepository.GetCompanyByGuid(companyGuid);
            if (companyDetails != null)
            {
                // Get Company Details
                companyDetails.CompanyName = model.CompanyName;
                companyDetails.RegisteredName = model.RegisteredCompanyName;
                companyDetails.ABN = model.ABN;
                companyDetails.CompanyAddress = model.CompanyAddress;
                companyDetails.ZIPCode = model.ZipCode;
                companyDetails.State = model.State;
                companyDetails.City = model.City;
                companyDetails.Website = model.Website;
                companyDetails.Email = model.Email;
                companyDetails.CompanyDescription = model.CompanyDescription;
                companyDetails.UpdatedBy = userId;
                companyDetails.UpdatedTime = DateTime.UtcNow;

                // Upload Blob to Azure Storage Container
                if (!string.IsNullOrEmpty(model.CompanyLogo))
                {
                    var blobName = string.Empty;
                    azureBlobHelper.UploadBlobToAzureStorageContainer(model.CompanyLogo, companyDetails.CompanyGuid, ref blobName);
                    companyDetails.CompanyLogoURL = blobName;
                }
                else
                {
                    companyDetails.CompanyLogoURL = string.Empty;
                }

                // Update Company Datas
                companyRepository.UpdateCompanyDatas(companyDetails);
                SaveUpdateCompanyDetails(companyDetails, token);

                if (model.Lenders.Count() > 0)
                {
                    // Remove BrokerLenderMappings
                    companyRepository.RemoveBrokerLenderMappings(companyDetails.Id);

                    // Add BrokerLenderMappings
                    var companies = companyRepository.GetLenderCompanies();
                    var mappedLenders = model.Lenders.Where(lender => lender.IsMapped == true).ToList();
                    mappedLenders.ForEach(
                        lender =>
                        {
                            var lenderDetail = companies.FirstOrDefault(l => l.CompanyGuid == lender.LenderGUID);
                            if (lenderDetail != null)
                            {
                                companyRepository.AddBrokerLenderMappings(companyDetails.Id, lenderDetail.Id);
                            }
                        });
                }

                // Map Assets to Company
                if (model.Assets != null &&
                    model.Assets.Count() > 0)
                {
                    // Save Assets
                    SaveAssetsMappedToBroker(model.Assets, companyDetails.Id);
                }
            }

            companyRepository.SaveDbChanges();
        }

        #endregion

        #region Get Broker Admin Company Details

        public BrokerAdminCompanyResponse GetBrokerAdminCompanyDetails(Companies companyDetails)
        {
            var response = new BrokerAdminCompanyResponse();

            // Get Company Details
            response.CompanyName = companyDetails.CompanyName;
            response.RegisteredCompanyName = companyDetails.RegisteredName;
            response.ABN = companyDetails.ABN;
            response.CompanyAddress = companyDetails.CompanyAddress;
            response.ZipCode = companyDetails.ZIPCode;
            response.State = companyDetails.State;
            response.City = companyDetails.City;
            response.Website = companyDetails.Website;
            response.Email = companyDetails.Email;

            // Get Blob from Azure Storage Container
            if (!string.IsNullOrEmpty(companyDetails.CompanyLogoURL))
            {
                var base64Text = string.Empty;
                var imageProps = new ImageProperties();
                azureBlobHelper.GetBlobFromAzureStorageContainer(companyDetails.CompanyGuid, ref base64Text, out imageProps);
                response.CompanyLogo = base64Text;
            }
            else
            {
                response.CompanyLogo = null;
            }

            response.CompanyDescription = companyDetails.CompanyDescription;

            // Get Lenders Mapped to Company
            response.Lenders = GetLendersWorkWith(companyDetails.Id);

            // Get Assets Mapped To Broker
            response.Assets = GetAssetsMappedToBroker(companyDetails.Id);

            return response;
        }

        #endregion

        #region Save Lender Company Details

        public void SaveLenderCompanyDetails(Companies companyDetails, Models.Lender.LenderCompanyRequest model,
                                             string lenderCompanyGuid, string token,
                                             long userId, out string errorMessage)
        {
            companyRepository.BeginTransaction();

            try
            {
                var blobName = string.Empty;
                errorMessage = "";
                object infoMessage = null;

                // Lender Information
                companyDetails.CompanyName = model.LenderName;
                companyDetails.RegisteredName = model.LenderCompanyName;
                companyDetails.ABN = model.ABN;
                companyDetails.CompanyAddress = model.CompanyAddress;
                companyDetails.ZIPCode = model.ZipCode;
                companyDetails.State = model.State;
                companyDetails.City = model.City;
                companyDetails.Website = model.Website;
                companyDetails.Email = model.Email;
                companyDetails.CompanyDescription = model.CompanyDescription;
                companyDetails.UpdatedBy = userId;
                companyDetails.UpdatedTime = DateTime.UtcNow;

                // Add Lender Configuration
                using (var client = new HttpClient())
                {
                    var requestUri = "api/v1/LenderConfiguration/savelenderdetails";
                    var coreApiUrl = Startup.AppConfiguration.GetSection("BaseURL").GetSection("CoreApiURL").Value;
                    model.LenderConfigurations.LenderCompanyGuid = lenderCompanyGuid;
                    var responseData = ExtensionMethods<Models.Lender.LenderConfigurations>
                                       .PostJsonDatas(client, coreApiUrl, requestUri, token, model.LenderConfigurations)
                                       .Result;
                    // Deserialize json data to Class
                    var apiPostResponseData = JsonSerializer.Deserialize<ApiPostResponseData>(responseData,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    errorMessage = apiPostResponseData.ErrorMessage;
                    infoMessage = apiPostResponseData.Message;
                }

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    companyRepository.RollbackTransaction();
                }
                else
                {
                    if (infoMessage != null)
                    {
                        if (Convert.ToString(infoMessage) == CoreMessages.LenderConfigurations_Invalid)
                        {
                            errorMessage = CoreMessages.LenderConfigurations_Invalid;

                            companyRepository.RollbackTransaction();
                        }
                    }
                    else
                    {
                        // Report Customisation
                        // Upload Blob to Azure Storage Container
                        if (!string.IsNullOrEmpty(model.CompanyLogo))
                        {
                            azureBlobHelper.UploadBlobToAzureStorageContainer(model.CompanyLogo, companyDetails.CompanyGuid, ref blobName);
                            companyDetails.CompanyLogoURL = blobName;
                        }
                        // Company Description
                        companyDetails.CompanyDescription = model.CompanyDescription;

                        // Save Company Details
                        companyRepository.UpdateCompanyDetails(companyDetails);
                        SaveUpdateCompanyDetails(companyDetails, token);

                        // Company Contacts
                        SaveCompanyContacts(model.CompanyContacts, companyDetails.Id);

                        // Assets Mapped To Lender
                        SaveAssetsMappedToLender(model.Assets, companyDetails.Id);

                        errorMessage = "";

                        companyRepository.CommitTransaction();
                    }
                }
            }
            catch
            {
                companyRepository.RollbackTransaction();

                throw;
            }
        }

        #endregion

        #region Get Lender Company Details

        public Models.Lender.LenderCompanyResponse GetLenderCompanyDetails(Companies companyDetails, string lenderCompanyGuid, string token)
        {
            var response = new Models.Lender.LenderCompanyResponse();

            // Lender Information
            response.LenderId = companyDetails.Id;
            response.LenderName = companyDetails.CompanyName;
            response.LenderCompanyName = companyDetails.RegisteredName;
            response.LenderCompanyGuid = companyDetails.CompanyGuid;
            response.ABN = companyDetails.ABN;
            response.CompanyAddress = companyDetails.CompanyAddress;
            response.ZipCode = companyDetails.ZIPCode;
            response.State = companyDetails.State;
            response.City = companyDetails.City;
            response.Website = companyDetails.Website;
            response.Email = companyDetails.Email;

            // Report Customisation
            // Get Blob from Azure Storage Container
            if (!string.IsNullOrEmpty(companyDetails.CompanyLogoURL))
            {
                var base64Text = string.Empty;
                var imageProps = new ImageProperties();
                azureBlobHelper.GetBlobFromAzureStorageContainer(companyDetails.CompanyGuid, ref base64Text, out imageProps);
                response.CompanyLogo = base64Text;
            }
            else
            {
                response.CompanyLogo = null;
            }
            // Company Description
            response.CompanyDescription = companyDetails.CompanyDescription;

            // Company Contacts
            response.CompanyContacts = GetCompanyContacts(companyDetails.Id);

            // Assets Mapped To Lender
            response.Assets = GetAssetsMappedToLender(companyDetails.Id);

            // Lender Configuration
            using (var client = new HttpClient())
            {
                var requestUri = "api/v1/LenderConfiguration/getlenderdetails/" + lenderCompanyGuid;
                var coreApiUrl = Startup.AppConfiguration.GetSection("BaseURL").GetSection("CoreApiURL").Value;
                var lenderConfigurations = ExtensionMethods<Models.Lender.LenderConfigurations>
                                           .GetDeserializedData(client, coreApiUrl, requestUri, token)
                                           .Result;
                if (lenderConfigurations != null)
                {
                    lenderConfigurations.LenderCompanyGuid = lenderCompanyGuid;
                }
                response.LenderConfigurations = lenderConfigurations;
            }

            return response;
        }

        #endregion

        #region Get Lender Onboard Details

        public Models.Lender.OnboardDetailsResponse GetOnboardDetails()
        {
            var response = new Models.Lender.OnboardDetailsResponse();

            // Get States
            response.States = companyRepository.GetStateoption();

            // Get Assets
            response.Assets = companyRepository.GetAllAssets();

            return response;
        }

        #endregion

        #region Lender Onboard-Save Company Details

        public void OnboardLender(Models.Lender.LenderRegisterRequest model, long userId, string token, out string companyGuid, out string result)
        {
            companyRepository.BeginTransaction();

            try
            {
                companyGuid = "";
                result = "";
                var errorMessage = "";
                object infoMessage = null;

                // Save Company Details
                var newCompanyDetail = model.NewCompanyDetail;
                var company = new Companies();
                company.CompanyGuid = ExtensionMethods.GetNewGuid();
                company.CompanyName = newCompanyDetail.CompanyName;
                company.RegisteredName = newCompanyDetail.RegisteredName;
                company.ABN = newCompanyDetail.ABN;
                company.CompanyAddress = newCompanyDetail.CompanyAddress;
                company.City = newCompanyDetail.City;
                company.State = newCompanyDetail.State;
                company.ZIPCode = newCompanyDetail.ZIPCode;
                company.Website = newCompanyDetail.Website;
                company.Email = newCompanyDetail.Email;
                company.CompanyTypeId = (int)Enums.CompanyTypes.Lender;
                company.CreatedBy = userId;
                company.CreatedTime = DateTime.UtcNow;
                company.IsUseCompanyLogoForWebApp = false;
                companyRepository.SaveCompanyDetails(company);
                companyRepository.SaveDbChanges();

                if (company.Id > 0)
                {
                    // Map Asssets to Company
                    if (model.AssetsWorkWith != null &&
                        model.AssetsWorkWith.Count() > 0)
                    {
                        MapLenderAssetsToCompany(model.AssetsWorkWith, company.Id);
                    }

                    // Add Primary Contact
                    var primaryContact = model.PrimaryContact;
                    var companyContact = new Models.CompanyContacts();
                    companyContact.Name = primaryContact.Name;
                    companyContact.SurName = primaryContact.SurName;
                    companyContact.Email = primaryContact.Email;
                    companyContact.Mobile = primaryContact.Mobile;
                    companyContact.CompanyContactTypeId = (int)Enums.CompanyContactTypes.PrimaryContact;
                    SaveCompanyContacts(companyContact, company.Id);

                    // Save ADCompanies
                    using (var client = new HttpClient())
                    {
                        var companyDetailsRequest = new Models.Data.SaveCompanyDetailsRequest();
                        companyDetailsRequest.CompanyGuid = company.CompanyGuid;
                        companyDetailsRequest.CompanyName = company.CompanyName;
                        companyDetailsRequest.CompanyTypeId = (int)Common.Models.Enums.CompanyTypes.Lender;
                        companyDetailsRequest.IsPayer = false;
                        companyDetailsRequest.ExcemptPayment = false;

                        var requestUri = "api/v1/Data/savecompanydetails";
                        var coreApiUrl = Startup.AppConfiguration.GetSection("BaseURL").GetSection("CoreApiURL").Value;
                        var responseData = ExtensionMethods<Models.Data.SaveCompanyDetailsRequest>
                                           .PostJsonDatas(client, coreApiUrl, requestUri, token, companyDetailsRequest)
                                           .Result;
                        // Deserialize json data to Class
                        var apiPostResponseData = JsonSerializer.Deserialize<ApiPostResponseData>(responseData,
                            new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            });

                        errorMessage = apiPostResponseData.ErrorMessage;
                    }

                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        companyRepository.RollbackTransaction();
                    }
                    else
                    {
                        // Add Lender Configuration
                        using (var client = new HttpClient())
                        {
                            var requestUri = "api/v1/LenderConfiguration/savelenderdetails";
                            var coreApiUrl = Startup.AppConfiguration.GetSection("BaseURL").GetSection("CoreApiURL").Value;
                            var lenderConfigurations = new Models.Lender.LenderConfiguration();
                            lenderConfigurations.LenderCompanyGuid = company.CompanyGuid;
                            lenderConfigurations.LenderRefPrefix = "";
                            lenderConfigurations.IsReportRequired = false;
                            lenderConfigurations.ReportEmailAddress = "";
                            lenderConfigurations.IsBSAllowed = false;
                            lenderConfigurations.IsNonOwnerAllowed = false;
                            lenderConfigurations.IsAllowAwaitedRef = false;
                            var responseData = ExtensionMethods<Models.Lender.LenderConfiguration>
                                               .PostJsonDatas(client, coreApiUrl, requestUri, token, lenderConfigurations)
                                               .Result;
                            // Deserialize json data to Class
                            var apiPostResponseData = JsonSerializer.Deserialize<ApiPostResponseData>(responseData,
                                new JsonSerializerOptions
                                {
                                    PropertyNameCaseInsensitive = true
                                });

                            errorMessage = apiPostResponseData.ErrorMessage;
                            infoMessage = apiPostResponseData.Message;
                        }

                        if (!string.IsNullOrEmpty(errorMessage))
                        {
                            result = errorMessage;

                            companyRepository.RollbackTransaction();
                        }
                        else
                        {
                            if (infoMessage != null)
                            {
                                if (Convert.ToString(infoMessage) == CoreMessages.LenderConfigurations_Invalid)
                                {
                                    result = CoreMessages.LenderConfigurations_Invalid;

                                    companyRepository.RollbackTransaction();
                                }
                            }
                            else
                            {
                                companyRepository.CommitTransaction();

                                companyGuid = company.CompanyGuid;
                                result = "";
                            }
                        }
                    }
                }
            }
            catch
            {
                companyRepository.RollbackTransaction();

                throw;
            }
        }

        #endregion

        #region Map Broker Assets to Company

        private void MapBrokerAssetsToCompany(List<string> assetsWorkWith, long companyId)
        {
            // Get Mapped TemplateSets
            var newTemplateSets = companyRepository.GetMappedTemplateSets(assetsWorkWith);

            // Remove BrokerTemplateMappings
            companyRepository.RemoveBrokerTemplateMappings(companyId);

            // Add BrokerTemplateMappings
            foreach (var item in newTemplateSets)
            {
                companyRepository.AddBrokerTemplateMappings(companyId, item);
            }

            companyRepository.SaveDbChanges();
        }

        #endregion

        #region Map Lender Assets to Company

        private void MapLenderAssetsToCompany(List<string> assetsWorkWith, long companyId)
        {
            // Get Mapped TemplateSets
            var newTemplateSets = companyRepository.GetMappedTemplateSets(assetsWorkWith);

            // Remove LenderTemplateMappings
            companyRepository.RemoveLenderTemplateMappings(companyId);

            // Add LenderTemplateMappings
            foreach (var item in newTemplateSets)
            {
                companyRepository.AddLenderTemplateMappings(companyId, item);
            }

            companyRepository.SaveDbChanges();
        }

        #endregion

        #region Map Lenders to Company

        private void MapLendersToCompany(List<string> lendersWorkWith, long companyId)
        {
            // Get Mapped Lenders
            var lenders = companyRepository.GetMappedLenders(lendersWorkWith);

            // Remove BrokerLenderMappings
            companyRepository.RemoveBrokerLenderMappings(companyId);

            // Add BrokerLenderMappings
            foreach (var item in lenders)
            {
                companyRepository.AddBrokerLenderMappings(companyId, item);
            }

            companyRepository.SaveDbChanges();
        }

        #endregion

        #region Get Company By Name

        public Companies GetCompanyByName(string name, string companyGuid)
        {
            // Get Company By Name
            var response = companyRepository.GetCompanyByName(name, companyGuid);

            return response;
        }

        #endregion

        #region Get State Option

        public List<StateOptionsResponse> GetStateoption()
        {
            // Get State Option
            var responses = companyRepository.GetStateoption();

            return responses;
        }

        #endregion

        #region Get Lenders Work With

        public List<LendersWorkWithResponse> GetLendersWorkWith(long companyId)
        {
            var responses = new List<LendersWorkWithResponse>();

            // Get Lender Visibility Mappings
            var lenderVisiblityMappings = companyRepository.GetLenderVisibilityMappings(companyId);

            // Get Lenders Work With
            var lenderDetails = companyRepository.GetBrokerLenderMappings(companyId);
            var lenders = companyRepository.GetLenderCompanies()
                            .Where(c => c.LenderVisibility != (int)Enums.LenderVisibilities.Self)
                            .ToList();
            foreach (var lender in lenders)
            {
                if (lender.LenderVisibility == (int)Enums.LenderVisibilities.Selective &&
                    lenderVisiblityMappings.Count(lvm => lvm.LenderCompanyId == lender.Id) == 0)
                {
                    continue;
                }

                var mappedLender = lenderDetails.FirstOrDefault(l => l.LenderCompanyId == lender.Id);
                responses.Add(
                    new LendersWorkWithResponse()
                    {
                        LenderName = lender.CompanyName,
                        LenderGUID = lender.CompanyGuid,
                        IsPayer = lender.IsPayer,
                        IsMapped = (mappedLender != null)
                                 ? true
                                 : false
                    });
            }

            return responses;
        }

        #endregion

        #region Get Assets Work With

        public List<AssetsWorkWithResponse> GetAssetsWorkWith(long companyId, int userTypeId)
        {
            List<AssetsWorkWithResponse> responses = null;

            // If UserType is broker Get Assets Mapped to Broker
            // Else Get Assets Mapped to Lender
            if (userTypeId == (int)Common.Models.Enums.UserTypes.Broker)
            {
                responses = GetAssetsMappedToBroker(companyId);
            }
            else if (userTypeId == (int)Common.Models.Enums.UserTypes.Lender)
            {
                responses = GetAssetsMappedToLender(companyId);
            }

            return responses;
        }

        #endregion

        #region Get Lenders List

        public List<LendersListResponse> GetLendersList(Models.LendersListRequest model)
        {
            var responses = new List<LendersListResponse>();

            // Get Lenders List
            var lendersList = companyRepository.GetLenderCompanies();
            if (lendersList != null &&
                lendersList.Count() > 0)
            {
                lendersList.ForEach(
                    lender =>
                    {
                        responses.Add(
                            new LendersListResponse()
                            {
                                LenderId = lender.Id,
                                LenderName = lender.CompanyName,
                                ABN = lender.ABN,
                                Website = lender.Website,
                                Email = lender.Email,
                                LiveStatus = "Yes"
                            });
                    });
            }
            if (!string.IsNullOrEmpty(model.SortColumn) &&
                !string.IsNullOrEmpty(model.SortDirection))
            {
                responses = responses
                            .AsQueryable()
                            .Sort(model.SortColumn, model.SortDirection)
                            .ToDynamicList<LendersListResponse>();
            }

            return responses;
        }

        #endregion

        #region Get User Profile Company

        public UserProfileCompanyResponse GetUserProfileCompany(Entities.Companies companyDetails)
        {
            var response = new UserProfileCompanyResponse();

            response.CompanyName = companyDetails.CompanyName;
            response.RegisteredCompanyName = companyDetails.RegisteredName;
            response.ABN = companyDetails.ABN;
            response.CompanyAddress = companyDetails.CompanyAddress;
            response.ZipCode = companyDetails.ZIPCode;
            response.State = companyDetails.State;
            response.City = companyDetails.City;
            response.Website = companyDetails.Website;
            response.Email = companyDetails.Email;

            return response;
        }

        #endregion

        #region Save User Profile Company

        public void SaveUserProfileCompany(UserProfileCompanyRequest model, string companyGuid, long userId)
        {
            // Get Company Details by CompanyGuid
            var companyDetails = companyRepository.GetCompanyByGuid(companyGuid);
            if (companyDetails != null)
            {
                // Save User Profile Company
                companyDetails.CompanyName = model.CompanyName;
                companyDetails.RegisteredName = model.RegisteredCompanyName;
                companyDetails.ABN = model.ABN;
                companyDetails.CompanyAddress = model.CompanyAddress;
                companyDetails.ZIPCode = model.ZipCode;
                companyDetails.State = model.State;
                companyDetails.City = model.City;
                companyDetails.Website = model.Website;
                companyDetails.Email = model.Email;
                companyDetails.UpdatedBy = userId;
                companyDetails.UpdatedTime = DateTime.UtcNow;
                companyRepository.UpdateCompanyDetails(companyDetails);
            }
        }

        #endregion

        #region Get Lender Details

        public LenderCompanyResponse GetLenderDetails(string lenderCompanyGuid, string token)
        {
            var response = new LenderCompanyResponse();

            var lenderDetails = companyRepository.GetCompanyByGuid(lenderCompanyGuid);
            if (lenderDetails != null)
            {
                // Lender Information
                response.LenderId = lenderDetails.Id;
                response.LenderName = lenderDetails.CompanyName;
                response.LenderCompanyName = lenderDetails.RegisteredName;
                response.LenderCompanyGuid = lenderDetails.CompanyGuid;
                response.ABN = lenderDetails.ABN;
                response.CompanyAddress = lenderDetails.CompanyAddress;
                response.ZipCode = lenderDetails.ZIPCode;
                response.State = lenderDetails.State;
                response.City = lenderDetails.City;
                response.Website = lenderDetails.Website;
                response.Email = lenderDetails.Email;
                response.IsPayer = lenderDetails.IsPayer;
                response.LiveStatus = "Yes";

                // Legal Details
                response.ContractLocation = lenderDetails.ContractLocation;
                response.SignDate = lenderDetails.SignDate;
                response.GoLiveDate = lenderDetails.GoLiveDate;

                // Visibility
                response.LenderVisibility = lenderDetails.LenderVisibility;

                // Report Customisation
                // Get Blob from Azure Storage Container
                if (!string.IsNullOrEmpty(lenderDetails.CompanyLogoURL))
                {
                    var base64Text = string.Empty;
                    var imageProps = new ImageProperties();
                    azureBlobHelper.GetBlobFromAzureStorageContainer(lenderDetails.CompanyGuid, ref base64Text, out imageProps);
                    response.CompanyLogo = base64Text;
                }
                else
                {
                    response.CompanyLogo = null;
                }
                // Company Description
                response.CompanyDescription = lenderDetails.CompanyDescription;

                // Company Contacts
                response.CompanyContacts = GetCompanyContacts(lenderDetails.Id);

                // Get Assets Mapped To Broker
                response.Assets = GetAssetsMappedToBroker(lenderDetails.Id);

                // Lender Configuration
                using (var client = new HttpClient())
                {
                    var requestUri = "api/v1/LenderConfiguration/getlenderdetails/" + lenderCompanyGuid;
                    var coreApiUrl = Startup.AppConfiguration.GetSection("BaseURL").GetSection("CoreApiURL").Value;
                    var lenderConfigurations = ExtensionMethods<Models.LenderConfigurations>
                                               .GetDeserializedData(client, coreApiUrl, requestUri, token)
                                               .Result;
                    if (lenderConfigurations != null)
                    {
                        lenderConfigurations.LenderCompanyGuid = lenderCompanyGuid;
                    }
                    response.LenderConfigurations = lenderConfigurations;
                }
            }

            return response;
        }

        #endregion

        #region Get Lender By Name

        public Companies GetLenderByName(string lenderName, string lenderCompanyGuid)
        {
            // Get Lender By Name
            var response = companyRepository.GetLenderByName(lenderName, lenderCompanyGuid);

            return response;
        }

        #endregion

        #region Save Lender Details

        public void SaveLenderDetails(LenderCompanyRequest model, long createdBy, string token,
                                      ref string lenderCompanyGuid, out string errorMessage)
        {
            companyRepository.BeginTransaction();

            try
            {
                var blobName = string.Empty;
                errorMessage = "";
                object infoMessage = null;

                if (string.IsNullOrEmpty(model.LenderCompanyGuid))
                {
                    // New Lender information
                    var newLender = new Companies();
                    newLender.CompanyName = model.LenderName;
                    newLender.RegisteredName = model.LenderCompanyName;
                    newLender.ABN = model.ABN;
                    newLender.CompanyAddress = model.CompanyAddress;
                    newLender.ZIPCode = model.ZipCode;
                    newLender.State = model.State;
                    newLender.City = model.City;
                    newLender.Website = model.Website;
                    newLender.Email = model.Email;
                    newLender.CompanyTypeId = (int)Enums.CompanyTypes.Lender;
                    newLender.CompanyGuid = ExtensionMethods.GetNewGuid();
                    newLender.CreatedBy = createdBy;
                    newLender.CreatedTime = DateTime.UtcNow;

                    // Legal details
                    newLender.ContractLocation = model.ContractLocation;
                    newLender.SignDate = !string.IsNullOrEmpty(model.SignDate)
                                       ? DateTime.ParseExact(model.SignDate, "dd-MM-yyyy", null)
                                       : null;
                    newLender.GoLiveDate = !string.IsNullOrEmpty(model.GoLiveDate)
                                         ? DateTime.ParseExact(model.GoLiveDate, "dd-MM-yyyy", null)
                                         : null;

                    // Visibility
                    newLender.LenderVisibility = model.LenderVisibility;

                    // Report Customisation
                    // Upload Blob to Azure Storage Container
                    if (!string.IsNullOrEmpty(model.CompanyLogo))
                    {
                        azureBlobHelper.UploadBlobToAzureStorageContainer(model.CompanyLogo, newLender.CompanyGuid, ref blobName);
                        newLender.CompanyLogoURL = blobName;
                    }
                    // Company Description
                    newLender.CompanyDescription = model.CompanyDescription;

                    newLender.IsUseCompanyLogoForWebApp = false;

                    // Save Company Details
                    companyRepository.SaveCompanyDetails(newLender);
                    SaveUpdateCompanyDetails(newLender, token);

                    // Company Contacts
                    SaveCompanyContacts(model.CompanyContacts, newLender.Id);

                    // Assets
                    SaveAssetsMappedToBroker(model.Assets, newLender.Id);

                    lenderCompanyGuid = newLender.CompanyGuid;
                }
                else
                {
                    // Existing Lender Information
                    var lenderDetails = companyRepository.GetCompanyByGuid(model.LenderCompanyGuid);
                    lenderDetails.CompanyName = model.LenderName;
                    lenderDetails.RegisteredName = model.LenderCompanyName;
                    lenderDetails.ABN = model.ABN;
                    lenderDetails.CompanyAddress = model.CompanyAddress;
                    lenderDetails.ZIPCode = model.ZipCode;
                    lenderDetails.State = model.State;
                    lenderDetails.City = model.City;
                    lenderDetails.Website = model.Website;
                    lenderDetails.Email = model.Email;
                    lenderDetails.UpdatedTime = DateTime.Now;
                    lenderDetails.UpdatedBy = createdBy;

                    // Legal Details
                    lenderDetails.ContractLocation = model.ContractLocation;
                    lenderDetails.SignDate = !string.IsNullOrEmpty(model.SignDate)
                                           ? DateTime.ParseExact(model.SignDate, "dd-MM-yyyy", null)
                                           : null;
                    lenderDetails.GoLiveDate = !string.IsNullOrEmpty(model.GoLiveDate)
                                             ? DateTime.ParseExact(model.GoLiveDate, "dd-MM-yyyy", null)
                                             : null;

                    // Visibility
                    lenderDetails.LenderVisibility = model.LenderVisibility;

                    // Report Customisation
                    // Upload Blob to Azure Storage Container
                    if (!string.IsNullOrEmpty(model.CompanyLogo))
                    {
                        azureBlobHelper.UploadBlobToAzureStorageContainer(model.CompanyLogo, lenderDetails.CompanyGuid, ref blobName);
                        lenderDetails.CompanyLogoURL = blobName;
                    }
                    // Company Description
                    lenderDetails.CompanyDescription = model.CompanyDescription;

                    // Update Company Details
                    companyRepository.UpdateCompanyDetails(lenderDetails);
                    SaveUpdateCompanyDetails(lenderDetails, token);

                    // Company Contacts
                    SaveCompanyContacts(model.CompanyContacts, lenderDetails.Id);

                    // Assets
                    SaveAssetsMappedToBroker(model.Assets, lenderDetails.Id);

                    lenderCompanyGuid = lenderDetails.CompanyGuid;
                }

                // Lender Configuration
                using (var client = new HttpClient())
                {
                    var requestUri = "api/v1/LenderConfiguration/savelenderdetails";
                    var coreApiUrl = Startup.AppConfiguration.GetSection("BaseURL").GetSection("CoreApiURL").Value;
                    model.LenderConfigurations.LenderCompanyGuid = lenderCompanyGuid;
                    var responseData = ExtensionMethods<Models.LenderConfigurations>
                                       .PostJsonDatas(client, coreApiUrl, requestUri, token, model.LenderConfigurations)
                                       .Result;
                    // Deserialize json data to Class
                    var apiPostResponseData = JsonSerializer.Deserialize<ApiPostResponseData>(responseData,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    errorMessage = apiPostResponseData.ErrorMessage;
                    infoMessage = apiPostResponseData.Message;
                }

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    companyRepository.RollbackTransaction();
                }
                else
                {
                    if (infoMessage != null)
                    {
                        if (Convert.ToString(infoMessage) == CoreMessages.LenderConfigurations_Invalid)
                        {
                            errorMessage = CoreMessages.LenderConfigurations_Invalid;

                            companyRepository.RollbackTransaction();
                        }
                    }
                    else
                    {
                        errorMessage = "";

                        companyRepository.CommitTransaction();
                    }
                }
            }
            catch
            {
                companyRepository.RollbackTransaction();

                throw;
            }
        }

        #endregion

        #region Get All Lenders Details

        public List<Usp_GetLendersList> GetAllLendersDetails()
        {
            var responses = companyRepository.GetLendersList();

            return responses;
        }

        #endregion

        #region Get Assets Mapped To Broker

        private List<AssetsWorkWithResponse> GetAssetsMappedToBroker(long companyId)
        {
            var responses = new List<AssetsWorkWithResponse>();

            // Get BrokerTemplateMappings
            var templateMappings = companyRepository.GetBrokerTemplateMappings(companyId);
            var templateSets = companyRepository.GetTemplateSets();
            if (templateSets != null &&
                templateSets.Count() > 0)
            {
                templateSets.ForEach(
                    templateSet =>
                    {
                        var mappedTemplate = templateMappings.FirstOrDefault(template => template.TemplateId == templateSet.Id);
                        responses.Add(
                            new AssetsWorkWithResponse()
                            {
                                TemplateSetId = templateSet.Id,
                                TemplateName = templateSet.Name,
                                TemplateSetGUID = templateSet.TemplateSetGuid,
                                IsMapped = (mappedTemplate != null)
                                         ? true
                                         : false
                            });
                    });
            }

            return responses;
        }

        #endregion

        #region Save Assets Mapped To Broker

        private void SaveAssetsMappedToBroker(List<AssetsWorkWithResponse> assets, long companyId)
        {
            if (assets.Count() > 0)
            {
                // Remove BrokerTemplateMappings
                companyRepository.RemoveBrokerTemplateMappings(companyId);

                // Add BrokerTemplateMappings
                var templateSets = companyRepository.GetTemplateSets();
                var mappedTemplates = assets.Where(template => template.IsMapped == true).ToList();
                if (mappedTemplates != null &&
                    mappedTemplates.Count() > 0)
                {
                    mappedTemplates.ForEach(
                        asset =>
                        {
                            var templateDetail = templateSets.FirstOrDefault(template => template.TemplateSetGuid == asset.TemplateSetGUID);
                            if (templateDetail != null)
                            {
                                companyRepository.AddBrokerTemplateMappings(companyId, templateDetail.Id);
                            }
                        });
                }

                companyRepository.SaveDbChanges();
            }
        }

        private void SaveAssetsMappedToBroker(List<string> assetsWorkWith, long companyId)
        {
            // Get Mapped TemplateSets
            var newTemplateSets = companyRepository.GetMappedTemplateSets(assetsWorkWith);

            // Remove BrokerTemplateMappings
            companyRepository.RemoveBrokerTemplateMappings(companyId);

            // Add BrokerTemplateMappings
            foreach (var item in newTemplateSets)
            {
                companyRepository.AddBrokerTemplateMappings(companyId, item);
            }

            companyRepository.SaveDbChanges();
        }

        #endregion

        #region Get Assets Mapped To Lender

        private List<AssetsWorkWithResponse> GetAssetsMappedToLender(long companyId)
        {
            var responses = new List<AssetsWorkWithResponse>();

            // Get LenderTemplateMappings
            var templateMappings = companyRepository.GetLenderTemplateMappings(companyId);
            var templateSets = companyRepository.GetTemplateSets();
            if (templateSets != null &&
                templateSets.Count() > 0)
            {
                templateSets.ForEach(
                    templateSet =>
                    {
                        var mappedTemplate = templateMappings.FirstOrDefault(template => template.TemplateId == templateSet.Id);
                        responses.Add(
                            new AssetsWorkWithResponse()
                            {
                                TemplateName = templateSet.Name,
                                TemplateSetGUID = templateSet.TemplateSetGuid,
                                IsMapped = (mappedTemplate != null)
                                         ? true
                                         : false
                            });
                    });
            }

            return responses;
        }

        #endregion

        #region Save Assets Mapped To Lender

        private void SaveAssetsMappedToLender(List<string> assetsWorkWith, long companyId)
        {
            // Get Mapped TemplateSets
            var newTemplateSets = companyRepository.GetMappedTemplateSets(assetsWorkWith);

            // Remove LenderTemplateMappings
            companyRepository.RemoveLenderTemplateMappings(companyId);

            // Add LenderTemplateMappings
            foreach (var item in newTemplateSets)
            {
                companyRepository.AddLenderTemplateMappings(companyId, item);
            }

            companyRepository.SaveDbChanges();
        }

        #endregion

        #region Get Company Contacts

        private List<Models.CompanyContacts> GetCompanyContacts(long companyId)
        {
            // Get Company Contacts
            var responses = companyRepository.GetCompanyContacts(companyId);

            return responses;
        }

        #endregion

        #region Save Company Contacts

        private void SaveCompanyContacts(List<Models.CompanyContacts> companyContacts, long companyId)
        {
            // Remove Company Contacts
            companyRepository.RemoveCompanyContacts(companyId);

            // Add Company Contacts
            if (companyContacts != null &&
                companyContacts.Count() > 0)
            {
                companyContacts.ForEach(
                    contact =>
                    {
                        companyRepository.AddCompanyContacts(companyId, contact);
                    });

                companyRepository.SaveDbChanges();
            }
        }

        private void SaveCompanyContacts(Models.CompanyContacts companyContact, long companyId)
        {
            // Remove Company Contacts
            companyRepository.RemoveCompanyContacts(companyId);

            // Add Company Contacts
            companyRepository.AddCompanyContacts(companyId, companyContact);
            companyRepository.SaveDbChanges();
        }

        #endregion

        #region Get Company Details

        public Companies GetCompanyDetails(string companyGuid)
        {
            // Get Company By Guid
            var response = companyRepository.GetCompanyByGuid(companyGuid);

            return response;
        }

        #endregion

        #region Save Card Details

        public string SaveCardDetails(Companies companyDetails, List<PaymentMethod> paymentMethods, long userId)
        {
            var cardDetails = new List<CardDetails>();

            var expMonth = 0;
            var expYear = 0;

            paymentMethods.ForEach(
                item =>
                {
                    if (!string.IsNullOrEmpty(item.ExpiryDate))
                    {
                        var data = item.ExpiryDate.Split('/');
                        if (data.Length > 1)
                        {
                            expMonth = Convert.ToInt32(data[0]);
                            expYear = Convert.ToInt32(data[1]);
                        }
                    }

                    // Adding Card Details to List
                    cardDetails.Add(
                        new CardDetails()
                        {
                            CardHolderName = item.CardHolderName,
                            CardNumber = item.CardNumber,
                            ExpMonth = expMonth,
                            ExpYear = expYear,
                            Cvc = item.CVC,
                            IsPrimary = item.IsPrimary,
                            PaymentMethodId = item.PaymentMethodId,
                            IsDelete = item.IsDelete
                        });
                });

            // Get Company Details
            var companyName = companyDetails.CompanyName;
            var email = companyDetails.Email;
            var abn = companyDetails.ABN;
            var paymentCustomerId = companyDetails.PaymentCustomerId;

            // Add New Customer
            if (string.IsNullOrEmpty(paymentCustomerId))
            {
                paymentCustomerId = stripeIntegrationHelper.AddNewCustomerDetails(companyName, email, abn);
            }

            // Save Card Details
            cardDetails = stripeIntegrationHelper.SaveCardDetails(paymentCustomerId, cardDetails);

            var cardDetailsList = cardDetails.Where(c => !string.IsNullOrEmpty(c.CardNumber)).ToList();
            if (cardDetailsList.Count() > 0)
            {
                // Update Primary Card
                var primaryCardDetails = cardDetailsList.FirstOrDefault(cd => cd.IsPrimary == true);
                if (primaryCardDetails != null &&
                    primaryCardDetails.IsDelete == false)
                {
                    companyDetails.PrimaryPaymentId = primaryCardDetails.PaymentMethodId;
                }

                // Update UpdatedBy & UpdatedTime
                if (companyDetails != null)
                {
                    companyDetails.PaymentCustomerId = paymentCustomerId;
                    companyDetails.UpdatedBy = userId;
                    companyDetails.UpdatedTime = DateTime.UtcNow;

                    companyRepository.UpdateCompanyDetails(companyDetails);
                }

                // Check any error
                var errors = (from c in cardDetailsList
                              where c.ErrorMessage != null &&
                                    c.ErrorMessage.Length > 0
                              select new
                              {
                                  CardNumber = c.CardNumber,
                                  ErrorMessage = c.ErrorMessage
                              }).ToList();
                if (errors.Count() > 0)
                {
                    return JsonSerializer.Serialize(errors);
                }
            }

            return "";
        }

        #endregion

        #region Get Card Details

        public List<CardDetails> GetCardDetails(Companies companyDetails)
        {
            var responses = new List<CardDetails>();

            // Get Card Details
            var paymentCustomerId = companyDetails.PaymentCustomerId;
            var primaryPaymentId = companyDetails.PrimaryPaymentId;
            if (companyDetails != null &&
                !string.IsNullOrEmpty(paymentCustomerId) &&
                !string.IsNullOrEmpty(primaryPaymentId))
            {
                responses = stripeIntegrationHelper.GetCardsDetailsMappedToCustomer(paymentCustomerId, primaryPaymentId);
            }

            return responses;
        }

        #endregion

        #region Get Credit Card Type

        public string GetCreditCardType(string cardNumber)
        {
            // Get Credit Card Type
            var response = cardNumber.GetCreditCardType();

            return response;
        }

        #endregion

        #region Validate Card

        public CardDetails ValidateCard(CardDetails model)
        {
            var response = stripeIntegrationHelper.ValidateCard(model);

            return response;
        }

        #endregion

        #region Get Validated Users Result

        public string GetValidatedUsersResult(List<BrokerUsers> users)
        {
            var validationMsg = string.Empty;

            if (users != null &&
                users.Count > 0)
            {
                // Admin
                var adminUser = users.Where(u => u.IsAdmin == true).FirstOrDefault();
                if (adminUser == null)
                {
                    validationMsg = CommonMessages.ValidateUser_Error_IsAdminRoleMandatory;
                }

                // Billing Responsible
                var billingResponsibleUserCount = users.Count(u => u.IsBillingResponsible == true);
                if (billingResponsibleUserCount == 0)
                {
                    validationMsg = CommonMessages.ValidateUser_Error_IsBillingResponsibleRoleMandatory;
                }
                else if (billingResponsibleUserCount > 1)
                {
                    validationMsg = CommonMessages.ValidateUser_Error_OnlyOneBillingResponsibleRole;
                }

                // Primary Contact
                var primaryContactUserCount = users.Count(u => u.IsPrimaryContact == true);
                if (primaryContactUserCount == 0)
                {
                    validationMsg = CommonMessages.ValidateUser_Error_IsPrimaryContactRoleMandatory;
                }
                else if (primaryContactUserCount > 1)
                {
                    validationMsg = CommonMessages.ValidateUser_Error_OnlyOnePrimaryContactRole;
                }
            }

            return validationMsg;
        }

        #endregion

        #region Get Invalid Users Email

        public string GetInvalidUsersEmail(List<string> usersList, long userTypeId)
        {
            var response = companyRepository.GetInvalidUsersEmail(usersList, userTypeId);

            return response;
        }

        #endregion

        #region Get Invalid AssetsWorkWith

        public string GetInvalidAssetsWorkWith(List<string> assetsWorkWith)
        {
            var response = companyRepository.GetInvalidAssetsWorkWith(assetsWorkWith);

            return response;
        }

        #endregion

        #region Get Invalid LendersWorkWith

        public string GetInvalidLendersWorkWith(List<string> lendersWorkWith)
        {
            var response = companyRepository.GetInvalidLendersWorkWith(lendersWorkWith);

            return response;
        }

        #endregion

        #region Get User Details By CompanyGuid

        public List<ADUsers> GetUserDetailsByCompanyGuid(string companyGuid)
        {
            var response = companyRepository.GetUserDetailsByCompanyGuid(companyGuid);

            return response;
        }

        #endregion

        #region Get Invalid Company Contacts

        public string GetInvalidCompanyContactTypes(List<Models.CompanyContacts> companyContacts)
        {
            var response = companyRepository.GetInvalidCompanyContactTypes(companyContacts);

            return response;
        }

        #endregion

        #region Import Brokers

        public void ImportBrokers(string token, out int importErrorCount)
        {
            var errorCount = 0;

            // Get Company Names
            var staCollectorDetails = companyRepository.GetStaCollectorDetails();
            var staCollectorDetailsResponses = staCollectorDetails.Select(
                                                c => new StaCollectorDetailsResponse()
                                                {
                                                    CompanyName = c.CompanyName,
                                                    UserFirstName = c.UserFirstName,
                                                    UserLastName = c.UserLastName,
                                                    UserEmail = c.UserEmail,
                                                    UserMobileNumber = c.UserMobileNumber,
                                                    CollectorAggregatorRole = c.CollectorAggregatorRole
                                                }).ToList();
            var companyNames = staCollectorDetailsResponses.Select(d => d.CompanyName).Distinct().ToList();

            // Get Lenders
            var lenders = new List<LendersWorkWithResponse>();
            using (var client = new HttpClient())
            {
                var requestUri = "api/v1/Data/getalllenders";
                var configApiUrl = Startup.AppConfiguration.GetSection("BaseURL").GetSection("ConfigApiURL").Value;
                lenders = ExtensionMethods<LendersWorkWithResponse>
                          .GetDeserializedDatas(client, configApiUrl, requestUri, token)
                          .Result;

                lenders = lenders.Where(l => l.LenderGUID != "-1").ToList();
            }

            // Get Assets
            var assets = new List<AssetsWorkWithResponse>();
            using (var client = new HttpClient())
            {
                var requestUri = "api/v1/Data/getallassets";
                var configApiUrl = Startup.AppConfiguration.GetSection("BaseURL").GetSection("ConfigApiURL").Value;
                assets = ExtensionMethods<AssetsWorkWithResponse>
                         .GetDeserializedDatas(client, configApiUrl, requestUri, token)
                         .Result;

                assets = assets.Where(a => a.TemplateSetGUID != "-1").ToList();
            }

            // Save Company Details
            companyNames.ForEach(
                companyName =>
                {
                    var companyDetails = new CompanyDetails();
                    companyDetails.CompanyName = companyName;
                    companyDetails.RegisteredName = companyName;
                    companyDetails.ABN = "Blank";
                    companyDetails.CompanyAddress = "Blank";
                    companyDetails.City = "Blank";
                    companyDetails.State = "Blank";
                    companyDetails.ZIPCode = "Blank";
                    companyDetails.Website = "";
                    companyDetails.Email = "";
                    companyDetails.ExcemptPayment = false;
                    companyDetails.AllowOnlyInvitedUser = true;

                    var model = new Models.SaveCompanyDetailsRequest();
                    model.IsNewCompany = true;
                    model.ExistingCompanyGuid = "";
                    model.CompanyDetails = companyDetails;
                    model.LendersWorkWith = lenders.Select(l => l.LenderGUID).ToList();
                    model.AssetsWorkWith = assets.Select(a => a.TemplateSetGUID).ToList();
                    model.CompanyLogo = "";
                    model.CompanyDescription = "";
                    model.SkipRegistrationMail = true;
                    var users = staCollectorDetailsResponses.Where(u => u.CompanyName == companyName).ToList();
                    var brokerUsers = new List<BrokerUsers>();

                    bool isAdminAdded = false;
                    users.ForEach(
                        user =>
                        {
                            brokerUsers.Add(
                                new BrokerUsers()
                                {
                                    Email = user.UserEmail,
                                    FirstName = user.UserFirstName,
                                    LastName = user.UserLastName,
                                    UserGuid = "",
                                    Mobile = user.UserMobileNumber,
                                    IsAdmin = user.CollectorAggregatorRole == 1,
                                    IsBillingResponsible = user.CollectorAggregatorRole == 1 && !isAdminAdded ? true : false,
                                    IsPrimaryContact = user.CollectorAggregatorRole == 1 && !isAdminAdded ? true : false
                                });

                            if (user.CollectorAggregatorRole == 1)
                                isAdminAdded = true;
                        });
                    model.BrokerUsers = brokerUsers;

                    using (var client = new HttpClient())
                    {
                        var requestUri = "api/v1/Company/saveadmincompanydetails";
                        var configApiUrl = Startup.AppConfiguration.GetSection("BaseURL").GetSection("ConfigApiURL").Value;
                        var responseData = ExtensionMethods<Models.SaveCompanyDetailsRequest>
                                           .PostJsonDatas(client, configApiUrl, requestUri, token, model)
                                           .Result;
                        // Deserialize json data to Class
                        var apiPostResponseData = JsonSerializer.Deserialize<ApiPostResponseData>(responseData,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                        if (!string.IsNullOrEmpty(apiPostResponseData.ErrorMessage))
                        {
                            var companyInfos = staCollectorDetails.Where(c => c.CompanyName == companyName).ToList();
                            companyInfos.ForEach(
                                companyInfo =>
                                {
                                    companyInfo.ErrorMessage = apiPostResponseData.ErrorMessage;
                                });

                            companyRepository.UpdateStaCollectorDetails(companyInfos);

                            errorCount = errorCount + 1;
                        }
                    }
                });

            companyRepository.SaveChanges();

            importErrorCount = errorCount;
        }

        #endregion

        #region Private Methods

        #region Save Company Details

        private void SaveUpdateCompanyDetails(Companies companyDetails, string token)
        {
            var companyDetailsModel = new Models.Data.SaveCompanyDetailsRequest();
            companyDetailsModel.CompanyGuid = companyDetails.CompanyGuid;
            companyDetailsModel.CompanyName = companyDetails.CompanyName;
            companyDetailsModel.CompanyTypeId = companyDetails.CompanyTypeId;
            companyDetailsModel.IsPayer = companyDetails.IsPayer;
            companyDetailsModel.ExcemptPayment = companyDetails.ExcemptPayment;
            dataRepository.SaveCompanyDetails(companyDetailsModel, token);
        }

        #endregion

        #endregion
    }
}
