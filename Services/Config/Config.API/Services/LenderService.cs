using Config.API.Entities;
using Config.API.Models;
using Config.API.Models.Lender;
using Config.API.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Config.API.Services
{
    public class LenderService : ILenderService
    {
        private readonly ILenderRepository lenderRepository;
        private readonly ICompanyRepository companyRepository;

        #region Constructor
        public LenderService(ILenderRepository repository, ICompanyRepository companyRepository)
        {
            this.lenderRepository = repository;
            this.companyRepository = companyRepository;
        }
        #endregion

        #region Get Company Visibility

        public int GetCompanyVisibility(Companies companyDetails)
        {
            // Get Company Visibility
            var response = companyDetails.LenderVisibility;

            return response;
        }

        #endregion

        #region Save Company Visibility

        public int SaveCompanyVisibility(Companies companyDetails, SaveCompanyVisibilityRequest model)
        {
            int result = 0;

            //Compare Visibility 
            if (companyDetails.LenderVisibility != model.Visibility)
            {
                companyDetails.LenderVisibility = model.Visibility;

                //Save Company Visibility
                lenderRepository.SaveCompanyVisibility(companyDetails);

                result = 1;
            }
            else
            {
                result = -1;
            }

            return result;
        }

        #endregion

        #region Get lender Company details

        public Companies GetLenderCompanyDetailsByGuid(string lendercompanyGuid)
        {
            // Get lender Company details
            var response = lenderRepository.GetCompanyDetailsByGuid(lendercompanyGuid);

            return response;
        }

        #endregion

        #region Get Broker Companies

        public List<BrokerCompanyResponse> GetBrokerCompanies(string companyGuid)
        {
            // Get CompanyId
            var companyId = lenderRepository.GetLenderCompanyByGuid(companyGuid).Select(c => c.Id).FirstOrDefault();

            // Get Broker Companies
            var brokerCompanies = lenderRepository.GetBrokerCompanies();
            var responses = lenderRepository.GetBrokerCompanies(brokerCompanies, companyId);

            return responses;
        }

        #endregion

        #region Update LenderVisibility

        public void UpdateLenderVisibility(Entities.Companies companyDetails, List<SaveLenderVisibilityRequest> model)
        {
            // Remove Mapped Lenders
            companyRepository.RemoveMappedLenders(companyDetails.Id);

            // Get Visible Lenders
            var visibleLenders = model.Where(l => l.IsVisible == true).ToList();
            visibleLenders.ForEach(l =>
            {
                // Add BrokerLenderMappings
                companyRepository.AddBrokerLenderMappings(l.BrokerCompanyId, companyDetails.Id);
            });

            companyRepository.SaveDbChanges();
        }

        #endregion

        #region Get Blocked BrokerUsers

        public List<LenderBlockedBrokerUsersResponse> GetBlockedBrokerUsers(string brokerCompanyGuid, string lenderCompanyGuid)
        {
            // Get Broker Users
            var brokerUsers = lenderRepository.GetBrokerUsersByCompany(brokerCompanyGuid);

            // Get Broker CompanyId
            // Get Lender CompanyId
            var brokerCompanyId = lenderRepository.GetBrokerCompanyByGuid(brokerCompanyGuid).Select(c => c.Id).FirstOrDefault();
            var lenderCompanyId = lenderRepository.GetLenderCompanyByGuid(lenderCompanyGuid).Select(c => c.Id).FirstOrDefault();

            // Get Blocked Users List
            var blockedUsersList = lenderRepository.GetBlockedBrokerUsersList(brokerCompanyId, lenderCompanyId);

            // Get Blocked BrokerUsers
            var responses = lenderRepository.GetBlockedBrokerUsers(brokerUsers, blockedUsersList);

            return responses;
        }

        #endregion

        #region Save Blocked BrokerUsers

        public void SaveBlockedBrokerUsers(LenderBlockedBrokerUsersRequest model, string lenderCompanyGuid)
        {
            // Get Broker CompanyId
            // Get Lender CompanyId
            var brokerCompanyId = lenderRepository.GetBrokerCompanyByGuid(model.BrokerCompanyGuid).Select(c => c.Id).FirstOrDefault();
            var lenderCompanyId = lenderRepository.GetLenderCompanyByGuid(lenderCompanyGuid).Select(c => c.Id).FirstOrDefault();

            // Get Blocked BrokerUsers List
            var blockedUsersList = lenderRepository.GetBlockedBrokerUsersList(brokerCompanyId, lenderCompanyId);

            // Get Allowed Users List
            var allowedUsers = model.BlockedUsers.Where(u => u.IsAllowed == true).ToList();
            allowedUsers.ForEach(
                user =>
                {
                    // Remove Blocked BrokerUsers
                    var userData = blockedUsersList.FirstOrDefault(bul => bul.UserGuid == user.UserGuid);
                    if (userData != null)
                    {
                        lenderRepository.RemoveBlockedBrokerUsers(userData);
                    }
                });

            // Get Blocked Users List
            var blockedUsers = model.BlockedUsers.Where(u => u.IsAllowed == false).ToList();
            blockedUsers.ForEach(
                user =>
                {
                    // Add Blocked BrokerUsers
                    var userData = blockedUsersList.FirstOrDefault(bul => bul.UserGuid == user.UserGuid);
                    if (userData == null)
                    {
                        var newUser = new LenderBlockedBrokerUsers();
                        newUser.BrokerCompanyId = brokerCompanyId;
                        newUser.LenderCompanyId = lenderCompanyId;
                        newUser.UserGuid = user.UserGuid;
                        lenderRepository.AddBlockedBrokerUsers(newUser);
                    }
                });

            lenderRepository.SaveDbChanges();
        }

        #endregion

        #region Get Mapped Assets

        public TemplateSetandStates GetMappedAssetsStates(string companyGuid)
        {
            var response = new TemplateSetandStates();

            // Get Mapped Assets
            var templateSets = lenderRepository.GetMappedAssets(companyGuid);
            response.TemplateSets = templateSets;

            // Get State Options
            response.StateOptions = lenderRepository.GetStateoption();

            return response;
        }

        #endregion

        #region Get Asset Document Details

        public InspectionPlanDetailReponse GetAssetDocumentDetails(Entities.TemplateSets templateDetails, string templateGuid, 
                                                                   bool isAppliedAllStates, bool isUseDbValue, string companyGuid)
        {
            var response = new InspectionPlanDetailReponse();

            // Get available plan details for template
            var templateSetPlans = lenderRepository.GetTemplateSetPlans(templateDetails.Id, isAppliedAllStates, companyGuid);

            // Get TemplateSet Default Plans
            var templateDefaultPlans = lenderRepository.GetTemplateSetDefaultPlans(templateDetails.Id);

            response.TemplateSetGuid = templateGuid;

            // Apply Same Configuration to all states
            if (isUseDbValue)
            {
                response.IsApplyToAllStates = templateSetPlans.Count() > 0
                                            ? templateSetPlans.FirstOrDefault().IsAppliedAllState
                                            : true;
            }
            else
            {
                response.IsApplyToAllStates = isAppliedAllStates;
            }

            // Get Documents for Templates
            var documents = lenderRepository.GetTemplateDocuments(templateDetails.Id);

            // Get Custom Plan Documents for all plans
            var docMappings = lenderRepository.GetCustomDocuments(templateSetPlans.Select(p => p.TemplatePlanId).ToList());

            // Get Custom Plan Images for all plans
            var imageMappings = lenderRepository.GetCustomImages(templateSetPlans.Select(p => p.TemplatePlanId).ToList());

            // Get States
            var states = lenderRepository.GetStateoption();
            if (response.IsApplyToAllStates)
            {
                states.Clear();

                states.Add(
                    new StateOptionsResponse()
                    {
                        StateID = 0,
                        StateCode = "All"
                    });
            }

            // Get State Plans
            response.StatePlans = new List<StatePlansResponse>();
            states.ForEach(
                state =>
                {
                    var planDetails = templateSetPlans.Where(p => p.StateId == state.StateID).ToList();
                    if (planDetails.Count() == 0)
                    {
                        planDetails = templateDefaultPlans;
                    }
                    response.StatePlanDetails.Add(
                        new TemplateSetStatePlanDetailsResponse()
                        {
                            StateId = state.StateID,
                            StateCode = state.StateCode,
                            PlanDetails = planDetails
                        });
                    var customDocMappings = docMappings.Where(d => d.StateId == state.StateID).ToList();
                    var customImageMappings = imageMappings.Where(d => d.StateId == state.StateID).ToList();

                    var planDocs = new List<PlanDocumentResponse>();

                    documents.ForEach(
                        doc =>
                        {
                            planDocs.Add(GetPlanDocumentDetails(planDetails, customDocMappings, customImageMappings, doc));
                        });

                    response.StatePlans.Add(
                        new StatePlansResponse()
                        {
                            StateId = state.StateID,
                            StateCode = state.StateCode,
                            PlanDocuments = planDocs
                        });
                });

            return response;
        }

        #region Get Plan Document Details

        private PlanDocumentResponse GetPlanDocumentDetails(List<TemplateSetPlanDetailsResponse> templatePlans,
                                                            List<Entities.TemplateSetCustomDocMappings> docMappings,
                                                            List<Entities.TemplateSetCustomImageMappings> imageMappings,
                                                            AssetDocumentsListResponse doc)
        {
            var planDoc = new PlanDocumentResponse();

            // Add Documents
            // Add Document Plans
            var docDetails = docMappings.Where(d => d.TemplateDocumentId == doc.Id);
            planDoc.DocumentId = doc.Id;
            planDoc.Name = doc.Name;
            planDoc.Description = doc.Description;
            planDoc.WarningMessage = doc.WarningMessage;
            planDoc.IsShowAdditionalData = doc.HasAdditionalData;
            planDoc.IsAdditionalDataSelected = docDetails.Count() > 0
                                             ? docDetails.FirstOrDefault().IsAdditionalDataRequired
                                             : doc.HasAdditionalData;
            if (planDoc.IsAdditionalDataSelected == false)
                planDoc.HasAdditionalData = 0;
            else
            {
                if (docDetails.Count() > 0 && 
                    docDetails.FirstOrDefault().IsAdditionalDataMandatory == true)
                    planDoc.HasAdditionalData = 2;
                else
                    planDoc.HasAdditionalData = 1;
            }

            //if (docDetails.FirstOrDefault().IsAdditionalDataRequired == false)
            //    planDoc.HasAdditionalData = 0;
            //else
            //{
            //    if (docDetails.FirstOrDefault().IsAdditionalDataMandatory == false)
            //        planDoc.HasAdditionalData = 1;
            //    else
            //        planDoc.HasAdditionalData = 2;
            //}
            planDoc.IsAdditionalDataReadOnly = false;
            templatePlans.ForEach(
                plan =>
                {
                    var planDetails = docDetails.FirstOrDefault(d => d.CustomPlanId == plan.TemplatePlanId);
                    var isDocumentSelected = planDetails != null
                                           ? planDetails.IsSelected
                                           : (plan.TemplatePlanLevel >= doc.MinimumPlanLevelToInclude);
                    planDoc.DocPlanDetails.Add(
                        new PlanDocumentDetailsResponse()
                        {
                            TemplatePlanGuid = plan.TemplatePlanGuid,
                            IsShowDocument = (plan.TemplatePlanLevel >= doc.MinimumPlanLevelToInclude),
                            IsDocumentSelected = isDocumentSelected,
                            IsDocumentReadOnly = false
                        });
                });

            // Add Document Images
            // Add Document Image Plans
            var images = lenderRepository.GetDocumentImages(doc.Id, doc.CategoryId);
            images.ForEach(
                img =>
                {
                    var imgDetails = imageMappings.Where(i => i.TemplateImageId == img.Id);
                    var planImg = new PlanImageResponse();
                    planImg.ImageId = img.Id;
                    planImg.Name = img.Name;
                    planImg.Description = img.Description;
                    planImg.WarningMessage = img.WarningMessage;
                    planImg.IsShowMandatory = img.AllowSkip;
                    planImg.IsMandatorySelected = imgDetails.Count() > 0
                                                ? !imgDetails.FirstOrDefault().IsSkippable
                                                : img.AllowSkip;
                    planImg.IsMandatoryReadOnly = false;
                    templatePlans.ForEach(
                        plan =>
                        {
                            var planDetails = imgDetails.FirstOrDefault(d => d.CustomPlanId == plan.TemplatePlanId);
                            var isDocumentSelected = planDetails != null
                                                   ? planDetails.IsSelected
                                                   : (plan.TemplatePlanLevel >= doc.MinimumPlanLevelToInclude);

                            planImg.ImagePlanDetails.Add(
                                new PlanDocumentDetailsResponse()
                                {
                                    TemplatePlanGuid = plan.TemplatePlanGuid,
                                    IsShowDocument = (plan.TemplatePlanLevel >= doc.MinimumPlanLevelToInclude),
                                    IsDocumentSelected = isDocumentSelected,
                                    IsDocumentReadOnly = false
                                });
                        });
                    planDoc.ImageDetails.Add(planImg);
                });

            return planDoc;
        }

        #endregion

        #endregion

        #region Save Inspection Plans

        public void SaveInspectionPlans(Entities.TemplateSets templateDetails, InspectionPlanDetailRequest model, string companyGuid)
        {
            lenderRepository.BeginTransaction();

            try
            {
                var plansInfos = new List<PlanInfo>();

                // Get Available Plan Details For Template
                var templatePlans = lenderRepository.GetTemplateSetPlans(templateDetails.Id, model.IsApplyToAllStates, companyGuid);

                // Get TemplateSet Default Plans
                var templateDefaultPlans = lenderRepository.GetTemplateSetDefaultPlans(templateDetails.Id);

                // Get Company Details By Guid
                var companyDetails = companyRepository.GetCompanyByGuid(companyGuid);

                // Get TemplateSet CustomPlan Mappings
                var templateSetCustomPlanMappings = lenderRepository.GetTemplateSetCustomPlanMappings(templateDetails.Id, companyDetails.Id);

                model.StatePlanDetails.ForEach(
                    state =>
                    {
                        state.PlanDetails.ForEach(
                            plan =>
                            {
                                // Get Plan Detail By PlanGuid
                                var planDetail = templatePlans.FirstOrDefault(tp => tp.StateId == state.StateId &&
                                                                                    tp.TemplatePlanGuid == plan.TemplatePlanGuid);
                                if (planDetail == null)
                                {
                                    planDetail = templateDefaultPlans.FirstOrDefault(tp => tp.TemplatePlanGuid == plan.TemplatePlanGuid);
                                }

                                // Get Custom Plan Mappings Details
                                var planMappings = templateSetCustomPlanMappings.FirstOrDefault(p => p.StateId == state.StateId &&
                                                                                                     p.Id == planDetail.TemplatePlanId);

                                // Add TemplateSet CustomPlan Mappings Or
                                // Update TemplateSet CustomPlan Mappings
                                var customPlanMappings = new TemplateSetCustomPlanMappings();
                                if (planMappings == null)
                                {
                                    customPlanMappings.TemplateSetId = templateDetails.Id;
                                    customPlanMappings.PlanId = planDetail.TemplatePlanId;
                                    customPlanMappings.LenderCompanyId = companyDetails.Id;
                                    customPlanMappings.IsActive = plan.IsActivated;
                                    customPlanMappings.IsAppliedAllState = model.IsApplyToAllStates;
                                    customPlanMappings.StateId = state.StateId;
                                    lenderRepository.AddTemplateSetCustomPlanMappings(customPlanMappings);
                                    lenderRepository.SaveDbChanges();
                                }
                                else
                                {
                                    planMappings.IsActive = plan.IsActivated;
                                    planMappings.IsAppliedAllState = model.IsApplyToAllStates;
                                    planMappings.StateId = state.StateId;
                                    lenderRepository.UpdateTemplateSetCustomPlanMappings(planMappings);
                                    lenderRepository.SaveDbChanges();
                                }

                                var planGuid = plan.TemplatePlanGuid;
                                var planId = planMappings == null
                                           ? customPlanMappings.Id
                                           : planMappings.Id;

                                // Add PlanInfos
                                plansInfos.Add(
                                    new PlanInfo()
                                    {
                                        PlanGuid = planGuid,
                                        StateId = state.StateId,
                                        PlanId = planId
                                    });
                            });
                    });

                // Set IsAppliedToAllStates
                templateSetCustomPlanMappings.ForEach(
                    plan =>
                    {
                        plan.IsAppliedAllState = model.IsApplyToAllStates;
                        lenderRepository.UpdateTemplateSetCustomPlanMappings(plan);
                    });

                lenderRepository.SaveDbChanges();

                // Get Custom Plan Documents for all plans
                var customDocs = lenderRepository.GetCustomDocuments(plansInfos.Select(p => p.PlanId).ToList());

                // Get Custom Plan Documents for all plans
                var customImgs = lenderRepository.GetCustomImages(plansInfos.Select(p => p.PlanId).ToList());

                // For each states
                model.StatePlans.ForEach(
                    state =>
                    {
                        // For each documents for the state
                        state.PlanDocuments.ForEach(
                            doc =>
                            {
                                // For each document plan for the document
                                doc.DocPlanDetails.ForEach(
                                    docPlan =>
                                    {
                                        var planInfo = plansInfos.FirstOrDefault(p => p.StateId == state.StateId &&
                                                                                      p.PlanGuid == docPlan.TemplatePlanGuid);
                                        var custDoc = customDocs.FirstOrDefault(d => d.CustomPlanId == planInfo.PlanId &&
                                                                                     d.StateId == state.StateId &&
                                                                                     d.TemplateDocumentId == doc.DocumentId);
                                        if (custDoc == null)
                                        {
                                            lenderRepository.AddTemplateSetCustomDocMappings(
                                                new TemplateSetCustomDocMappings()
                                                {
                                                    CustomPlanId = planInfo.PlanId,
                                                    StateId = state.StateId,
                                                    StateCode = state.StateCode,
                                                    TemplateDocumentId = doc.DocumentId,
                                                    IsSelected = docPlan.IsDocumentSelected,
                                                    IsAdditionalDataRequired = doc.HasAdditionalData > 0,
                                                    IsAdditionalDataMandatory = doc.HasAdditionalData > 1
                                                });
                                        }
                                        else
                                        {
                                            custDoc.StateId = state.StateId;
                                            custDoc.StateCode = state.StateCode;
                                            custDoc.IsSelected = docPlan.IsDocumentSelected;
                                            custDoc.IsAdditionalDataRequired = doc.HasAdditionalData > 0;
                                            custDoc.IsAdditionalDataMandatory = doc.HasAdditionalData > 1;
                                            lenderRepository.UpdateTemplateSetCustomDocMappings(custDoc);
                                        }
                                    });

                                lenderRepository.SaveDbChanges();

                                // For each image
                                doc.ImageDetails.ForEach(
                                    imageDoc =>
                                    {
                                        // For each image plan
                                        imageDoc.ImagePlanDetails.ForEach(
                                            imagePlan =>
                                            {
                                                var planInfo = plansInfos.FirstOrDefault(p => p.StateId == state.StateId &&
                                                                                     p.PlanGuid == imagePlan.TemplatePlanGuid);
                                                var custImg = customImgs.FirstOrDefault(d => d.CustomPlanId == planInfo.PlanId &&
                                                                                             d.StateId == state.StateId &&
                                                                                             d.TemplateImageId == imageDoc.ImageId);

                                                if (custImg == null)
                                                {
                                                    lenderRepository.AddTemplateSetCustomImageMappings(
                                                        new TemplateSetCustomImageMappings()
                                                        {
                                                            CustomPlanId = planInfo.PlanId,
                                                            StateId = state.StateId,
                                                            StateCode = state.StateCode,
                                                            TemplateImageId = imageDoc.ImageId,
                                                            IsSelected = imagePlan.IsDocumentSelected,
                                                            IsSkippable = imageDoc.IsShowMandatory
                                                                        ? !imageDoc.IsMandatorySelected
                                                                        : false
                                                        });
                                                }
                                                else
                                                {
                                                    custImg.StateId = state.StateId;
                                                    custImg.StateCode = state.StateCode;
                                                    custImg.IsSelected = imagePlan.IsDocumentSelected;
                                                    custImg.IsSkippable = imageDoc.IsShowMandatory
                                                                        ? !imageDoc.IsMandatorySelected
                                                                        : false;
                                                    lenderRepository.UpdateTemplateSetCustomImageMappings(custImg);
                                                }
                                            });
                                    });

                                lenderRepository.SaveDbChanges();
                            });
                    });

                lenderRepository.CommitTransaction();
            }
            catch (Exception ex)
            {
                lenderRepository.RollbackTransaction();
                throw;
            }
        }

        #endregion

        public InspectionPlanDetailReponse GetTemplatePlanDetails(string templateGuid, string companyGuid)
        {
            var data = lenderRepository.GetTemplatePlanData(templateGuid, companyGuid);
            if (data.Count() == 0)
                return null;

            var response = new InspectionPlanDetailReponse();

            // Template Set Guid
            response.TemplateSetGuid = data[0].TemplateSetGuid;

            // Is Applied to all states?
            response.IsApplyToAllStates = data[0].IsAppliedAllState.Value;

            // State Plan Details
            var distinctStatePlans = data.Select(d => new
            {
                d.StateId,
                d.StateCode,
                d.PlanGuid,
                d.PlanName,
                d.PlanDescription,
                d.Price,
                d.IsActive,
                d.PlanLevel
            }).Distinct().ToList();

            response.StatePlanDetails = (from d in distinctStatePlans
                                         group d by new { d.StateId, d.StateCode } into grp
                                         select new TemplateSetStatePlanDetailsResponse()
                                         {
                                             StateId = grp.Key.StateId,
                                             StateCode = grp.Key.StateCode,
                                             PlanDetails = grp.Select(g =>
                                             new TemplateSetPlanDetailsResponse()
                                             {
                                                 TemplatePlanGuid = g.PlanGuid,
                                                 TemplatePlanName = g.PlanName,
                                                 TemplatePlanDescription = g.PlanDescription,
                                                 BasePrice = g.Price,
                                                 TemplatePlanLevel = g.PlanLevel,
                                                 IsActivated = g.IsActive == null ? false : g.IsActive.Value
                                             }).OrderBy(o => o.TemplatePlanLevel).Distinct().ToList()
                                         }).ToList();

            // Get distinct plans
            var distinctPlans = data.Select(d => new { d.PlanGuid }).Distinct().ToList();

            // State Plans
            response.StatePlans = new List<StatePlansResponse>();
            foreach (var state in data.Select(d => new { d.StateId, d.StateCode }).Distinct())
            {
                var statePlan = new StatePlansResponse();
                statePlan.StateId = state.StateId;
                statePlan.StateCode = state.StateCode;
                statePlan.PlanDocuments = new List<PlanDocumentResponse>();

                foreach (var docs in data.Select(d => new { d.DocId }).Distinct())
                {
                    var doc = data.Where(d => d.DocId == docs.DocId).FirstOrDefault();

                    var planDoc = new PlanDocumentResponse()
                    {
                        DocumentId = doc.DocId,
                        Name = doc.DocName,
                        Description = doc.DocDescription,
                        WarningMessage = doc.DocWarningMessage,
                        IsShowAdditionalData = doc.IsShowAdditionalData == null ? false : doc.IsShowAdditionalData.Value,
                        IsAdditionalDataSelected = doc.IsAdditionalDataSelected == null ? false : doc.IsAdditionalDataSelected.Value,
                    };

                    // Add Plan Details
                    planDoc.DocPlanDetails = new List<PlanDocumentDetailsResponse>();
                    foreach (var plan in distinctPlans)
                    {
                        var docPlanDetails = data.Where(d => d.DocId == doc.DocId && d.PlanGuid == plan.PlanGuid).FirstOrDefault();
                        planDoc.DocPlanDetails.Add(new PlanDocumentDetailsResponse()
                        {
                            TemplatePlanGuid = plan.PlanGuid,
                            IsShowDocument = docPlanDetails.IsShowDocument == null ? false : docPlanDetails.IsShowDocument.Value,
                            IsDocumentSelected = docPlanDetails.IsDocumentSelected == null ? false : docPlanDetails.IsDocumentSelected.Value,
                            IsDocumentReadOnly = docPlanDetails.isDocumentReadOnly == null ? false : docPlanDetails.isDocumentReadOnly.Value
                        });
                    }

                    // Add Image Details
                    planDoc.ImageDetails = new List<PlanImageResponse>();
                    foreach (var img in data.Where(d => d.DocId == doc.DocId).Select(d => new { d.ImageId }).Distinct())
                    {
                        var imgData = data.Where(d => d.ImageId == img.ImageId).FirstOrDefault();

                        var imgPlanData = new PlanImageResponse()
                        {
                            ImageId = imgData.ImageId,
                            Name = imgData.ImageName,
                            Description = imgData.ImageDescription,
                            WarningMessage = imgData.WarningMessage,
                            IsShowMandatory = imgData.IsShowMandatory == null ? false : doc.IsShowMandatory.Value,
                            IsMandatorySelected = imgData.IsMandatorySelected == null ? false : doc.IsMandatorySelected.Value,
                            //IsMandatoryReadOnly = imgData.IsMandatoryReadOnly == null ? false : doc.IsMandatoryReadOnly.Value
                        };

                        // Add Image Plan Details
                        imgPlanData.ImagePlanDetails = new List<PlanDocumentDetailsResponse>();
                        foreach (var plan in distinctPlans)
                        {
                            var imgPlanDetails = data.Where(d => d.ImageId == img.ImageId && d.PlanGuid == plan.PlanGuid).FirstOrDefault();
                            imgPlanData.ImagePlanDetails.Add(new PlanDocumentDetailsResponse()
                            {
                                TemplatePlanGuid = plan.PlanGuid,
                                IsShowDocument = imgPlanDetails.IsShowDocument == null ? false : imgPlanDetails.IsShowDocument.Value,
                                IsDocumentSelected = imgPlanDetails.IsDocumentSelected == null ? false : imgPlanDetails.IsDocumentSelected.Value,
                                IsDocumentReadOnly = imgPlanDetails.isDocumentReadOnly == null ? false : imgPlanDetails.isDocumentReadOnly.Value
                            });
                        }

                        planDoc.ImageDetails.Add(imgPlanData);
                    }

                    statePlan.PlanDocuments.Add(planDoc);
                }

                response.StatePlans.Add(statePlan);
            }

            return response;
        }
    }
}

