using Config.API.Models.Lender;
using System.Collections.Generic;

namespace Config.API.Repositories
{
    public interface IAssetRepository
    {
        public List<Entities.TemplateSets> GetAllAssets();

        public LenderAssetConfigResponse GetLenderAssetConfigs(LenderAssetConfigRequest model, long companyId);

        public void SaveLenderAssetConfigs(LenderAssetConfigRequest model, long companyId);
    }
}
