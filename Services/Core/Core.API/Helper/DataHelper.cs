using Common.Extensions;
using Core.API.Models.Data;
using System.Collections.Generic;
using System.Net.Http;

namespace Core.API.Helper
{
    public class DataHelper
    {
        #region Get LendersWorkWith Details

        public static List<LenderWorkWithResponse> GetLendersWorkWithDetails(string token)
        {
            List<LenderWorkWithResponse> lenderWorkWithResponse = null;

            using (var client = new HttpClient())
            {
                var requestUri = "api/v1/Data/getlendersworkwith";
                var configApiUrl = Startup.AppConfiguration.GetSection("BaseURL").GetSection("ConfigApiURL").Value;
                var response = ExtensionMethods<List<LenderWorkWithResponse>>
                               .GetDeserializedData(client, configApiUrl, requestUri, token)
                               .Result;
                if (response != null)
                {
                    lenderWorkWithResponse = response;
                }
            }

            return lenderWorkWithResponse;
        }

        #endregion

        #region Get AssetsWorkWith Details

        public static List<AssetWorkWithResponse> GetAssetsWorkWithDetails(string token)
        {
            List<AssetWorkWithResponse> assetWorkWithResponse = null;

            using (var client = new HttpClient())
            {
                var requestUri = "api/v1/Data/getassetsworkwith";
                var configApiUrl = Startup.AppConfiguration.GetSection("BaseURL").GetSection("ConfigApiURL").Value;
                var response = ExtensionMethods<List<AssetWorkWithResponse>>
                               .GetDeserializedData(client, configApiUrl, requestUri, token)
                               .Result;
                if (response != null)
                {
                    assetWorkWithResponse = response;
                }
            }

            return assetWorkWithResponse;
        }

        #endregion

        #region Get Company Details

        public static CompanyDetailsResponse GetCompanyDetails(string token)
        {
            CompanyDetailsResponse companyDetailsResponse = null;

            using (var client = new HttpClient())
            {
                var requestUri = "api/v1/Data/getcompanydetails";
                var configApiUrl = Startup.AppConfiguration.GetSection("BaseURL").GetSection("ConfigApiURL").Value;
                var response = ExtensionMethods<CompanyDetailsResponse>
                               .GetDeserializedData(client, configApiUrl, requestUri, token)
                               .Result;
                if (response != null)
                {
                    companyDetailsResponse = response;
                }
            }

            return companyDetailsResponse;
        }

        #endregion
    }
}
