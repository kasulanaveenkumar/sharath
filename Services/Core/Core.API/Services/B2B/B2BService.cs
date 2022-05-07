using Core.API.Entities;
using Core.API.Repositories.B2B;
using System.Collections.Generic;
using System.Linq;
using System;
using Common.AzureBlobUtility.Models;
using Common.AzureBlobUtility.Helper;
using Core.API.Repositories;
using System.Net.Http;
using Common.Extensions;
using System.Text.Json;
using Core.API.Models.B2B;
using Core.API.Models;
using Common.Messages;
using Common.Notifications.SMS;
using Core.API.Helper;

namespace Core.API.Services.B2B
{
    public class B2BService : IB2BService
    {
        private readonly IB2BRepository b2bRepository;
        private readonly IInspectionRepository inspectionRepository;
        private readonly IInspectionService inspectionService;
        private readonly IDataRepository dataRepository;
        private readonly AzureBlobHelper azureBlobHelper;
        private readonly SmsSender smsSender;
        private readonly EmailHelper emailHelper;

        #region Constructor

        public B2BService(IB2BRepository repository, IInspectionRepository inspectionRepository,
                          IInspectionService inspectionService, IDataRepository dataRepository)
        {
            this.b2bRepository = repository;
            this.inspectionRepository = inspectionRepository;
            this.inspectionService = inspectionService;
            this.dataRepository = dataRepository;

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
        }

        #endregion

        #region Get Inspections List

        public List<AllInspectionResponse> GetInspectionsList(AllInspectionRequest model)
        {
            var responses = new List<AllInspectionResponse>();

            var b2bInspections = b2bRepository.GetInspectionsList(model);

            var inpsectionIds = b2bInspections.Select(i => i.Id).Distinct().ToList();
            inpsectionIds.ForEach(
                inspectionId =>
                {
                    var response = new AllInspectionResponse();

                    var inspectionDetails = b2bInspections.Where(i => i.Id == inspectionId).FirstOrDefault();
                    if (inspectionDetails != null)
                    {
                        // Inspection Details
                        response.Id = inspectionId;
                        response.ApplicationStatus = inspectionDetails.ApplicationStatus;
                        response.ApplicationStatusId = inspectionDetails.ApplicationStatusId;
                        response.ReferenceNumber = inspectionDetails.ReferenceNumber;
                        response.ExternalRef = inspectionDetails.ExternalRef;
                        response.CreationDate = inspectionDetails.CreationDate;

                        // Document Details
                        response.TotalDocuments = inspectionDetails.TotalDocuments;
                        response.DocumentsPending = inspectionDetails.DocumentsPending;
                        response.DocumentsUploaded = inspectionDetails.DocumentsUploaded;
                        response.DocumentsAccepted = inspectionDetails.DocumentsAccepted;
                        response.DocumentsRejected = inspectionDetails.DocumentsRejected;
                        response.DocumentsProcessed = inspectionDetails.DocumentsProcessed;
                        response.UploadPercentage = (short)((response.DocumentsUploaded / response.TotalDocuments) * 100);
                        response.CompletionPercentage = (short)((response.DocumentsProcessed / response.TotalDocuments) * 100);
                        response.IsSellerActionRequired = inspectionDetails.IsSellerActionRequired;

                        // Seller Details
                        response.Seller.Name = inspectionDetails.SellerName;
                        response.Seller.SurName = inspectionDetails.SellerSurName;
                        response.Seller.Email = inspectionDetails.SellerEmail;
                        response.Seller.PhoneNumber = inspectionDetails.SellerMobile;

                        // Buyer Details
                        response.Buyer.Name = inspectionDetails.BuyerName;
                        response.Buyer.SurName = inspectionDetails.BuyerSurName;
                        response.Buyer.Email = inspectionDetails.BuyerEmail;
                        response.Buyer.PhoneNumber = inspectionDetails.BuyerMobile;

                        // Company Details
                        response.Company.CompanyName = inspectionDetails.CompanyName;

                        // Primary Broker
                        var primaryBroker = b2bInspections.Where(i => i.Id == inspectionId &&
                                                                      i.IsOwner == true).FirstOrDefault();
                        if (primaryBroker != null)
                        {
                            response.PrimaryBroker.Name = primaryBroker.BrokerName;
                            response.PrimaryBroker.SurName = primaryBroker.BrokerSurName;
                            response.PrimaryBroker.Email = primaryBroker.BrokerEmail;
                            response.PrimaryBroker.Mobile = primaryBroker.BrokerMobile;
                        }

                        // Shared Brokers
                        response.SharedBrokers = b2bInspections.Where(i => i.Id == inspectionId &&
                                                                           i.IsOwner == false)
                                                .Select(b => new BrokerDetails()
                                                {
                                                    Name = b.BrokerName,
                                                    SurName = b.BrokerSurName,
                                                    Email = b.BrokerEmail,
                                                    Mobile = b.BrokerMobile
                                                })
                                                .ToList();

                        responses.Add(response);
                    }
                });

            return responses;
        }

        #endregion

        #region Get Inspection Details

        public Applications GetInspectionDetails(long inspectionId, string companyGuid, string exterRef = "")
        {
            var response = b2bRepository.GetInspectionDetails(inspectionId, companyGuid, exterRef);

            return response;
        }

        #endregion

        #region Get AppUsers

        public List<AppUsers> GetAppUsers(long inspectionId)
        {
            var responses = b2bRepository.GetAppUsers(inspectionId);

            return responses;
        }

        #endregion

        #region Cancel Inspection

        public void CancelInspection(Models.B2B.CancelInspectionRequest model, Applications application)
        {
            // Get User Details By Email
            var userDetails = b2bRepository.GetUserDetailsByEmail(model.BrokerEmail);

            application.ApplicationStatus = (int)Models.Enums.ApplicationStatus.Cancelled;

            application.UpdatedTime = DateTime.UtcNow;
            application.UpdatedBy = userDetails.UserId;

            b2bRepository.CancelInspection(application);
        }

        #endregion

        #region Update Inspection

        public void UpdateInspection(Applications application, UpdateInspectionRequest model, long userId)
        {
            if (model.Buyer == null &&
                model.Seller == null)
            {
                // If RefNumber not null
                if (!string.IsNullOrEmpty(model.RefNumber))
                {
                    application.RefNumber = model.RefNumber;
                }

                // If ExternalRefNumber not null
                if (!string.IsNullOrEmpty(model.ExternalRefNumber))
                {
                    application.ExternalRefNumber = model.ExternalRefNumber;
                }

                application.UpdatedTime = DateTime.UtcNow;
                application.UpdatedBy = userId;

                b2bRepository.UpdateInspection(application);
            }
        }

        #endregion

        #region Update AppUsers

        public void UpdateAppUsers(List<AppUsers> appUsers, UpdateInspectionRequest model)
        {
            // Update Buyer Details
            var buyerDetails = appUsers.Where(au => au.Role == (int)Models.Enums.Role.Buyer).FirstOrDefault();
            if (buyerDetails != null)
            {
                if (model.Buyer != null)
                {
                    // If Name not null
                    if (!string.IsNullOrEmpty(model.Buyer.Name))
                    {
                        buyerDetails.Name = model.Buyer.Name;
                    }

                    // If SurName not null
                    if (!string.IsNullOrEmpty(model.Buyer.SurName))
                    {
                        buyerDetails.SurName = model.Buyer.SurName;
                    }

                    // If Email not null
                    if (!string.IsNullOrEmpty(model.Buyer.Email))
                    {
                        buyerDetails.Email = model.Buyer.Email;
                    }

                    // If PhoneNumber not null
                    if (!string.IsNullOrEmpty(model.Buyer.PhoneNumber))
                    {
                        buyerDetails.PhoneNumber = model.Buyer.PhoneNumber;
                    }

                    b2bRepository.UpdateAppUsers(buyerDetails);
                }
            }

            // Update Seller Details
            var sellerDetails = appUsers.Where(au => au.Role == (int)Models.Enums.Role.Seller).FirstOrDefault();
            if (sellerDetails != null)
            {
                if (model.Seller != null)
                {
                    // If Name not null
                    if (!string.IsNullOrEmpty(model.Seller.Name))
                    {
                        sellerDetails.Name = model.Seller.Name;
                    }

                    // If SurName not null
                    if (!string.IsNullOrEmpty(model.Seller.SurName))
                    {
                        sellerDetails.SurName = model.Seller.SurName;
                    }

                    // If Email not null
                    if (!string.IsNullOrEmpty(model.Seller.Email))
                    {
                        sellerDetails.Email = model.Seller.Email;
                    }

                    // If PhoneNumber not null
                    if (!string.IsNullOrEmpty(model.Seller.PhoneNumber))
                    {
                        sellerDetails.PhoneNumber = model.Seller.PhoneNumber;
                    }

                    b2bRepository.UpdateAppUsers(sellerDetails);
                }
            }

            // Save Db Changes
            b2bRepository.SaveDbChanges();
        }

        #endregion

        #region Get Report

        public string GetReport(Applications application)
        {
            var filePath = string.Format("{0}/{1}.pdf", application.Id, application.InspectionGuid);
            var base64String = "";
            ImageProperties props = null;
            azureBlobHelper.GetBlobFromAzureStorageAdditionalContainer(filePath, ref base64String, out props, false, false);

            if (string.IsNullOrEmpty(base64String))
                return null;

            return base64String.Split(",")[1];
        }

        #endregion

        #region Create New Inspection

        public long CreateNewInspection(CreateInspectionRequest model, string companyGuid,
                                        out string paymentFailedReason, out Exception errorMessage)
        {
            paymentFailedReason = "";
            errorMessage = null;

            // Get Users
            var brokers = model.Brokers;
            b2bRepository.GetUsersToShare(ref brokers, companyGuid);
            model.Brokers = brokers;

            var primaryUser = brokers.FirstOrDefault(b => b.IsOwner == true);
            if (primaryUser == null)
            {
                errorMessage = new Exception("Inspection owner is required");
                return -1;
            }

            if (brokers.Count(b => b.IsOwner == true) > 1)
            {
                errorMessage = new Exception("Multiple owner found.");
                return -2;
            }

            if (brokers.Count(b => b.UserGuid == null) > 0)
            {
                errorMessage = new Exception(String.Format("Invalid brokers({0})",
                                                            String.Join(",", brokers.Where(b => b.UserGuid == null).Select(b => b.Email))));
                return -3;
            }

            // Get New Inspection Details
            var newInspectionDetaisRequest = new NewInspectionDetailsRequest();
            newInspectionDetaisRequest.UserGuid = primaryUser.UserGuid;
            newInspectionDetaisRequest.CompanyGuid = companyGuid;
            newInspectionDetaisRequest.TemplateGuid = model.AssetGuid;
            newInspectionDetaisRequest.planGuid = model.PlanGuid;
            newInspectionDetaisRequest.LenderGuid = model.LenderGuid;
            newInspectionDetaisRequest.StateId = b2bRepository.GetStateIdByCode(model.StateCode);
            newInspectionDetaisRequest.IsIncludeNoLenderPreference = false;
            var newInspectionDetailsResponse = new NewInspectionDetailsResponse();
            using (var client = new HttpClient())
            {
                var configApiUrl = Startup.AppConfiguration.GetSection("BaseURL").GetSection("ConfigApiURL").Value;
                var requestUri = "apiB2B/v1/getnewinspectiondetails";
                var responseData = ExtensionMethods<NewInspectionDetailsRequest>
                                   .PostJsonDatas(client, configApiUrl, requestUri, "", newInspectionDetaisRequest)
                                   .Result;

                // Deserialize json data to Class
                newInspectionDetailsResponse = JsonSerializer.Deserialize<NewInspectionDetailsResponse>(responseData,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }

            // Create New Inspection
            paymentFailedReason = string.Empty;
            errorMessage = null;
            var documents = new List<TemplateDocument>();
            var newInspectionRequest = new NewInspectionRequest();
            newInspectionRequest.Buyer = model.Buyer;
            newInspectionRequest.Seller = model.Seller;
            newInspectionRequest.LenderGuid = model.LenderGuid;
            newInspectionRequest.LenderRef = model.LenderRef;
            newInspectionRequest.ExternalRef = model.ExternalRef;
            newInspectionRequest.TemplateSetGuid = model.AssetGuid;
            newInspectionRequest.TemplateSetPlanGuid = model.PlanGuid;
            newInspectionRequest.StateCode = model.StateCode;
            newInspectionRequest.PaymentMethodId = "";
            var documentDetails = newInspectionDetailsResponse.TemplateDetails.DocumentDetails;
            documentDetails.ForEach(
                documentDetail =>
                {
                    var templateImages = new List<TemplateImage>();
                    documentDetail.ImageDetails.ForEach(
                        imageDetail =>
                        {
                            // Add Image Detail
                            templateImages.Add(
                                new TemplateImage()
                                {
                                    ImageName = imageDetail.ImageName,
                                    DocGroup = (short)imageDetail.DocGroup,
                                    ImageType = (short)imageDetail.ImageType,
                                    Position = (short)imageDetail.Position,
                                    IsMandatory = imageDetail.IsMandatory,
                                    IsDefaultSelected = imageDetail.IsDefaultSelected
                                });
                        });

                    // Add Document Detail
                    documents.Add(
                        new TemplateDocument()
                        {
                            DocumentId = documentDetail.DocumentId,
                            DocumentName = documentDetail.DocumentName,
                            IsAdditionalDataMandatory = documentDetail.IsAdditionalDataMandatory,
                            IsAdditionalDataRequired = documentDetail.IsAdditionalDataRequired,
                            ImageDetails = templateImages,
                            Position = (short)documentDetail.Position,
                            DocumentRequired = documentDetail.DocumentRequired
                        });
                });

            newInspectionRequest.Documents = documents;
            newInspectionRequest.UsersToShare = brokers.Where(b => b.IsOwner == false).Select(b => b.UserGuid).ToList();

            var response = inspectionService.CreateNewInspection(newInspectionRequest, primaryUser.UserId, "", companyGuid, primaryUser.UserGuid,
                                                                (int)Common.Models.Enums.UserTypes.Broker,
                                                                 out paymentFailedReason, out errorMessage);

            return response;
        }

        #endregion

        #region Send Inspection Created Email

        public void SendInspectionCreatedEmail(long inspectionId)
        {
            // Get Inspection Details By Id
            var inspectionDetails = b2bRepository.GetInspectionDetailsById(inspectionId);
            if (inspectionDetails != null)
            {
                // Get User Details By Id
                var userDetails = dataRepository.GetUserDetailsByUserId(inspectionDetails.CreatedBy);

                // Get Seller and Buyer
                var appUsers = b2bRepository.GetAppUsers(inspectionId);
                var sellerDetails = appUsers.Where(u => u.Role == (int)Enums.Role.Seller)
                                    .Select(u => new AppUser()
                                    {
                                        Name = u.Name,
                                        SurName = u.SurName,
                                        Email = u.Email,
                                        PhoneNumber = u.PhoneNumber
                                    }).FirstOrDefault();
                var buyerDetails = appUsers.Where(u => u.Role == (int)Enums.Role.Buyer)
                                   .Select(u => new AppUser()
                                   {
                                       Name = u.Name,
                                       SurName = u.SurName,
                                       Email = u.Email,
                                       PhoneNumber = u.PhoneNumber
                                   }).FirstOrDefault();

                // Get Asset Type
                var asset = inspectionRepository.GetAllAssets().FirstOrDefault(a => a.TemplateGuid == inspectionDetails.TemplateSetGuid);
                var assetType = asset != null
                              ? asset.TemplateName
                              : "";

                //Get Lender Name
                var lender = inspectionRepository.GetAllLenders().FirstOrDefault(a => a.LenderGuid == inspectionDetails.LenderCompanyGuid);
                var lenderName = lender != null
                               ? lender.LenderName
                               : "";

                // Get Documents
                var appDocuments = inspectionRepository.GetInspectionDocumentsList(inspectionId, null);
                var documents = appDocuments.Select(d => new TemplateDocument()
                {
                    DocumentId = d.DocId,
                    DocumentName = d.DocumentName
                }).ToList();

                // Get WebappShortLink
                var webAppShortLink = inspectionDetails.WebAppShortLink;

                // Send Inspection Created Email
                var userName = userDetails != null
                                         ? string.Join(" ", userDetails.Name, userDetails.SurName)
                                         : "";
                var userEmail = userDetails != null
                              ? userDetails.Email
                              : "";
                var userMobile = userDetails != null
                               ? userDetails.Mobile
                               : "";

                var sellerEmail = sellerDetails != null
                                ? sellerDetails.Email
                                : "";
                var sellerName = sellerDetails != null
                               ? string.Join(" ", sellerDetails.Name, sellerDetails.SurName)
                               : "";
                var buyerName = buyerDetails != null
                              ? string.Join(" ", buyerDetails.Name, buyerDetails.SurName)
                              : "";

                // Send New Inspection Email
                emailHelper.SendNewInspectionEmail(sellerEmail, sellerName, userName, userEmail, userMobile, documents, assetType,
                                                   webAppShortLink, lenderName, buyerName, inspectionId.ToString());
            }
        }

        #endregion

        #region Send Inspection Created Sms

        public void SendInspectionCreatedSms(long inspectionId)
        {
            // Get Inspection Details By Id
            var inspectionDetails = b2bRepository.GetInspectionDetailsById(inspectionId);
            if (inspectionDetails != null)
            {
                // Get Seller and Buyer
                var appUsers = b2bRepository.GetAppUsers(inspectionId);
                var sellerDetails = appUsers.Where(u => u.Role == (int)Enums.Role.Seller)
                                    .Select(u => new AppUser()
                                    {
                                        Name = u.Name,
                                        SurName = u.SurName,
                                        Email = u.Email,
                                        PhoneNumber = u.PhoneNumber
                                    }).FirstOrDefault();
                var buyerDetails = appUsers.Where(u => u.Role == (int)Enums.Role.Buyer)
                                   .Select(u => new AppUser()
                                   {
                                       Name = u.Name,
                                       SurName = u.SurName,
                                       Email = u.Email,
                                       PhoneNumber = u.PhoneNumber
                                   }).FirstOrDefault();

                // Get Asset Type
                var asset = inspectionRepository.GetAllAssets().FirstOrDefault(a => a.TemplateGuid == inspectionDetails.TemplateSetGuid);
                var assetType = asset != null
                              ? asset.TemplateName
                              : "";

                //Get Lender Name
                var lender = inspectionRepository.GetAllLenders().FirstOrDefault(a => a.LenderGuid == inspectionDetails.LenderCompanyGuid);
                var lenderName = lender != null
                               ? lender.LenderName
                               : "";

                // Get WebappShortLink
                var webAppShortLink = inspectionDetails.WebAppShortLink;

                var buyerName = buyerDetails != null
                              ? string.Join(" ", buyerDetails.Name, buyerDetails.SurName)
                              : "";
                var sellerName = sellerDetails != null
                               ? string.Join(" ", sellerDetails.Name, sellerDetails.SurName)
                               : "";
                var sellerEmail = sellerDetails != null
                                ? sellerDetails.Email
                                : "";
                var sellerPhoneNumber = sellerDetails != null
                                      ? sellerDetails.PhoneNumber
                                      : "";
                if (string.IsNullOrEmpty(buyerName))
                {
                    smsSender.SendSMS(sellerPhoneNumber, string.Format(NotificationMessages.InspectionCreationSMSWithoutBuyer,
                                                                       sellerName, assetType, lenderName,
                                                                       webAppShortLink, inspectionId), sellerEmail);
                }
                else
                {
                    smsSender.SendSMS(sellerPhoneNumber, string.Format(NotificationMessages.InspectionCreationSMSWithBuyer,
                                                                       sellerName, buyerName, assetType, lenderName,
                                                                       webAppShortLink, inspectionId), sellerEmail);
                }
            }
        }

        #endregion

        #region Send Reminder

        public int SendReminder(ReminderRequest model, long userId)
        {
            var response = inspectionService.SendReminder(model, userId);

            return response;
        }

        #endregion

        #region Get WebHooks By Id

        public B2BWebHooks GetWebHookById(long id)
        {
            var response = b2bRepository.GetWebHookById(id);

            return response;
        }

        #endregion

        #region Get WebHooks By Company

        public List<B2BWebHooks> GetWebHooksByCompany(string companyGuid)
        {
            var responses = b2bRepository.GetAllWebHooksByCompany(companyGuid);

            return responses;
        }

        #endregion

        #region Get All WebHooks By Company

        public List<WebHookSubscribed> GetAllWebBooksByCompany(string companyGuid)
        {
            var allWebHooks = b2bRepository.GetAllWebHooksByCompany(companyGuid);

            var responses = allWebHooks.Select(
                                wh => new WebHookSubscribed()
                                {
                                    Id = wh.Id,
                                    TargetUrl = wh.TargetUrl,
                                    Event = Convert.ToString((Enums.ApplicationStatus)wh.EventId)
                                }).ToList();

            return responses;
        }

        #endregion

        #region Subscribe WebHooks

        public void SubscribeWebHooks(WebHookSubscribe model, string companyGuid)
        {
            var b2bWebHooks = new B2BWebHooks();

            b2bWebHooks.TargetUrl = model.TargetUrl;
            b2bWebHooks.EventId = (short)(Enum.Parse(typeof(Enums.ApplicationStatus), model.Event));
            b2bWebHooks.CompanyGuid = companyGuid;

            b2bRepository.AddWebHook(b2bWebHooks);
        }

        #endregion

        #region Unsubscribe WebHooks

        public void UnsubscribeWebHooks(B2BWebHooks b2bWeHook)
        {
            b2bRepository.DeleteWebHook(b2bWeHook);
        }

        #endregion

        #region UnsubscribeAll WebHooks

        public void UnsubscribeAllWebHooks(List<B2BWebHooks> b2bWebHooks)
        {
            b2bRepository.DeleteAllWebHooks(b2bWebHooks);
        }

        #endregion
    }
}