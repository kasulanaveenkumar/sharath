using Config.API.Models;
using Config.API.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace Config.API.Services
{
    public class AssetService : IAssetService
    {
        private readonly IAssetRepository assetRepository;

        #region Constructor

        public AssetService(IAssetRepository repository)
        {
            this.assetRepository = repository;
        }

        #endregion

        #region Get All Assets

        public List<AssetsWorkWithResponse> GetAllAssets()
        {
            // Get All Assets
            var assets = assetRepository.GetAllAssets();

            var responses = (from t in assets
                             select new AssetsWorkWithResponse()
                             {
                                 TemplateName = t.Name,
                                 TemplateSetGUID = t.TemplateSetGuid
                             }).ToList();

            return responses;
        }

        #endregion
    }
}
