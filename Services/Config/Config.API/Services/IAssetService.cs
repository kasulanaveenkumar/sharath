using Config.API.Models;
using System.Collections.Generic;

namespace Config.API.Services
{
    public interface IAssetService
    {
        public List<AssetsWorkWithResponse> GetAllAssets();
    }
}
