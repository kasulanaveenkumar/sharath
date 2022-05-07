using Common.Extensions;
using Common.Identity.Models.DriverLicenseOCR;
using Common.Identity.Models.DVS;
using Common.Models.ApiResponse;
using System.Collections.Generic;
using System.Net.Http;

namespace Common.Identity.Helper
{
    public class KreanoHelper
    {
        #region Check DVS

        public ApiResponseWithStatusCode CheckDVS(string apiUrl,
                                                  string requestUri,
                                                  Dictionary<string, object> httpHeaderValues,
                                                  KreanoDVSCheckModel model)
        {
            ApiResponseWithStatusCode apiPostResponseData = null;

            using (var client = new HttpClient())
            {
                apiPostResponseData = ExtensionMethods<KreanoDVSCheckModel>
                                      .PostJsonDatas(client, apiUrl, requestUri, httpHeaderValues, model)
                                      .Result;
            }

            return apiPostResponseData;
        }

        #endregion

        #region Driver Licence OCR

        public ApiResponseWithStatusCode DriverLicenceOCR(string apiUrl,
                                          string requestUri,
                                          Dictionary<string, object> httpHeaderValues,
                                          DriverLicenceOCRRequest driverLicenceOCRRequest)
        {
            ApiResponseWithStatusCode apiPostResponseData = null;

            using (var client = new HttpClient())
            {
                apiPostResponseData = ExtensionMethods<DriverLicenceOCRRequest>
                                      .PostJsonDatas(client, apiUrl, requestUri, httpHeaderValues, null, driverLicenceOCRRequest.DriverLicenceOCRImages)
                                      .Result;
            }

            return apiPostResponseData;
        }

        #endregion
    }
}
