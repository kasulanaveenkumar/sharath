using Common.AzureBlobUtility.Helper;
using Common.AzureBlobUtility.Models;
using Common.EventMessages;
using Common.Extensions;
using Common.Identity.Helper;
using Common.Identity.Models.DVS;
using Common.Messages;
using Common.Models.ApiResponse;
using Common.Models.Core.Entities;
using Common.Notifications.InspectionNotifications;
using Common.Notifications.Models;
using Common.Notifications.SMS;
using Common.Payments.Helper;
using Core.API.Entities;
using Core.API.Helper;
using Core.API.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Core.API.Services
{
    public class InspectionService : IInspectionService
    {
        private readonly IInspectionRepository inspectionRepository;
        private readonly ILenderConfigurationRepository lenderConfigurationRepository;
        private readonly INotificationsRepository notificationsRepository;
        private readonly IDataRepository dataRepository;
        private readonly IConfiguration configuration;
        private readonly StripeIntegrationHelper stripeIntegrationHelper;
        private readonly AzureBlobHelper azureBlobHelper;
        private readonly SmsSender smsSender;
        private readonly EmailHelper emailHelper;
        private readonly SmsHelper smsHelper;

        #region Constructor

        public InspectionService(IInspectionRepository repository,
                                 ILenderConfigurationRepository lenderConfigurationRepository,
                                 INotificationsRepository notificationsRepository,
                                 IDataRepository dataRepository,
                                 IConfiguration configuration,
                                 IMessageSender messageSender)
        {
            this.inspectionRepository = repository;
            this.lenderConfigurationRepository = lenderConfigurationRepository;
            this.notificationsRepository = notificationsRepository;
            this.dataRepository = dataRepository;
            this.configuration = configuration;

            stripeIntegrationHelper = new StripeIntegrationHelper(Startup.AppConfiguration.GetSection("AppSettings").GetSection("StripeApiKey").Value);

            // Get AccountName value from appsettings.json File
            // Get AccountKey value from appsettings.json File
            // Get Container value from appsettings.json File
            var accountName = Startup.AppConfiguration.GetSection("AppSettings").GetSection("AzureStorageAccountName").Value;
            var accountKey = Startup.AppConfiguration.GetSection("AppSettings").GetSection("AzureStorageAccountKey").Value;
            var containerName = Startup.AppConfiguration.GetSection("AppSettings").GetSection("ContainerNameForAppImages").Value;
            var additionalContainerName = Startup.AppConfiguration.GetSection("AppSettings").GetSection("ContainerNameForReports").Value;
            azureBlobHelper = new AzureBlobHelper(accountName, accountKey, containerName, additionalContainerName);

            smsSender = new SmsSender(Startup.AppConfiguration);
            emailHelper = new EmailHelper(Startup.AppConfiguration);
            smsHelper = new SmsHelper(Startup.AppConfiguration);
        }

        #endregion

        #region New Inspection Details

        public Models.NewInspectionResponse NewInspectionDetails(string token, long userTypeId, string userGuid, string companyGuid)
        {
            var response = new Models.NewInspectionResponse();

            var companyDetails = dataRepository.GetCompanyDetails(companyGuid);
            response.ExemptPayment = companyDetails != null
                                   ? companyDetails.ExcemptPayment
                                   : false;

            // Get Lenders
            if (userTypeId == (int)Common.Models.Enums.UserTypes.Lender)
            {
                response.Lenders = inspectionRepository.GetLenders(companyGuid);
            }
            else
            {
                var lenders = DataHelper.GetLendersWorkWithDetails(token);
                response.Lenders = inspectionRepository.GetLenders(lenders);
            }

            // Get Assets
            response.Assets = DataHelper.GetAssetsWorkWithDetails(token);

            // Get States
            response.States = inspectionRepository.GetAllStates();

            // Get Broker Users
            response.BrokerUsers = inspectionRepository.GetBrokerUsers(userGuid, companyGuid, 0);

            response.IsLender = userTypeId == (int)Common.Models.Enums.UserTypes.Lender
                              ? true
                              : false;

            return response;
        }

        #endregion

        #region Create New Inspection

        public Int64 CreateNewInspection(Models.NewInspectionRequest model, long userId, string token, string companyGuid,
                                         string userGuid, long userTypeId, out string paymentFailedReason, out Exception errorMessage)
        {
            inspectionRepository.BeginTransaction();

            try
            {
                paymentFailedReason = "";
                errorMessage = null;
                Models.Data.CompanyDetailsResponse companyDetails = null;

                // Get TemplateSet Details By Guid
                var templateSetDetails = inspectionRepository.GetTemplateSetDetailsByGuid(model.TemplateSetGuid);
                if (templateSetDetails == null)
                {
                    return -1;
                }

                // Get Lender Details By Guid
                var lenderDetails = inspectionRepository.GetLenderDetailsByGuid(model.LenderGuid);
                if (lenderDetails == null)
                {
                    return -2;
                }

                var lenderCompanyDetails = dataRepository.GetCompanyDetails(model.LenderGuid);
                if (lenderCompanyDetails == null)
                {
                    return -3;
                }

                var brokerCompanyDetails = dataRepository.GetCompanyDetails(companyGuid);
                if (brokerCompanyDetails == null)
                {
                    return -4;
                }

                if (!lenderCompanyDetails.IsPayer &&
                    !brokerCompanyDetails.ExcemptPayment &&
                    string.IsNullOrEmpty(model.PaymentMethodId))
                {
                    return -5;
                }

                // Get Illion Intgration Details By Guid
                var illionIntegrationDetails = inspectionRepository.GetIllionDetailsByCompanyGuid(companyGuid);

                // Get Lender Configurations By Guid
                var lenderConfigurations = lenderConfigurationRepository.GetLenderDetails(model.LenderGuid);

                // Add New Applications
                var newApp = new Applications();
                newApp.TemplateSetGuid = model.TemplateSetGuid;
                newApp.RefNumber = model.LenderRef;
                newApp.ExternalRefNumber = model.ExternalRef;
                newApp.BrokerCompanyGuid = companyGuid;
                newApp.LenderCompanyGuid = model.LenderGuid;
                newApp.TemplateSetPlanGuid = model.TemplateSetPlanGuid;
                newApp.StateCode = model.StateCode;
                newApp.CreatedBy = userId;
                newApp.CreatedTime = DateTime.UtcNow;
                newApp.UpdatedTime = null;
                newApp.ApplicationStatus = (int)Models.Enums.ApplicationStatus.Created;
                newApp.WebAppShortLink = "";
                newApp.InspectionGuid = ExtensionMethods.GetNewGuid();
                newApp.DVSStatus = null;

                if ((illionIntegrationDetails != null && illionIntegrationDetails.IsActive) &&
                    (lenderConfigurations != null && lenderConfigurations.IsIllionIntegrationEnabled))
                {
                    newApp.BankStatementUrl = Startup.AppConfiguration.GetSection("AppSettings").GetSection("IllionBankStatementBaseUrl").Value;
                }

                newApp = inspectionRepository.AddApplication(newApp);

                //Get Mobile Web App Short URL
                var mobileWebAppURL = string.Format(Startup.AppConfiguration.GetSection("BaseURL").GetSection("MobileWebAppURL").Value, newApp.Id);
                var response = GetShortenURL(mobileWebAppURL).Result;

                newApp.WebAppShortLink = response.secureShortURL;

                inspectionRepository.UpdateInspection(newApp, userId);

                // Add Documents and Images
                inspectionRepository.AddAppDocumentsImages(model.Documents, newApp.Id);

                // Add AppUsers
                var appUsers = new List<AppUsers>();

                // Buyer Details
                appUsers.Add(
                    new AppUsers()
                    {
                        ApplicationId = newApp.Id,
                        Name = model.Buyer.Name,
                        SurName = model.Buyer.SurName,
                        Email = model.Buyer.Email,
                        PhoneNumber = model.Buyer.PhoneNumber,
                        Role = (int)Models.Enums.Role.Buyer,
                        UserGuid = ExtensionMethods.GetNewGuid()
                    });

                // Seller Details
                appUsers.Add(
                    new AppUsers()
                    {
                        ApplicationId = newApp.Id,
                        Name = model.Seller.Name,
                        SurName = model.Seller.SurName,
                        Email = model.Seller.Email,
                        PhoneNumber = model.Seller.PhoneNumber,
                        Role = (int)Models.Enums.Role.Seller,
                        UserGuid = ExtensionMethods.GetNewGuid()
                    });

                inspectionRepository.AddAppUsers(appUsers);

                // Add Stakeholders
                var appStakeholders = new List<AppStakeholders>();
                appStakeholders.Add(
                    new AppStakeholders()
                    {
                        ApplicationId = newApp.Id,
                        UserGuid = userGuid,
                        IsOwner = 1
                    });
                if (model.UsersToShare != null &&
                    model.UsersToShare.Count() > 0)
                {
                    model.UsersToShare.ForEach(
                        item =>
                        {
                            appStakeholders.Add(
                                new AppStakeholders()
                                {
                                    ApplicationId = newApp.Id,
                                    UserGuid = item,
                                    IsOwner = 0
                                });
                        });
                }

                inspectionRepository.AddStakeHolders(appStakeholders);

                // Add App Activity Logs
                AddAppActivityLogs((int)Models.Enums.AppActivityLogs.InspectionCreated, userTypeId, newApp.Id, userGuid, 0);

                // If NoLender Preference included
                if (model.IsIncludeNoLenderPreference)
                {
                    var noLenderPreferences = new List<Models.NoLenderPreference>();

                    // If Save Preferences Checkbox Selected
                    if (model.IsPreferenceSaved)
                    {
                        var docsList = model.Documents.Where(d => d.DocumentRequired == "yes").ToList();
                        docsList.ForEach(
                            doc =>
                            {
                                var imageTypes = new List<int>();

                                // Get Selected ImageType Details
                                var imageTypeDetails = doc.ImageDetails.Where(id => id.IsDefaultSelected == true).ToList();
                                if (imageTypeDetails != null &&
                                    imageTypeDetails.Count() > 0)
                                {
                                    imageTypeDetails.ForEach(
                                        image =>
                                        {
                                            imageTypes.Add(image.ImageType);
                                        });

                                    // Creating NoLender Preferences List
                                    noLenderPreferences.Add(
                                            new Models.NoLenderPreference()
                                            {
                                                DocumentId = doc.DocumentId,
                                                ImageTypes = imageTypes
                                            });
                                }
                            });
                    }

                    if (!string.IsNullOrEmpty(token))
                    {
                        // Save NoLender Preferences
                        using (var client = new HttpClient())
                        {
                            var requestUri = "api/v1/Data/savenolenderpreference";
                            var configApiUrl = Startup.AppConfiguration.GetSection("BaseURL").GetSection("ConfigApiURL").Value;
                            var noLenderPreferenceRequest = new Models.SaveNoLenderPreferenceRequest();
                            noLenderPreferenceRequest.TemplateSetGuid = model.TemplateSetGuid;
                            noLenderPreferenceRequest.NoLenderPreferences = noLenderPreferences;
                            noLenderPreferenceRequest.IsPreferenceSaved = model.IsPreferenceSaved;
                            var responseData = ExtensionMethods<Models.SaveNoLenderPreferenceRequest>
                                               .PostJsonDatas(client, configApiUrl, requestUri, token, noLenderPreferenceRequest)
                                               .Result;
                            // Deserialize json data to Class
                            var apiPostResponseData = JsonSerializer.Deserialize<ApiPostResponseData>(responseData,
                            new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            });
                            if (apiPostResponseData.ExceptionMessage != null)
                            {
                                paymentFailedReason = "";
                                errorMessage = apiPostResponseData.ExceptionMessage;

                                inspectionRepository.RollbackTransaction();
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(token))
                {
                    // Get Company Details
                    companyDetails = DataHelper.GetCompanyDetails(token);
                }

                var failedReason = string.Empty;

                if (newApp.Id > 0)
                {
                    var templateSetPlanPrice = (inspectionRepository.GetTemplateSetPlanPrice(model.TemplateSetGuid, 
                                                                                             model.TemplateSetPlanGuid,
                                                                                             model.LenderGuid,
                                                                                             lenderCompanyDetails.IsPayer));

                    var taxConfigs = inspectionRepository.GetCoreConfigsValue("Tax");

                    var transactionId = string.Empty;

                    if (!string.IsNullOrEmpty(model.PaymentMethodId))
                    {
                        var paymentCustomerId = companyDetails.PaymentCustomerId;

                        // Process Payment
                        decimal netAmount = 0;
                        var description = string.Join("_", "InspectionId", newApp.Id.ToString(), "New");
                        var paymentMethodId = model.PaymentMethodId;
                        var paymentPrice = templateSetPlanPrice * 100;
                        if (taxConfigs != null &&
                            taxConfigs.Count() > 0)
                        {
                            netAmount = paymentPrice + ((paymentPrice * Convert.ToInt32(taxConfigs.FirstOrDefault().Value)) / 100);
                        }
                        else
                        {
                            netAmount = paymentPrice;
                        }
                        stripeIntegrationHelper.ProcessPayment((long)netAmount, description, paymentCustomerId, paymentMethodId,
                                                               out transactionId, out failedReason);
                    }

                    // Save Payment Log
                    var paymentLogs = new PaymentLogs();
                    paymentLogs.ApplicationId = newApp.Id;
                    paymentLogs.InspectionType = (int)Models.Enums.InspectionTypes.New;
                    paymentLogs.Amount = templateSetPlanPrice;
                    paymentLogs.PaymentTime = DateTime.UtcNow;
                    paymentLogs.FailedReason = failedReason;
                    paymentLogs.TransactionId = transactionId;
                    paymentLogs.IsPaidByCard = transactionId.Length > 0;
                    paymentLogs.IsInvoiced = false;
                    if (lenderCompanyDetails.IsPayer)
                    {
                        paymentLogs.InvoiceCompanyGuid = lenderCompanyDetails.CompanyGuid;
                        paymentLogs.PaymentStatus = (int)Models.Enums.PaymentStatus.Pending;
                    }
                    else if (brokerCompanyDetails.ExcemptPayment)
                    {
                        paymentLogs.InvoiceCompanyGuid = brokerCompanyDetails.CompanyGuid;
                        paymentLogs.PaymentStatus = (int)Models.Enums.PaymentStatus.Pending;
                    }
                    else
                    {
                        paymentLogs.InvoiceCompanyGuid = "";
                        paymentLogs.PaymentStatus = string.IsNullOrEmpty(failedReason)
                                                  ? (int)Models.Enums.PaymentStatus.Success
                                                  : (int)Models.Enums.PaymentStatus.Failed;
                    }
                    if (taxConfigs != null &&
                        taxConfigs.Count() > 0)
                    {
                        paymentLogs.TaxAmount = (templateSetPlanPrice * Convert.ToInt32(taxConfigs.FirstOrDefault().Value)) / 100;
                    }
                    else
                    {
                        paymentLogs.TaxAmount = 0;
                    }
                    inspectionRepository.SavePaymentLog(paymentLogs);

                    // If payment succeeded or
                    // failed for some reason
                    if (string.IsNullOrEmpty(failedReason))
                    {
                        // Get User Details
                        var userDetails = dataRepository.GetUserDetailsByUserGuid(userGuid);
                        var userName = userDetails != null
                                     ? string.Join(" ", userDetails.Name, userDetails.SurName)
                                     : "";
                        var userEmail = userDetails != null
                                      ? userDetails.Email
                                      : "";
                        var userMobile = userDetails != null
                                       ? userDetails.Mobile
                                       : "";

                        var sellerEmail = model.Seller.Email;
                        var sellerName = model.Seller != null
                                       ? string.Join(" ", model.Seller.Name, model.Seller.SurName)
                                       : "";
                        var buyerName = model.Buyer != null
                                      ? string.Join(" ", model.Buyer.Name, model.Buyer.SurName)
                                      : "";
                        var documents = model.Documents;

                        var asset = inspectionRepository.GetAllAssets().FirstOrDefault(a => a.TemplateGuid == model.TemplateSetGuid);
                        var assetType = asset != null
                                      ? asset.TemplateName
                                      : "";

                        //Get Lender Name
                        var lender = inspectionRepository.GetAllLenders().FirstOrDefault(a => a.LenderGuid == model.LenderGuid);
                        var lenderName = lender != null
                                       ? lender.LenderName
                                       : "";

                        // Send New Inspection Email
                        emailHelper.SendNewInspectionEmail(sellerEmail, sellerName, userName, userEmail, userMobile, documents, assetType, newApp.WebAppShortLink,
                                                           lenderName, buyerName, newApp.Id.ToString());

                        // Send Sms
                        if (string.IsNullOrEmpty(buyerName))
                        {
                            smsSender.SendSMS(model.Seller.PhoneNumber, string.Format(NotificationMessages.InspectionCreationSMSWithoutBuyer,
                                                                                      sellerName, assetType, lenderName, newApp.WebAppShortLink,
                                                                                      newApp.Id.ToString()), sellerEmail);
                        }
                        else
                        {
                            smsSender.SendSMS(model.Seller.PhoneNumber, string.Format(NotificationMessages.InspectionCreationSMSWithBuyer,
                                                                                      sellerName, buyerName, assetType, lenderName, newApp.WebAppShortLink,
                                                                                      newApp.Id.ToString()), sellerEmail);
                        }

                        // Send Inspection Shared Email
                        if (model.UsersToShare != null &&
                            model.UsersToShare.Count() > 0)
                        {
                            SendInspectionSharedEmail(userTypeId, newApp.Id, asset, model.UsersToShare, userDetails);
                        }

                        inspectionRepository.CommitTransaction();
                    }
                    else
                    {
                        inspectionRepository.RollbackTransaction();
                    }
                }

                paymentFailedReason = failedReason;
                errorMessage = null;

                return newApp.Id;
            }
            catch
            {
                inspectionRepository.RollbackTransaction();

                throw;
            }

        }

        #region Get Shorten URL

        public async Task<Models.URLShortenResponseModel> GetShortenURL(string originalUrlValue)
        {
            try
            {
                Models.URLShortenResponseModel model = new Models.URLShortenResponseModel();

                var URLShorten_Domain = (Startup.AppConfiguration.GetSection("ShortenURLValues").GetSection("URLShorten_Domain").Value);

                var pairs = new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>( "domain", URLShorten_Domain),
                            new KeyValuePair<string, string>( "originalURL", originalUrlValue)
                        };

                var content = new FormUrlEncodedContent(pairs);
                using (var client = new HttpClient())
                {
                    var AuthorizationValue = (Startup.AppConfiguration.GetSection("ShortenURLValues").GetSection("URLShorten_Key").Value);

                    var Shorten_BaseURL = (Startup.AppConfiguration.GetSection("ShortenURLValues").GetSection("URLShorten_BaseURL").Value);

                    var Shorten_BaseURL_Address = new Uri(Shorten_BaseURL);

                    client.DefaultRequestHeaders.Add("Authorization", AuthorizationValue);

                    var response = await client.PostAsync(Shorten_BaseURL_Address, content).ConfigureAwait(false);
                    if (!response.IsSuccessStatusCode)
                    {
                        return new Models.URLShortenResponseModel() { secureShortURL = originalUrlValue };
                    }
                    var result = await response.Content.ReadAsStringAsync();
                    model = JsonSerializer.Deserialize<Models.URLShortenResponseModel>(result);
                }
                return model;
            }
            catch (Exception e)
            {
                // If any exception return original url
                return new Models.URLShortenResponseModel() { secureShortURL = originalUrlValue };
            }
        }

        #endregion

        #endregion

        #region Get Inspection Statuses

        public List<Models.ApplicationStatus> GetInspectionStatuses(bool isSupportTeam = false)
        {
            // Get Application Statuses
            var responses = inspectionRepository.GetInspectionStatuses(isSupportTeam);

            return responses;
        }

        #endregion

        #region Get Inspections Filter

        public Models.AdminInspectionsFilterResponse GetInspectionsFilter()
        {
            var response = new Models.AdminInspectionsFilterResponse();

            // Get All Assets
            response.Assets = inspectionRepository.GetAllAssets();

            // Get All Lenders
            response.Lenders = inspectionRepository.GetAllLenders();

            // Get Application Statuses
            response.ApplicationStatuses = inspectionRepository.GetApplicationStatuses();

            return response;
        }

        #endregion

        #region Get All Inspections List

        public Models.BrokerAllInspectionsResponse GetAllInspectionsList(Models.BrokerInspectionsRequest model,
                                                                         string userGuid, string companyGuid)
        {
            var response = new Models.BrokerAllInspectionsResponse();

            // Get All Inspections List
            long recordsCount = 0;
            var inspectionsList = inspectionRepository.GetAllInspectionsList(model, userGuid, companyGuid, out recordsCount);
            inspectionsList.ForEach(i =>
            {
                i.Status = ((Models.Enums.ApplicationStatus)(int)i.ApplicationStatus).GetEnumDescriptionAttributeValue();
            });

            response.InspectionsList = inspectionsList;
            response.TotalRecords = recordsCount;

            return response;
        }

        #endregion

        #region Get Completed Inspections List

        public Models.BrokerCompletedInspectionsResponse GetCompletedInspectionsList(Models.BrokerInspectionsRequest model,
                                                                                     string userGuid, string companyGuid)
        {
            var response = new Models.BrokerCompletedInspectionsResponse();

            // Get Completed Inspections List
            long recordsCount = 0;
            var inspectionsList = inspectionRepository.GetCompletedInspectionsList(model, userGuid, companyGuid, out recordsCount);
            inspectionsList.ForEach(i =>
            {
                i.Status = ((Models.Enums.ApplicationStatus)(int)i.ApplicationStatus).GetEnumDescriptionAttributeValue();
            });

            response.InspectionsList = inspectionsList;
            response.TotalRecords = recordsCount;

            return response;
        }

        #endregion

        #region Edit Inspection Details

        public Models.EditInspectionResponse EditInspectionDetails(Models.EditInspectionRequest model, string token,
                                                                   string userGuid, string companyGuid)
        {
            var response = new Models.EditInspectionResponse();

            // Get Lenders List
            response.Lenders = inspectionRepository.GetLenders();

            // Get Assets List
            response.Assets = inspectionRepository.GetAllAssets();

            // Get States List
            response.States = inspectionRepository.GetAllStates();

            // Get Remainders List
            response.Reminders = inspectionRepository.GetActiveReminders();

            // Get Inspection Details
            response.InspectionDetails = inspectionRepository.GetInspectionDetails(model.InspectionId, response.Lenders);

            // Exempt Payment
            var companyDetails = dataRepository.GetCompanyDetails(companyGuid);
            response.ExemptPayment = companyDetails != null
                                   ? companyDetails.ExcemptPayment
                                   : false;

            // Get Activity Logs
            response.ActivityLogs = inspectionRepository.GetActivityLogs(model.InspectionId);

            // Get Inspection Progress
            response.InspectionProgresses = inspectionRepository.GetInspectionProgress(model.InspectionId);

            // Get Inspection Documents
            response.Documents = inspectionRepository.GetInspectionDocumentsList(model.InspectionId, null);

            if (response.InspectionDetails != null)
            {
                if (response.InspectionDetails.ApplicationStatus == (int)Models.Enums.ApplicationStatus.Rejected)
                {
                    // Set Processed Status as Rejected
                    var rejectedDetails = response.InspectionProgresses.FirstOrDefault(ip => ip.StatusId == (int)Models.Enums.ApplicationStatus.Rejected);
                    if (rejectedDetails != null)
                    {
                        rejectedDetails.IsRejected = true;
                    }
                }

                // Get Inspection Plans
                var plansRequest = new Models.InspectionPlansRequest();
                plansRequest.LenderGuid = response.InspectionDetails.LenderGuid;
                plansRequest.TemplateGuid = response.InspectionDetails.TemplateSetGuid;
                plansRequest.StateId = response.InspectionDetails.StateId;
                response.InspectionPlans = inspectionRepository.GetTemplatePlans(plansRequest, token);

                // Checking whether Plan is selected
                response.IsPlanSelected = response.InspectionPlans.Count() > 0 ? true : false;
            }

            // Get Broker Users
            response.BrokerUsers = inspectionRepository.GetBrokerUsers(userGuid, companyGuid, model.InspectionId);

            // Get Config Name from appsettings
            // Get Reminder Configs 
            var configName = Startup.AppConfiguration.GetSection("CoreConfigs").GetSection("EditInspection").GetSection("RemainderVariables").Value;
            response.ReminderConfigs = inspectionRepository.GetCoreConfigsValue(configName);

            // Allow Edit Inspection
            var loggedinUser = dataRepository.GetUserDetailsByUserGuid(userGuid);
            var inspectionCreatedUser = dataRepository.GetUserDetailsByUserId(response.InspectionDetails.CreatedBy);
            if (loggedinUser != null &&
                inspectionCreatedUser != null)
            {
                response.IsEditAllowed = (loggedinUser.UserId == inspectionCreatedUser.UserId)
                                       ? true
                                       : false;
            }

            var currentTime = DateTime.UtcNow;
            var lastNotifiedTime = response.InspectionDetails.LastNotifiedTime;
            response.isNotificationAllowed = (!lastNotifiedTime.HasValue ||
                                             (lastNotifiedTime.HasValue &&
                                              currentTime.Subtract((DateTime)lastNotifiedTime).TotalHours >= 24))
                                           ? true
                                           : false;

            return response;
        }

        #endregion

        #region Edit Inspection

        public int EditInspection(Applications inspection, Models.EditInspectionRequest model, string token,
                                  long userId, string userGuid, int userTypeId,
                                  out string paymentFailedReason, out string applicationStatus)
        {
            applicationStatus = "";

            paymentFailedReason = "";

            applicationStatus = ((Models.Enums.ApplicationStatus)inspection.ApplicationStatus).ToString();
            if (inspection.ApplicationStatus != (int)Models.Enums.ApplicationStatus.Created)
            {
                return -1;
            }

            // Get App Users
            var appUsers = inspectionRepository.GetAppUsers(inspection.Id);
            if (appUsers != null)
            {
                var sellerEmail = appUsers.FirstOrDefault(au => au.Role == (int)Models.Enums.Role.Seller).Email;
                var sellerPhoneNumber = appUsers.FirstOrDefault(au => au.Role == (int)Models.Enums.Role.Seller).PhoneNumber;

                var appUsersList = new List<AppUsers>();

                // Get Buyer List For Update
                var buyerDetails = inspectionRepository.GetBuyerListForUpdate(appUsers, model.Buyer);
                appUsersList.AddRange(buyerDetails);

                // Get Seller List For Update
                var sellerDetails = inspectionRepository.GetSellerListForUpdate(appUsers, model.Seller);
                appUsersList.AddRange(sellerDetails);

                var failedReason = string.Empty;

                if (model.TemplateSetPlanGuid == null)
                    model.TemplateSetPlanGuid = "";
                if (inspection.TemplateSetPlanGuid == null)
                    inspection.TemplateSetPlanGuid = "";
                if (inspection.TemplateSetGuid != model.TemplateSetGuid ||
                    inspection.TemplateSetPlanGuid != model.TemplateSetPlanGuid)
                {
                    if (!string.IsNullOrEmpty(model.PaymentMethodId))
                    {
                        // Get Company Details
                        var companyDetails = DataHelper.GetCompanyDetails(token);
                        var paymentCustomerId = companyDetails.PaymentCustomerId;

                        // Process Payment
                        var description = string.Join("_", "InspectionId", model.InspectionId.ToString(), "Upgrade");
                        var paymentMethodId = model.PaymentMethodId;
                        var transactionId = string.Empty;
                        var paymentPrice = 900;
                        stripeIntegrationHelper.ProcessPayment(paymentPrice, description, paymentCustomerId, paymentMethodId,
                                                               out transactionId, out failedReason);

                        // Save Payment Log
                        var paymentLogs = new PaymentLogs();
                        paymentLogs.ApplicationId = model.InspectionId;
                        paymentLogs.InspectionType = (int)Models.Enums.InspectionTypes.Upgrade;
                        paymentLogs.Amount = 9;
                        paymentLogs.PaymentTime = DateTime.UtcNow;
                        paymentLogs.PaymentStatus = string.IsNullOrEmpty(failedReason)
                                                  ? (int)Models.Enums.PaymentStatus.Success
                                                  : (int)Models.Enums.PaymentStatus.Failed;
                        paymentLogs.FailedReason = failedReason;
                        paymentLogs.TransactionId = transactionId;
                        inspectionRepository.SavePaymentLog(paymentLogs);
                    }

                    // Add App Activity Logs
                    AddAppActivityLogs((int)Models.Enums.AppActivityLogs.PlanUpgraded, userTypeId, model.InspectionId, userGuid, 0);

                    // Update Template Or Plan
                    inspectionRepository.UpdateTemplateOrPlan(model.Documents, model.InspectionId);
                }

                // If payment failed for some reason
                if (!string.IsNullOrEmpty(failedReason))
                {
                    paymentFailedReason = failedReason;

                    inspectionRepository.RollbackTransaction();

                    return -2;
                }

                // Get User Details
                var userDetails = dataRepository.GetUserDetailsByUserGuid(userGuid);

                // Get Asset Type
                var asset = inspectionRepository.GetAllAssets().FirstOrDefault(a => a.TemplateGuid == model.TemplateSetGuid);
                var assetType = asset != null
                              ? asset.TemplateName
                              : "";

                // If Seller Email or Phone Number changed
                if (model.Seller.Email != sellerEmail ||
                    model.Seller.PhoneNumber != sellerPhoneNumber)
                {
                    var userName = userDetails != null
                                 ? string.Join(" ", userDetails.Name, userDetails.SurName)
                                 : "";
                    var userEmail = userDetails != null
                                  ? userDetails.Email
                                  : "";
                    var userMobile = userDetails != null
                                   ? userDetails.Mobile
                                   : "";

                    //Get Lender Name
                    var lender = inspectionRepository.GetAllLenders().FirstOrDefault(a => a.LenderGuid == model.LenderGuid);
                    var lenderName = lender != null
                                   ? lender.LenderName
                                   : "";

                    // Send New Inspection Email
                    var sellerName = model.Seller != null
                                   ? string.Join(" ", model.Seller.Name, model.Seller.SurName)
                                   : "";
                    var buyerName = model.Buyer != null
                                  ? string.Join("", model.Buyer.Name, model.Buyer.SurName)
                                  : "";
                    var documents = model.Documents;

                    emailHelper.SendNewInspectionEmail(model.Seller.Email, sellerName, userName, userEmail, userMobile, documents, assetType, inspection.WebAppShortLink,
                                                       buyerName, lenderName, inspection.Id.ToString());

                    // Send Sms
                    if (string.IsNullOrEmpty(buyerName))
                    {
                        smsSender.SendSMS(model.Seller.PhoneNumber, string.Format(NotificationMessages.InspectionCreationSMSWithoutBuyer,
                                                                                  sellerName, assetType, lenderName, inspection.WebAppShortLink,
                                                                                  inspection.Id.ToString()), sellerEmail);
                    }
                    else
                    {
                        smsSender.SendSMS(model.Seller.PhoneNumber, string.Format(NotificationMessages.InspectionCreationSMSWithBuyer,
                                                                                  sellerName, buyerName, assetType, lenderName, inspection.WebAppShortLink,
                                                                                  inspection.Id.ToString()), sellerEmail);
                    }
                }

                // Send Inspection Shared Email
                if (model.UsersToShare != null &&
                    model.UsersToShare.Count() > 0)
                {
                    SendInspectionSharedEmail(userTypeId, model.InspectionId, asset, model.UsersToShare, userDetails);
                }

                inspectionRepository.UpdateAppUsers(appUsersList);
            }

            // Update Inspection
            inspectionRepository.UpdateInspection(inspection, model, userId);

            // Save Shared Users
            inspectionRepository.SaveSharedUsers(model.UsersToShare, inspection.Id);

            return 1;
        }

        #endregion

        #region Get Activity Logs Data 

        public string GetActivityLogsData(long inspectionId)
        {
            // Get CSV File Download String
            var response = inspectionRepository.GetCSVFileDownloadString(inspectionId);

            return response;
        }

        #endregion

        #region Send Reminder

        public int SendReminder(Models.ReminderRequest model, long userId)
        {
            // Get Reminder Datas
            var reminderDatas = inspectionRepository.GetReminderDatasByInspectionId(model.InspectionId);

            // Get Config Name from appsettings
            var configName = Startup.AppConfiguration.GetSection("CoreConfigs").GetSection("EditInspection")
                             .GetSection("RemainderVariablesToBeReplacedForSendingEmail").Value;

            // Get Reminder Configs
            // Get DOC-PENDING text from Reminder Configs
            // Get DOC-REJECTED text from Reminder Configs
            var reminderConfigs = inspectionRepository.GetCoreConfigsValue(configName);
            var docPendingText = reminderConfigs[5].Value;
            var docRejectedText = reminderConfigs[6].Value;

            // Get Reminder Message
            var reminderMessage = inspectionRepository.GetReminderMessage(reminderDatas, reminderConfigs, model.Message, model.InspectionId);

            if (model.Message.Contains(docPendingText) &&
                reminderDatas.PendingDocuments.Count() == 0)
            {
                return -1;
            }
            else if (model.Message.Contains(docRejectedText) &&
                     reminderDatas.RejectedDocuments.Count() == 0)
            {
                return -2;
            }
            else
            {
                // Send Reminder email
                var sellerEmail = reminderDatas.Seller.Email;
                emailHelper.SendReminderEmail(sellerEmail, reminderMessage.Replace("\r", "<br/>"));

                // Update Last NotifiedTime
                inspectionRepository.UpdateLastNotifiedTime(model.InspectionId, userId);

                return 1;
            }
        }

        #endregion

        #region Update Bypass Reason

        public void UpdateBypassReason(AppImages imageDetail, Models.InspectionBypassRequest model, int userTypeId,
                                       string userGuid, long userId)
        {
            // Update Bypass Reason
            imageDetail.BypassReason = model.ByPassReason;
            imageDetail.IsBypassRequested = true;
            imageDetail.UpdatedBy = userId;
            imageDetail.UserType = (short)userTypeId;
            imageDetail.UpdatedTime = DateTime.UtcNow;
            imageDetail.BypassRequestedBy = userGuid;
            imageDetail.ImageInternalStatus = (int)Models.Enums.DocInternalStatus.Uploaded;
            imageDetail.ImageStatus = (int)Models.Enums.DocImageStatus.Uploaded;
            inspectionRepository.UpdateAppImages(imageDetail);

            // Get Inspection Details By Id
            var inspectionDetail = inspectionRepository.GetInspectionDetails(model.InspectionId);
            if (inspectionDetail != null)
            {
                // Update Inspection
                inspectionDetail.IsBypassRequested = true;
                inspectionRepository.UpdateInspection(inspectionDetail, userId);
            }

            inspectionRepository.SaveDbChanges();

            // Add App Activity Logs
            AddAppActivityLogs((int)Models.Enums.AppActivityLogs.BypassRequested, userTypeId, model.InspectionId, userGuid, 0);
        }

        #endregion

        #region Cancel Inspection

        public void CancelInspection(long inspectionId, long userId)
        {
            // Get Inspection Details
            var inspection = inspectionRepository.GetInspectionDetails(inspectionId);
            if (inspection != null)
            {
                // Cancel Inspection
                inspectionRepository.CancelInspection(inspection, userId);
            }
        }

        #endregion#region Save Image Data

        #region Get Image Data

        public Models.InspectionGetImageResponse GetImageData(AppImages imageDetails, long inspectionId, long imageId)
        {
            Models.InspectionGetImageResponse response = null;

            if (!string.IsNullOrEmpty(imageDetails.FilePath))
            {
                // Get Image Data
                response = inspectionRepository.GetImageData(imageDetails);
            }

            return response;
        }

        #endregion

        #region Get Review Data

        public Models.ReviewDataResponse GetReviewData(long inspectionId)
        {
            var response = new Models.ReviewDataResponse();

            // Get Lenders List
            var lenders = inspectionRepository.GetLenders();

            // Get Inspection Details
            response.InspectionDetails = inspectionRepository.GetInspectionDetails(inspectionId, lenders);

            // Get Lender Configurations
            var lenderConfig = inspectionRepository.GetLenderConfigurationByGuid(response.InspectionDetails.LenderGuid);
            if (lenderConfig != null)
            {
                response.IsReportRequired = lenderConfig.IsReportRequired;
            }

            // Get Inspection Documents
            var documents = inspectionRepository.GetInspectionDocumentsList(inspectionId, lenderConfig);
            response.Documents = documents.Where(d => !d.DocumentName.Contains("PPSR")).ToList();

            // DVS Status
            response.DVSStatus = response.InspectionDetails.DVSStatus;

            // Get Core Configurations to Enable PPSR Search by Serial Number
            var templateGuids = inspectionRepository.GetCoreConfigsValue("TemplateId_SearchPPSRBySerialNumber");
            if (templateGuids != null &&
                templateGuids.Count() > 0)
            {
                response.IsPPSRSearchBySerialNumberEnabled = templateGuids.Select(t => t.Value).ToList().Contains(response.InspectionDetails.TemplateSetGuid);
            }

            // Set PPSR For Vehicle Status
            var ppsrForVehicle = documents.FirstOrDefault(d => d.ImageDetails.Any(i => i.ImageType == (short)Models.Enums.ImageTypes.PPSRCheck));
            if (ppsrForVehicle == null)
            {
                response.PPSRForVehicleStatus = 0;
            }
            else if (ppsrForVehicle.ImageDetails.Count(i => i.ImageStatus == (int)Models.Enums.DocImageStatus.Uploaded) > 0)
            {
                response.PPSRForVehicleStatus = 2;
            }
            else
            {
                response.PPSRForVehicleStatus = 1;
            }

            // Set PPSR For Boat Status
            var ppsrForBoat = documents.FirstOrDefault(d => d.ImageDetails.Any(i => i.ImageType == (short)Models.Enums.ImageTypes.PPSRBoat));
            if (ppsrForBoat == null)
            {
                response.PPSRForBoatStatus = 0;
            }
            else if (ppsrForBoat.ImageDetails.Count(i => i.ImageStatus == (int)Models.Enums.DocImageStatus.Uploaded) > 0)
            {
                response.PPSRForBoatStatus = 2;
            }
            else
            {
                response.PPSRForBoatStatus = 1;
            }

            // Set PPSR For Trailer Status
            var ppsrForTrailer = documents.FirstOrDefault(d => d.ImageDetails.Any(i => i.ImageType == (short)Models.Enums.ImageTypes.PPSRTrailer));
            if (ppsrForTrailer == null)
            {
                response.PPSRForTrailerStatus = 0;
            }
            else if (ppsrForTrailer.ImageDetails.Count(i => i.ImageStatus == (int)Models.Enums.DocImageStatus.Uploaded) > 0)
            {
                response.PPSRForTrailerStatus = 2;
            }
            else
            {
                response.PPSRForTrailerStatus = 1;
            }

            return response;
        }

        #endregion

        #region Save Review Data

        public int SaveReviewData(Models.ReviewInspectionSaveRequest model, string userGuid, long userId, out string errorMessage)
        {
            int response = 0;
            errorMessage = string.Empty;

            inspectionRepository.BeginTransaction();

            try
            {
                // Get AppImageReasons by Inspection Id
                var appImageReasons = inspectionRepository.GetAppImageReasonsByInspectionId(model.InspectionId);
                if (appImageReasons.Count() > 0)
                {
                    // Delete AppImageReasons
                    inspectionRepository.DeleteAppImageReasons(appImageReasons);
                }

                // Get Inspection Details by Inspection Id
                // Get AppDocuments by Inspection Id
                // Get AppImages by Inspection Id
                // Get User Details By Id
                // Get Seller Details by Inspection Id
                // Get Lender Details by CompanyGuid
                var inspectionDetails = inspectionRepository.GetInspectionDetails(model.InspectionId);
                var appDocuments = inspectionRepository.GetAppDocuments(inspectionDetails.Id);
                var appImages = inspectionRepository.GetAppImagesByInspectionId(inspectionDetails.Id);
                var userDetails = dataRepository.GetUserDetailsByUserId(inspectionDetails.CreatedBy);
                var sellerDetails = inspectionRepository.GetAppUsers(inspectionDetails.Id)
                                    .FirstOrDefault(au => au.Role == (int)Models.Enums.Role.Seller);
                var lenderDetails = dataRepository.GetCompanyDetails(inspectionDetails.LenderCompanyGuid);

                var isInspectionRejected = false;

                var imageUpdateCount = 0;

                model.Images.ForEach(
                    image =>
                    {
                        // Add AppImageReasons
                        if (image.SelectedFlagReasons != null &&
                            image.SelectedFlagReasons.Count() > 0)
                        {
                            AddAppImageReasons(image.SelectedFlagReasons, model.InspectionId, image.ImageId, Models.Enums.ReasonTypes.FlagReason);
                        }
                        if (image.SelectedRejectedReasons != null &&
                            image.SelectedRejectedReasons.Count() > 0)
                        {
                            AddAppImageReasons(image.SelectedRejectedReasons, model.InspectionId, image.ImageId, Models.Enums.ReasonTypes.RejectReason);
                        }

                        // Get Image Details
                        var imageDetails = appImages.FirstOrDefault(ai => ai.Id == image.ImageId);

                        // Update OtherFlagReason if not null or empty
                        //if (!string.IsNullOrEmpty(image.OtherFlagReason))
                        //{
                        //    imageDetails.OtherFlagReason = image.OtherFlagReason;
                        //}
                        imageDetails.OtherFlagReason = image.OtherFlagReason;

                        // Update OtherRejectReason if not null or empty
                        //if (!string.IsNullOrEmpty(image.OtherRejectReason))
                        //{
                        //    imageDetails.OtherRejectReason = image.OtherRejectReason;
                        //}
                        imageDetails.OtherRejectReason = image.OtherRejectReason;

                        // Update BypassNotes if not null or empty
                        //if (!string.IsNullOrEmpty(image.BypassNotes))
                        //{
                        //    imageDetails.BypassReason = image.BypassNotes;
                        //}
                        imageDetails.BypassReason = image.BypassNotes;

                        imageDetails.IsBypassRequested = !string.IsNullOrEmpty(imageDetails.BypassReason)
                                                       ? true
                                                       : false;

                        imageDetails.BypassRequestedBy = !string.IsNullOrEmpty(imageDetails.BypassReason)
                                                       ? userGuid
                                                       : null;

                        // Update ImageInternalStatus if not null or empty
                        if (!string.IsNullOrEmpty(image.ImageInternalStatus))
                        {
                            // Update ImageInternalStatus to Rejected if any reject reasons available or 
                            // Update ImageInternalStatus to Flagged if any flag reasons available or
                            // Update ImageInternalStatus to Accepted if any flag reasons available
                            if (!string.IsNullOrEmpty(image.BypassNotes))
                            {
                                imageDetails.ImageInternalStatus = (int)Models.Enums.DocInternalStatus.Uploaded;
                            }
                            else if (image.ImageInternalStatus == Models.Enums.DocInternalStatus.Rejected.ToString())
                            {
                                imageDetails.ImageInternalStatus = (int)Models.Enums.DocInternalStatus.Rejected;
                            }
                            else if (image.ImageInternalStatus == Models.Enums.DocInternalStatus.Flagged.ToString())
                            {
                                imageDetails.ImageInternalStatus = (int)Models.Enums.DocInternalStatus.Flagged;
                            }
                            else if (image.ImageInternalStatus == Models.Enums.DocInternalStatus.Accepted.ToString())
                            {
                                imageDetails.ImageInternalStatus = (int)Models.Enums.DocInternalStatus.Accepted;
                            }
                        }
                        else
                        {
                            imageDetails.ImageInternalStatus = 0;
                        }

                        // Update ImageStatus if not SaveDraft
                        if (!model.IsSaveDraft)
                        {
                            // Update ImageStatus to Rejected if any reject reasons available
                            if (!string.IsNullOrEmpty(image.BypassNotes) &&
                                !(imageDetails.ImageStatus == (int)Models.Enums.DocImageStatus.Pending ||
                                  imageDetails.ImageStatus == (int)Models.Enums.DocImageStatus.PreUploaded))
                            {
                                imageDetails.ImageStatus = (int)Models.Enums.DocImageStatus.Uploaded;
                            }
                            else if ((image.SelectedRejectedReasons != null &&
                                      image.SelectedRejectedReasons.Count() > 0) ||
                                    (!string.IsNullOrEmpty(image.OtherRejectReason)))
                            {
                                isInspectionRejected = true;

                                imageDetails.ImageStatus = (int)Models.Enums.DocImageStatus.Rejected;

                                // Update Document Status to Rejected
                                if (appDocuments != null &&
                                    appDocuments.Count() > 0)
                                {
                                    var appDocument = appDocuments.FirstOrDefault(d => d.Id == imageDetails.AppDocumentId);
                                    if (appDocument != null)
                                    {
                                        appDocument.DocStatus = (int)Models.Enums.DocStatus.Rejected;
                                        inspectionRepository.UpdateAppDocuments(appDocument);
                                    }
                                }
                            }
                        }

                        // Update ImageData
                        imageDetails.ImageData = image.ImageData;

                        // Update AppImages
                        inspectionRepository.UpdateAppImages(imageDetails);

                        imageUpdateCount++;
                    });

                if (imageUpdateCount > 0)
                {
                    response = 1;
                }

                // Update ApplicationStatus if not SaveDraft
                if (!model.IsSaveDraft)
                {
                    if (inspectionDetails != null)
                    {
                        if (!isInspectionRejected &&
                            appImages.Any(ai => ai.IsBypassRequested == true))
                        {
                            inspectionDetails.IsBypassRequested = true;
                        }

                        // Add App Activity Logs
                        var appActivityId = (isInspectionRejected == true)
                                          ? (int)Models.Enums.AppActivityLogs.InspectionRejected
                                          : (int)Models.Enums.AppActivityLogs.InspectionCompleted;
                        AddAppActivityLogs(appActivityId, (int)Common.Models.Enums.UserTypes.SupportTeam, model.InspectionId, userGuid, 0);

                        // Update Inspection
                        // Update ApplicationStatus to Rejected if any reject reasons available
                        // Otherwise ApplicationStatus to Completed
                        inspectionDetails.ApplicationStatus = (isInspectionRejected == true)
                                                            ? (short)Models.Enums.ApplicationStatus.Rejected
                                                            : (short)Models.Enums.ApplicationStatus.Completed;
                        inspectionRepository.UpdateInspection(inspectionDetails, userId, isInspectionRejected);
                    }
                }

                if (!model.IsSaveDraft)
                {
                    if (isInspectionRejected)
                    {
                        // Send Reject Email to seller if Inspection Rejected
                        response = SendRejectEmail(inspectionDetails, sellerDetails, lenderDetails);
                    }
                    else
                    {
                        // Otherwise Send Submission Report to lender
                        response = SendSubmissionReport(inspectionDetails, userDetails, sellerDetails, lenderDetails,
                                                        inspectionDetails.InspectionGuid, true, model.IsSendAutomaticReport,
                                                        out errorMessage);
                    }
                }

                if (string.IsNullOrEmpty(errorMessage))
                {
                    // If Response is Success
                    if (response == 1)
                    {
                        // Send Inspection Notification
                        SendInspectionNotification(inspectionDetails, userDetails, sellerDetails, appDocuments);

                        // Save Db Changes
                        // Commit Transaction
                        inspectionRepository.SaveDbChanges();
                        inspectionRepository.CommitTransaction();
                    }
                }
                else
                {
                    response = -1;
                    inspectionRepository.RollbackTransaction();
                }

                return response;
            }
            catch (Exception ex)
            {
                // Rollback Transaction
                inspectionRepository.RollbackTransaction();

                throw ex;
            }
        }

        #region Send Inspection Notification

        private void SendInspectionNotification(Applications inspectionDetails,
                                                Entities.ADUsers userDetails, 
                                                AppUsers sellerDetails,
                                                List<AppDocuments> documents)
        {
            var eventType = new Events();
            var model = new InspectionNotification();

            model.InspectionId = inspectionDetails.Id;
            model.BrokerName = String.Join(" ", userDetails.Name, userDetails.SurName);
            model.ApplicationStatusId = inspectionDetails.ApplicationStatus;
            model.SellerName = string.Join(" ", sellerDetails.Name, sellerDetails.SurName);

            // Is notification email required?
            var userNotifications = notificationsRepository.GetNotificationMappings(userDetails.UserGuid, inspectionDetails.BrokerCompanyGuid);
            var appStatus = userNotifications.FirstOrDefault(un => un.AppActivityId == inspectionDetails.ApplicationStatus);
            var isEmailRequired = appStatus != null
                                ? true
                                : false;

            if (isEmailRequired)
            {
                model.BrokerEmail = userDetails.Email;

                if (inspectionDetails.ApplicationStatus == (int)Models.Enums.ApplicationStatus.Rejected)
                {
                    model.RejectedDocuments = documents.Where(d => d.DocStatus == (int)Models.Enums.DocStatus.Rejected)
                                              .Select(d => new RejectedDocuments()
                                              {
                                                  DocName = d.Name,
                                                  SellerAction = ""
                                              }).ToList();

                    eventType = Events.Inspection_AdminRejected;
                }
                else
                {
                    eventType = Events.Inspection_AdminProcessed;
                }
            }

            // Is Webhook Enabled?
            var isWebhookEnabled = false;
            // Get B2BWebhook and pdate isWebhookRequired

            if (isWebhookEnabled)
            {
                model.WebhookTargetURL = "B2BWebhook.TargetURL";
            }

            // Invoke Notifier
            InspectionStatusNotifier.SendInspectionNotification(model, eventType, configuration);
        }

        #endregion

        #region Send Reject Email

        private int SendRejectEmail(Applications inspectionDetails, AppUsers sellerDetails, ADCompanies lenderDetails)
        {
            if (sellerDetails != null)
            {
                var sellerEmail = sellerDetails != null
                                ? sellerDetails.Email
                                : "";
                var sellerName = sellerDetails != null
                               ? string.Join(" ", sellerDetails.Name, sellerDetails.SurName)
                               : "";
                var lenderName = lenderDetails != null
                               ? lenderDetails.CompanyName
                               : "";

                // Send Inspection Rejectd Email
                emailHelper.SendInspectionRejectedEmail(sellerEmail, sellerName, inspectionDetails.WebAppShortLink,
                                                        inspectionDetails.Id, lenderName);

                // Send Inspection Rejectd Sms
                smsHelper.SendSMS(sellerDetails, string.Format(NotificationMessages.WebAppInspectionRejectionSMS,
                                                               sellerName, inspectionDetails.WebAppShortLink));
            }

            return 1;
        }

        #endregion

        #region Send Submission Report

        public int SendSubmissionReport(Applications inspectionDetails, Entities.ADUsers userDetails, AppUsers sellerDetails,
                                        ADCompanies lenderDetails, string inspectionGuid, bool isGenerateReport, bool isSendEmail,
                                        out string errorMessage)
        {
            int response = 1;
            errorMessage = string.Empty;

            // Generate report
            if (isGenerateReport)
            {
                using (var client = new HttpClient())
                {
                    var requestUri = "api/generatereport/" + inspectionDetails.Id;
                    var externalApiUrl = Startup.AppConfiguration.GetSection("BaseURL").GetSection("ExternalApiURL").Value;
                    var responseData = ExtensionMethods<ApiPostResponseData>
                                       .GetDeserializedData(client, externalApiUrl, requestUri, null)
                                       .Result;

                    // Error in generating report
                    if (!string.IsNullOrEmpty(responseData.ErrorMessage))
                    {
                        errorMessage = responseData.ErrorMessage;
                        response = -1;
                    }
                }
            }

            // Send Report in Email
            if (isSendEmail)
            {
                // Get Lender Configuration
                var lenderConfiguration = inspectionRepository.GetLenderConfigurationByGuid(lenderDetails.CompanyGuid);
                if (lenderConfiguration == null)
                {
                    response = -2;
                }

                // Get Broker Details By CompanyGuid
                // Get Lender Ref Number
                // Get Broker Company Name
                // Get Broker User
                // Get Seller Name
                // Get Lender Name
                var brokerDetails = dataRepository.GetCompanyDetails(inspectionDetails.BrokerCompanyGuid);
                var lenderRefNumber = inspectionDetails != null
                                    ? inspectionDetails.RefNumber
                                    : "";
                var brokerCompanyName = brokerDetails != null
                                      ? brokerDetails.CompanyName
                                      : "";
                var brokerUser = userDetails != null
                               ? string.Join(" ", userDetails.Name, userDetails.SurName)
                               : "";
                var sellerName = sellerDetails != null
                               ? string.Join(" ", sellerDetails.Name, sellerDetails.SurName)
                               : "";
                var lenderName = lenderDetails != null
                               ? lenderDetails.CompanyName
                               : "";

                // Get Report Email
                var reportEmail = lenderConfiguration.ReportEmailAddress;
                if (string.IsNullOrEmpty(reportEmail))
                {
                    response = -3;
                }

                long inspectionId = inspectionDetails.Id;
                var base64String = GetReport(ref inspectionId, inspectionGuid);
                if (string.IsNullOrEmpty(base64String))
                {
                    response = -4; // Report not available
                }

                var fileAttachments = new List<FileAttachments>();
                fileAttachments.Add(
                    new FileAttachments()
                    {
                        FileName = string.Format("Verimoto_InspectionReport_{0}.pdf", inspectionDetails.Id),
                        Base64FileData = base64String
                    });

                emailHelper.SendInspectionSubmissionEmail(reportEmail, sellerName, lenderName, inspectionDetails.Id,
                                                          lenderRefNumber, brokerCompanyName, brokerUser, fileAttachments);
            }

            return response; // All success
        }

        #endregion

        #region Get Report

        public string GetReport(ref long inspectionId, string inspectionGuid)
        {
            if (inspectionId == 0)
            {
                var inspectionDetails = inspectionRepository.GetInspectionDetails(inspectionGuid);
                if (inspectionDetails == null)
                {
                    return null;
                }

                inspectionId = inspectionDetails.Id;
            }

            var filePath = string.Format("{0}/{1}.pdf", inspectionId, inspectionGuid);
            var base64String = "";
            ImageProperties props = null;
            azureBlobHelper.GetBlobFromAzureStorageAdditionalContainer(filePath, ref base64String, out props, false, false);

            if (string.IsNullOrEmpty(base64String))
                return null;

            return base64String.Split(",")[1];
        }

        #endregion

        #region Add AppImageReasons

        private void AddAppImageReasons(List<int> reasons, long inspectionId, long appImageId, Models.Enums.ReasonTypes reasonType)
        {
            var reasonsList = new List<AppImageReasons>();

            reasons.ForEach(
                reason =>
                {
                    // Adding reason to List
                    reasonsList.Add(
                        new AppImageReasons()
                        {
                            ApplicationId = inspectionId,
                            AppImageId = appImageId,
                            ReasonId = reason,
                            ReasonType = (int)reasonType
                        });
                });

            inspectionRepository.AddAppImageReasons(reasonsList);
        }

        #endregion

        #endregion

        #region Rotate Image

        public string RotateImage(AppImages imageDetails, Models.ReviewInspectionRotateImageRequest model, long userId)
        {
            var response = string.Empty;

            if (imageDetails != null)
            {
                var filePath = imageDetails.FilePath;

                // Get Blob from Azure Storage Container
                azureBlobHelper.UploadBlobToAzureStorageContainer(filePath, model.RotationAngle, ref response);
            }

            return response;
        }

        #endregion

        #region Move Image

        public int MoveImage(Models.ReviewInspectionMoveImageRequest model, long userId)
        {
            // Get Source Image Details
            var sourceImageDetails = inspectionRepository.GetAppImageDetailsById(model.SourceImageId, model.InspectionId);

            // Get Destination Image Details
            var destinationImageDetails = inspectionRepository.GetAppImageDetailsById(model.DestinationImageId, model.InspectionId);

            if (sourceImageDetails != null &&
                destinationImageDetails != null)
            {
                // Source Image not exist
                if (string.IsNullOrEmpty(sourceImageDetails.FilePath))
                {
                    return -1;
                }

                if (string.IsNullOrEmpty(destinationImageDetails.FilePath))
                {
                    return -2;
                }

                // Transfer Blobs in Azure Storage Container
                var inspectionId = model.InspectionId;
                var sourceImageId = model.SourceImageId;
                var sourceFilePath = sourceImageDetails.FilePath;
                var destinationImageId = model.DestinationImageId;
                var destinationFilePath = destinationImageDetails.FilePath;
                var sourceBase64String = string.Empty;
                var destinationBase64String = string.Empty;
                var sourceNewFilePath = string.Empty;
                var destinationNewFilePath = string.Empty;
                azureBlobHelper.TransferBlobsInAzureStorageContainer(inspectionId,
                                                                     sourceImageId, sourceFilePath,
                                                                     destinationImageId, destinationFilePath,
                                                                     out sourceBase64String, out destinationBase64String,
                                                                     out sourceNewFilePath, out destinationNewFilePath);

                // Update Source Image Details
                if (!string.IsNullOrEmpty(sourceNewFilePath))
                {
                    sourceImageDetails.FilePath = !string.IsNullOrEmpty(sourceNewFilePath)
                                                ? sourceNewFilePath
                                                : null;
                    sourceImageDetails.FileName = !string.IsNullOrEmpty(sourceNewFilePath)
                                                ? sourceNewFilePath.Split("/")[2]
                                                : null;
                    sourceImageDetails.Extension = !string.IsNullOrEmpty(destinationBase64String)
                                                 ? destinationBase64String.GetBase64Extension().ToString()
                                                 : null;
                    sourceImageDetails.SizeInKb = !string.IsNullOrEmpty(destinationBase64String)
                                                ? destinationBase64String.GetFileSizeFromBase64().ToString()
                                                : null;
                    sourceImageDetails.UpdatedBy = userId;
                    sourceImageDetails.UpdatedTime = DateTime.UtcNow;
                    inspectionRepository.UpdateAppImages(sourceImageDetails);
                }

                // Update Destination Image Details
                if (!string.IsNullOrEmpty(destinationNewFilePath))
                {
                    destinationImageDetails.FilePath = !string.IsNullOrEmpty(destinationNewFilePath)
                                                     ? destinationNewFilePath
                                                     : null;
                    destinationImageDetails.FileName = !string.IsNullOrEmpty(destinationNewFilePath)
                                                     ? destinationNewFilePath.Split("/")[2]
                                                     : null;
                    destinationImageDetails.Extension = !string.IsNullOrEmpty(sourceBase64String)
                                                      ? sourceBase64String.GetBase64Extension().ToString()
                                                      : null;
                    destinationImageDetails.SizeInKb = !string.IsNullOrEmpty(sourceBase64String)
                                                     ? sourceBase64String.GetFileSizeFromBase64().ToString()
                                                     : null;
                    destinationImageDetails.UpdatedBy = userId;
                    destinationImageDetails.UpdatedTime = DateTime.UtcNow;
                    inspectionRepository.UpdateAppImages(destinationImageDetails);
                }

                inspectionRepository.SaveDbChanges();

                return 1;
            }
            else
            {
                return -3;
            }
        }

        #endregion

        #region Get Notifications List

        public Models.NotificationsDetailsResponse GetNotificationsList(Models.NotificationsDetailsRequest model,
                                                                        string userGuid, string companyGuid)
        {
            var response = new Models.NotificationsDetailsResponse();

            // Get Notifications List
            var notificationsList = inspectionRepository.GetNotificationsList(userGuid, companyGuid);
            response.NotificationsList = notificationsList.Sort(model.SortColumn, model.SortDirection)
                                         .Skip(model.SkipData).Take(model.LimitData)
                                         .ToDynamicList<Models.NotificationsList>();
            response.TotalRecords = notificationsList.Count();

            return response;
        }

        #endregion

        #region Get Notifications Count

        public int GetNotificationsCount(string userGuid, string companyGuid)
        {
            var response = inspectionRepository.GetNotificationsCount(userGuid, companyGuid);

            return response;
        }

        #endregion

        #region Update Notification Status

        public void UpdateNotificationStatus(string userGuid, string companyGuid)
        {
            inspectionRepository.UpdateNotificationStatus(userGuid, companyGuid);
        }

        #endregion

        #region Check DVS

        public int CheckDVS(CheckDVSRequest model, long userId)
        {
            var result = -1;

            // Get BaseUrl
            // Get DVS ApiUrl
            // Get DVSAccessKey, DVSSecretKey
            var apiUrl = Startup.AppConfiguration.GetSection("Kreano").GetSection("BaseUrl").Value;
            var requestUri = Startup.AppConfiguration.GetSection("Kreano").GetSection("DVSApiURL").Value;
            var httpHeaderValues = new Dictionary<string, object>();
            httpHeaderValues.Add("x-access-key", Startup.AppConfiguration.GetSection("Kreano").GetSection("DVSAccessKey").Value);
            httpHeaderValues.Add("x-secret-key", Startup.AppConfiguration.GetSection("Kreano").GetSection("DVSSecretKey").Value);

            // Check DVS
            var kreanoHelper = new KreanoHelper();
            var kreanoDVSCheckModel = (KreanoDVSCheckModel)model;
            var response = kreanoHelper.CheckDVS(apiUrl, requestUri, httpHeaderValues, kreanoDVSCheckModel);
            if (response.StatusCode == (int)HttpStatusCode.OK)
            {
                // Get VerificationResultCode
                var dvsPostResponseData = JsonSerializer.Deserialize<CheckDVSPostResponseData>(response.JsonResponse,
                     new JsonSerializerOptions
                     {
                         PropertyNameCaseInsensitive = true
                     });
                var resultCode = dvsPostResponseData.Data.VerifyDocumentResult.VerificationResultCode;

                result = resultCode == "N"
                       ? 0
                       : 1;
            }

            // Save DVS Checks
            var dvsChecks = new DVSChecks();
            dvsChecks.RequestDate = DateTime.UtcNow;
            dvsChecks.Result = result;
            dvsChecks.Message = JsonSerializer.Serialize(response);
            dvsChecks.ApplicationId = model.InspectionId;
            inspectionRepository.SaveDVSChecks(dvsChecks);

            // Update DVS Status
            inspectionRepository.UpdateDVSStatus(model.InspectionId, result, userId);

            return result;
        }

        #endregion

        #region Ping PPSR

        public void PingPPSR(Models.PPSRModel model, out string result, out HttpStatusCode httpStatusCode)
        {
            result = "";
            httpStatusCode = new HttpStatusCode();

            using (var client = new HttpClient())
            {
                var requestUri = "api/ppsr/ping";
                var externalApiUrl = Startup.AppConfiguration.GetSection("BaseURL").GetSection("ExternalApiURL").Value;
                var responseData = ExtensionMethods<Models.PPSRModel>
                                   .PostJsonDatas(client, externalApiUrl, requestUri, null, model, null)
                                   .Result;

                if (responseData.JsonResponse.IsValidJson<Models.PPSRModel>())
                {
                    // Deserialize json data to Class
                    var apiPostResponseData = JsonSerializer.Deserialize<ApiPostResponseData>(responseData.JsonResponse,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                    if (string.IsNullOrEmpty(apiPostResponseData.ErrorMessage))
                    {
                        if (apiPostResponseData.Message != null)
                        {
                            httpStatusCode = (HttpStatusCode)responseData.StatusCode;
                            result = Convert.ToString(apiPostResponseData.Message);
                        }
                    }
                    else
                    {
                        httpStatusCode = HttpStatusCode.InternalServerError;
                        result = apiPostResponseData.ErrorMessage;
                    }
                }
                else
                {
                    httpStatusCode = (HttpStatusCode)responseData.StatusCode;
                    result = Convert.ToString(responseData.JsonResponse);
                }
            }
        }

        #endregion

        #region Get User Mapped Inspection Details Count

        public int GetUserMappedInspectionDetailsCount(string userGuid, string companyGuid)
        {
            var response = inspectionRepository.GetUserMappedInspectionDetailsCount(userGuid, companyGuid);

            return response;
        }

        #endregion

        #region Get Inspection Details By Id

        public Applications GetInspectionDetailsById(long inspectionId)
        {
            var response = inspectionRepository.GetInspectionDetails(inspectionId);

            return response;
        }

        #endregion

        #region Get Inspection Details By Guid

        public Applications GetInspectionDetailsByGuid(string inspectionGuid)
        {
            var response = inspectionRepository.GetInspectionDetails(inspectionGuid);

            return response;
        }

        #endregion

        #region Get AppImage Detail

        public AppImages GetAppImageDetail(long imageId)
        {
            var response = inspectionRepository.GetAppImageDetailsById(imageId);

            return response;
        }

        public AppImages GetAppImageDetail(long imageId, long inspectionId)
        {
            var response = inspectionRepository.GetAppImageDetailsById(imageId, inspectionId);

            return response;
        }

        #endregion

        #region Verify Inspection Permission

        public bool VerifyInspectionPermission(long inspectionId, string userGuid)
        {
            var response = inspectionRepository.VerifyInspectionPermission(inspectionId, userGuid);

            return response;
        }

        #endregion

        #region Get Illion Details

        public Models.GetIllionDetailsResponse GetIllionDetails(string companyGuid)
        {
            Models.GetIllionDetailsResponse response = null;

            // Get Illion Details
            var illionIntegrationDetails = inspectionRepository.GetIllionDetailsByCompanyGuid(companyGuid);
            if (illionIntegrationDetails != null)
            {
                response = new Models.GetIllionDetailsResponse();
                response.IsActive = illionIntegrationDetails.IsActive;
                response.ReferralCode = illionIntegrationDetails.ReferralCode;
                response.IsStatementRequired = illionIntegrationDetails.IsStatementRequired;
            }

            return response;
        }

        #endregion

        #region Save Illion Details

        public void SaveIllionDetails(string companyGuid, Models.UpdateIllionDetailsRequest model)
        {
            // Get Illion Details
            var illionIntegrationDetails = inspectionRepository.GetIllionDetailsByCompanyGuid(companyGuid);

            // Save Illion Details
            var illionDetails = illionIntegrationDetails == null
                              ? new IllionIntegrationDetails()
                              : illionIntegrationDetails;
            illionDetails.CompanyGuid = companyGuid;
            illionDetails.ReferralCode = model.ReferralCode;
            illionDetails.IsStatementRequired = model.IsStatementRequired;
            illionDetails.IsActive = model.IsActive;
            inspectionRepository.SaveIllionDetails(illionDetails);
        }

        #endregion

        #region Get App Users

        public List<AppUsers> GetAppUsers(long inspectionId)
        {
            var responses = inspectionRepository.GetAppUsers(inspectionId);

            return responses;
        }

        #endregion

        #region Private Methods

        #region Add App Activity Logs

        private void AddAppActivityLogs(long appActivityId, long userTypeId, long applicationId, string userGuid, int processDuration)
        {
            var appActivityLogs = new AppActivityLogs();
            appActivityLogs.AppActivityId = appActivityId;
            appActivityLogs.UserType = (int)userTypeId;
            appActivityLogs.ApplicationId = applicationId;
            appActivityLogs.ProcessedTime = DateTime.UtcNow;
            appActivityLogs.UserGuid = userGuid;
            appActivityLogs.IsNotified = false;
            appActivityLogs.IsWebAppUser = false;
            appActivityLogs.ProcessDuration = processDuration;
            inspectionRepository.AddAppActivityLogs(appActivityLogs);
        }

        #endregion

        #region Send Inspection Shared Email

        public void SendInspectionSharedEmail(long userTypeId, long inspectionId, Models.TemplateSets asset,
                                              List<string> usersToShare, Entities.ADUsers userDetails)
        {
            if (usersToShare != null &&
                usersToShare.Count() > 0)
            {
                string ownerBrokerName = userDetails != null
                                       ? string.Join(" ", userDetails.Name, userDetails.SurName)
                                       : "";

                string assetName = asset != null
                                 ? asset.TemplateName
                                 : "";

                string inspectionLink = string.Format(Startup.AppConfiguration.GetSection("AppSettings").GetSection("InspectionLink").Value,
                                                      inspectionId);

                var sharedUsers = dataRepository.GetUsersDetailByUserGuid(usersToShare);

                usersToShare.ForEach(
                    user =>
                    {
                        // Add App Activity Logs
                        AddAppActivityLogs((int)Models.Enums.AppActivityLogs.InspectionShared, userTypeId, inspectionId, user, 0);

                        var sharedUser = sharedUsers.FirstOrDefault(u => u.UserGuid == user);

                        string sharedBrokerEmail = sharedUser != null
                                                 ? sharedUser.Email
                                                 : "";

                        string sharedBrokerName = sharedUser != null
                                                ? string.Join(" ", sharedUser.Name, sharedUser.SurName)
                                                : "";

                        emailHelper.SendInspectionShareEmail(ownerBrokerName, sharedBrokerEmail, sharedBrokerName,
                                                             inspectionId, assetName, inspectionLink);
                    });
            }
        }

        #endregion

        #endregion
    }
}
