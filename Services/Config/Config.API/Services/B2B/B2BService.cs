using Config.API.Entities;
using Config.API.Models;
using Config.API.Models.Asset;
using Config.API.Models.B2B;
using Config.API.Repositories.B2B;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Config.API.Services.B2B
{
    public class B2BService : IB2BService
    {
        private readonly IB2BRepository b2bRepository;

        #region Constructor

        public B2BService(IB2BRepository repository)
        {
            this.b2bRepository = repository;
        }

        #endregion

        #region Get Company Details By Guid

        public Companies GetCompanyDetailsByGuid(string companyGuid)
        {
            var response = b2bRepository.GetCompanyByGuid(companyGuid);

            return response;
        }

        #endregion

        #region Get New Inspection Details

        public NewInspectionResponse GetNewInspectionDetails(Companies companyDetails, NewInspectionRequest model)
        {
            var response = new NewInspectionResponse();

            // Get ExemptPayment
            response.ExemptPayment = companyDetails != null
                                   ? companyDetails.ExcemptPayment
                                   : false;

            // Get Lenders Work With
            var lenderDetails = b2bRepository.GetBrokerLenderMappings(companyDetails.Id);
            var companies = b2bRepository.GetLenderCompanies()
                            .Where(c => c.LenderVisibility != (int)Enums.LenderVisibilities.Self)
                            .ToList();
            companies.ForEach(
                company =>
                {
                    var mappedLender = lenderDetails.FirstOrDefault(lender => lender.LenderCompanyId == company.Id);
                    response.Lenders.Add(
                        new LendersWorkWithResponse()
                        {
                            LenderName = company.CompanyName,
                            LenderGUID = company.CompanyGuid,
                            IsPayer = company.IsPayer,
                            IsMapped = (mappedLender != null)
                                     ? true
                                     : false
                        });
                });

            // Get Assets Work With
            var templateMappings = b2bRepository.GetBrokerTemplateMappings(companyDetails.Id);
            var templateSets = b2bRepository.GetTemplateSets();
            if (templateSets != null &&
                templateSets.Count() > 0)
            {
                templateSets.ForEach(
                    templateSet =>
                    {
                        var mappedTemplate = templateMappings.FirstOrDefault(template => template.TemplateId == templateSet.Id);
                        response.Assets.Add(
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

            // Get States
            response.States = b2bRepository.GetStates();

            // Get Template Details
            var assetDocsList = new AssetDocListRequest()
            {
                TemplateGuid = model.TemplateGuid,
                planGuid = model.planGuid,
                LenderGuid = model.LenderGuid,
                StateId = model.StateId,
                IsIncludeNoLenderPreference = model.IsIncludeNoLenderPreference
            };
            response.TemplateDetails = GetAssetDocDetails(assetDocsList, model.UserGuid, model.CompanyGuid);

            return response;
        }

        #region Get Asset Document Details

        private TemplateDetails GetAssetDocDetails(AssetDocListRequest model, string userGuid, string companyGuid)
        {
            var response = new TemplateDetails();

            // Add Document Details
            // Add Image Details
            var result = b2bRepository.GetAssetDocDetails(model, userGuid, companyGuid);
            var docIds = result.Select(d => d.DocId).Distinct().ToList();
            docIds.ForEach(
                docId =>
                {
                    var assetDocListResponses = new AssetDocListResponse();

                    var docDetail = result.FirstOrDefault(d => d.DocId == docId);
                    assetDocListResponses.DocumentId = docId;
                    assetDocListResponses.DocumentName = docDetail.DocName;
                    assetDocListResponses.DocDescription = docDetail.DocDescription;
                    assetDocListResponses.AdditionalPrice = docDetail.AdditionalPrice;
                    assetDocListResponses.WarningMessage = docDetail.DocWarningMessage;
                    assetDocListResponses.Position = docDetail.DocPosition;
                    assetDocListResponses.DisplayPosition = response.DocumentDetails.Count() + 1;
                    assetDocListResponses.IsAdditionalDataRequired = docDetail.IsAdditionalDataRequired;
                    assetDocListResponses.IsAdditionalDataMandatory = docDetail.IsAdditionalDataMandatory;
                    assetDocListResponses.ImageDetails = (from i in result
                                                          where i.DocId == docId
                                                          select new Models.Asset.AssetImageListResponse()
                                                          {
                                                              ImageType = i.ImageType,
                                                              ImageName = i.ImageName,
                                                              DocGroup = i.DocGroup,
                                                              Position = i.ImagePosition,
                                                              Description = i.ImageDescription,
                                                              IsMandatory = i.IsMandatory,
                                                              IsDefaultSelected = i.IsDefaultSelected,
                                                              IsCheckboxDisabled = i.IsCheckboxDisabled,
                                                              WarningMessage = i.ImageWarningMessage
                                                          }).ToList();

                    response.DocumentDetails.Add(assetDocListResponses);
                });
            response.BasePrice = result != null &&
                                 result.Count() > 0
                               ? result.FirstOrDefault().BasePrice
                               : 0;

            if (model.IsIncludeNoLenderPreference)
            {
                // Get TemplateId
                var templateDetails = b2bRepository.GetTemplateSetDetailByGuid(model.TemplateGuid);

                // Get TemplateDoc NoLender Preference
                var noLenderPreference = b2bRepository.GetTemplateDocNoLenderPreference(templateDetails.Id, userGuid, companyGuid);

                // If Preference exist for loggedin user select mapped documents
                // Else select all documents
                if (noLenderPreference != null)
                {
                    var templateDocNoLenders = JsonSerializer.Deserialize<List<NoLenderPreference>>(noLenderPreference.Preference);

                    // If NoLender Preference is already saved select mapped documents
                    // Else select all documents
                    if (noLenderPreference.IsPreferenceSaved == true)
                    {
                        response.DocumentDetails.ForEach(
                            document =>
                            {
                                // Get NoLender Preference Data By DocumentId
                                var templateDocNoLender = templateDocNoLenders.FirstOrDefault(nlp => nlp.DocumentId == document.DocumentId);

                                document.DocumentRequired = templateDocNoLender != null
                                                          ? "yes"
                                                          : "no";

                                document.ImageDetails.ForEach(
                                    image =>
                                    {
                                        image.IsDefaultSelected = templateDocNoLender != null &&
                                                                  templateDocNoLender.ImageTypes.Count(it => it == image.ImageType) > 0
                                                                ? true
                                                                : false;
                                    });
                            });

                        response.IsPreferenceSaved = noLenderPreference.IsPreferenceSaved;
                    }
                    else
                    {
                        SelectNoLenderDocuments(response);
                    }
                }
                else
                {
                    SelectNoLenderDocuments(response);
                }
            }

            return response;
        }

        #region Select NoLender Documents

        private void SelectNoLenderDocuments(TemplateDetails templateDetails)
        {
            templateDetails.DocumentDetails.ForEach(
                       document =>
                       {
                           document.DocumentRequired = "yes";

                           document.ImageDetails.ForEach(
                               image =>
                               {
                                   image.IsDefaultSelected = true;
                               });
                       });

            templateDetails.IsPreferenceSaved = true;
        }

        #endregion

        #endregion

        #endregion
    }
}
