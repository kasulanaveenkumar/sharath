using Config.API.Entities;
using Config.API.Models;
using Config.API.Models.Asset;
using Config.API.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Config.API.Services
{
    public class InspectionService : IInspectionService
    {
        private readonly IInspectionRepository inspectionRepository;

        #region Constructor

        public InspectionService(IInspectionRepository repository)
        {
            this.inspectionRepository = repository;
        }

        #endregion

        #region Get Lender Detail By Guid

        public Companies GetLenderDetailByGuid(string lenderGuid)
        {
            // Get Lender Detail By Guid
            var response = inspectionRepository.GetLenderDetailByGuid(lenderGuid);

            return response;
        }

        #endregion

        #region Get TemplateSet Detail By Guid

        public TemplateSets GetTemplateSetDetailByGuid(string templateSetGuid)
        {
            // Get TemplateSet Detail By Guid
            var response = inspectionRepository.GetTemplateSetDetailByGuid(templateSetGuid);

            return response;
        }

        #endregion

        #region Get Plan Detail By Guid

        public TemplateSetPlans GetPlanDetailByGuid(string planGuid)
        {
            // Get TemplateSet Detail By Guid
            var response = inspectionRepository.GetPlanDetailByGuid(planGuid);

            return response;
        }

        #endregion

        #region Get State Detail By Id

        public States GetStateDetailById(long stateId)
        {
            // Get State Detail By Guid
            var response = inspectionRepository.GetStateDetailById(stateId);

            return response;
        }

        #endregion

        #region Get Template Plans

        public List<InspectionPlansResponse> GetTemplatePlans(long lenderCompanyId, long templateSetId, long stateId)
        {
            // Get Template Plans
            var responses = inspectionRepository.GetTemplatePlans(lenderCompanyId, templateSetId, stateId);

            return responses;
        }

        #endregion

        #region Get Asset Document Details

        public TemplateDetails GetAssetDocDetails(AssetDocListRequest model, string userGuid, string companyGuid)
        {
            var response = new TemplateDetails();

            // Add Document Details
            // Add Image Details
            var result = inspectionRepository.GetAssetDocDetails(model, userGuid, companyGuid);
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
                var templateDetails = inspectionRepository.GetTemplateSetDetailByGuid(model.TemplateGuid);

                // Get TemplateDoc NoLender Preference
                var noLenderPreference = inspectionRepository.GetTemplateDocNoLenderPreference(templateDetails.Id, userGuid, companyGuid);

                // If Preference exist for loggedin user select mapped documents
                // Else select all documents
                if (noLenderPreference != null)
                {
                    var templateDocNoLenders = JsonSerializer.Deserialize<List<Models.NoLenderPreference>>(noLenderPreference.Preference);

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

        #region Save NoLender Preferences

        public void SaveNoLenderPreference(TemplateSets templateSetDetail, SaveNoLenderPreferenceRequest model, 
                                           string userGuid, string companyGuid)
        {
            // Get TemplateDocNoLenderPreference Details
            var noLenderPreference = inspectionRepository.GetTemplateDocNoLenderPreference(templateSetDetail.Id, userGuid, companyGuid);
            if (noLenderPreference == null)
            {
                // Save TemplateDocNoLenderPreference
                var templateDocNoLenderPreferences = new TemplateDocNoLenderPreferences();
                templateDocNoLenderPreferences.TemplateSetId = templateSetDetail.Id;
                templateDocNoLenderPreferences.TemplateSetPlanId = 0;
                templateDocNoLenderPreferences.UserGuid = userGuid;
                templateDocNoLenderPreferences.CompanyGuid = companyGuid;
                templateDocNoLenderPreferences.Preference = JsonSerializer.Serialize(model.NoLenderPreferences);
                templateDocNoLenderPreferences.IsPreferenceSaved = model.IsPreferenceSaved;
                inspectionRepository.SaveTemplateDocNoLenderPreference(templateDocNoLenderPreferences);
            }
            else
            {
                // Update TemplateDocNoLenderPreference
                noLenderPreference.TemplateSetId = templateSetDetail.Id;
                noLenderPreference.TemplateSetPlanId = 0;
                noLenderPreference.UserGuid = userGuid;
                noLenderPreference.CompanyGuid = companyGuid;
                noLenderPreference.Preference = JsonSerializer.Serialize(model.NoLenderPreferences);
                noLenderPreference.IsPreferenceSaved = model.IsPreferenceSaved;
                inspectionRepository.UpdateTemplateDocNoLenderPreference(noLenderPreference);
            }
        }

        #endregion
    }
}
