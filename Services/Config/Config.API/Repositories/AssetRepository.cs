using Config.API.Models;
using Config.API.Models.Lender;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Config.API.Repositories
{
    public class AssetRepository : IAssetRepository
    {
        private readonly Entities.ConfigContext dbContext;

        #region Constructor
        public AssetRepository(Entities.ConfigContext context)
        {
            dbContext = context;
        }
        #endregion

        #region Get All Assets

        public List<Entities.TemplateSets> GetAllAssets()
        {
            // Get All Assets
            var responses = dbContext.TemplateSets.ToList();

            return responses;
        }

        #endregion

        #region Get Lender Asset Configs

        public LenderAssetConfigResponse GetLenderAssetConfigs(LenderAssetConfigRequest model, long companyId)
        {
            var docConfigs = new List<DocConfig>();
            var result = new LenderAssetConfigResponse();

            // Get TemplateSetId
            var templateSetId = dbContext.TemplateSets
                .Where(ts => ts.TemplateSetGuid == model.TemplateSetGuid)
                .Select(ts => ts.Id)
                .FirstOrDefault();

            // Get Lender Company Mapped to Document
            var lenderCompany = dbContext.LenderTemplateSetDocMappings
                .Where(document => document.LenderCompanyId == companyId)
                .FirstOrDefault();

            // Checking whether Lender Company exists
            if (lenderCompany != null)
            {
                // Get Documents
                var documents = (from ltd in dbContext.LenderTemplateSetDocMappings
                                 join td in dbContext.TemplateDocuments
                                 on ltd.TemplateDocumentId equals td.Id
                                 where ltd.LenderCompanyId == companyId && ltd.TemplateSetId == templateSetId
                                 select new
                                 {
                                     TemplateDocumentId = ltd.TemplateDocumentId,
                                     DocumentName = td.Name,
                                     UploadOptions = ltd.UploadOptions
                                 }).ToList();
                // Get Document Images
                var docImages = (from image in dbContext.LenderTemplateDocImageMappings
                                 where image.LenderCompanyId == companyId && image.TemplateSetId == templateSetId
                                 select new
                                 {
                                     TemplateDocumentId = image.TemplateDocumentId,
                                     TemplateImageId = image.TemplateImageId,
                                     IsMandatory = image.IsMandatory,
                                     IsSkippable = image.IsSkippable,
                                     NotRequired = image.NotRequired
                                 }).ToList();

                foreach (var document in documents)
                {
                    var uploadOptions = JsonSerializer.Deserialize<Entities.UploadOption>(document.UploadOptions);

                    // Adding Direct Options, State Options and Document Images
                    docConfigs.Add(new DocConfig()
                    {
                        TemplateDocumentId = document.TemplateDocumentId,
                        DocumentName = document.DocumentName,
                        DirectOptions = (from option in uploadOptions.DirectOptions
                                         select new Models.DirectOption()
                                         {
                                             OptionName = option.OptionName,
                                             ActionId = option.ActionId
                                         }).ToList(),
                        StateOptions = (from option in uploadOptions.StateOptions
                                        select new Models.StateOption()
                                        {
                                            StateCode = option.StateCode,
                                            IsMandatory = option.IsMandatory,
                                            IsSkippable = option.IsSkippable,
                                            NotRequired = option.NotRequired

                                        }).ToList(),
                        DocImages = (from image in docImages
                                     where image.TemplateDocumentId == document.TemplateDocumentId
                                     select new DocImageConfig()
                                     {
                                         TemplateImageId = image.TemplateImageId,
                                         IsMandatory = image.IsMandatory,
                                         IsSkippable = image.IsSkippable,
                                         NotRequired = image.NotRequired
                                     }).ToList()
                    });
                }
            }
            else
            {
                // Get Documents
                var documents = (from tdm in dbContext.TemplateSetDocMappings
                                 join td in dbContext.TemplateDocuments on tdm.TemplateDocumentId equals td.Id
                                 where tdm.TemplateSetId == templateSetId
                                 select new
                                 {
                                     TemplateDocumentId = td.Id,
                                     DocumentName = td.Name,
                                     TemplateDocCategoryId = td.TemplateDocCategoryId
                                 }).ToList();

                foreach (var document in documents)
                {
                    // Get Direct Options
                    var directOptions = (from tdo in dbContext.TemplateDocOptions
                                         where tdo.TemplateDocumentId == document.TemplateDocumentId &&
                                         tdo.OptionType == (int)Enums.OptionTypes.DirectOption
                                         select new Models.DirectOption()
                                         {
                                             OptionName = tdo.OptionName,
                                             ActionId = tdo.ActionId
                                         }).ToList();

                    // Get State Options
                    var stateOptions = (from s in dbContext.States
                                        select new Models.StateOption()
                                        {
                                            StateCode = s.StateCode,
                                            IsMandatory = false,
                                            IsSkippable = false,
                                            NotRequired = false
                                        }).ToList();

                    // Get Document Images
                    var docImages = (from ti in dbContext.TemplateImages
                                     join tim in dbContext.TemplateDocImageMappings on ti.Id equals tim.TemplateImageId
                                     where tim.TemplateDocumentId == document.TemplateDocumentId &&
                                     ti.TemplateDocCategoryId == document.TemplateDocCategoryId
                                     select new DocImageConfig()
                                     {
                                         TemplateImageId = tim.TemplateImageId,
                                         IsMandatory = tim.IsMandatory,
                                         IsSkippable = false,
                                         NotRequired = false
                                     }).ToList();

                    // Adding Direct Options, State Options and Document Images
                    docConfigs.Add(new DocConfig()
                    {
                        TemplateDocumentId = document.TemplateDocumentId,
                        DocumentName = document.DocumentName,
                        DirectOptions = directOptions,
                        StateOptions = stateOptions,
                        DocImages = docImages
                    });
                }
            }

            result.Documents = docConfigs;

            return result;
        }

        #endregion

        #region Save Lender Asset Configs

        public void SaveLenderAssetConfigs(LenderAssetConfigRequest model, long companyId)
        {
            // Get TemplateSetId
            var templateSetId = dbContext.TemplateSets.Where(ts => ts.TemplateSetGuid == model.TemplateSetGuid).Select(ts => ts.Id).FirstOrDefault();

            foreach (var document in model.Documents)
            {
                var directOptions = new List<Entities.DirectOption>();
                var stateOptions = new List<Entities.StateOption>();
                var uploadOption = new Entities.UploadOption();
                var lenderTemplateSetDocMappings = new Entities.LenderTemplateSetDocMappings();

                lenderTemplateSetDocMappings.LenderCompanyId = companyId;
                lenderTemplateSetDocMappings.TemplateSetId = templateSetId;
                lenderTemplateSetDocMappings.TemplateDocumentId = document.TemplateDocumentId;

                // Checking whether DirectOptions exists
                if (document.DirectOptions != null &&
                    document.DirectOptions.Count() > 0)
                {
                    foreach (var option in document.DirectOptions)
                    {
                        directOptions.Add(
                            new Entities.DirectOption()
                            {
                                OptionName = option.OptionName,
                                ActionId = option.ActionId
                            });
                    }

                    uploadOption.DirectOptions = directOptions;
                }

                // Checking whether StateOptions exists
                if (document.StateOptions != null &&
                    document.StateOptions.Count() > 0)
                {
                    foreach (var option in document.StateOptions)
                    {
                        stateOptions.Add(
                            new Entities.StateOption()
                            {
                                StateCode = option.StateCode,
                                IsMandatory = option.IsMandatory,
                                IsSkippable = option.IsSkippable,
                                NotRequired = option.NotRequired
                            });
                    }

                    uploadOption.StateOptions = stateOptions;
                }

                // UploadOptions
                lenderTemplateSetDocMappings.UploadOptions = JsonSerializer.Serialize(uploadOption);

                dbContext.LenderTemplateSetDocMappings.Add(lenderTemplateSetDocMappings);
                dbContext.SaveChanges();

                foreach (var image in document.DocImages)
                {
                    var lenderTemplateDocImageMappings = new Entities.LenderTemplateDocImageMappings();

                    lenderTemplateDocImageMappings.LenderCompanyId = companyId;
                    lenderTemplateDocImageMappings.TemplateSetId = templateSetId;
                    lenderTemplateDocImageMappings.TemplateDocumentId = document.TemplateDocumentId;
                    lenderTemplateDocImageMappings.TemplateImageId = image.TemplateImageId;
                    lenderTemplateDocImageMappings.IsMandatory = image.IsMandatory;
                    lenderTemplateDocImageMappings.IsSkippable = image.IsSkippable;
                    lenderTemplateDocImageMappings.NotRequired = image.NotRequired;

                    dbContext.LenderTemplateDocImageMappings.Add(lenderTemplateDocImageMappings);
                    dbContext.SaveChanges();
                }
            }
        }

        #endregion
    }
}
