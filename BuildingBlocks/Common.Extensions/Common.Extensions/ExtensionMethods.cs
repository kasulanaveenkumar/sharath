using System;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Dynamic;
using CsvHelper;
using System.Globalization;
using TimeZoneConverter;
using Common.Models.ApiResponse;

namespace Common.Extensions
{
    public static class ExtensionMethods
    {
        #region Get New GUID
        public static string GetNewGuid()
        {
            return Guid.NewGuid().ToString().ToUpper();
        }

        #endregion

        #region Base64 to Image

        public static Image Base64ToImage(this string base64)
        {
            var result = base64.Split(',');

            if (result.Length > 1)
            {
                base64 = result[1];
            }

            byte[] bytes = Convert.FromBase64String(base64);

            Image image;

            using (MemoryStream ms = new MemoryStream(bytes))
            {
                image = Image.FromStream(ms);
            }
            return image;
        }

        #endregion

        #region Image to Base64

        public static string ImageToBase64(this Image image)
        {
            using (MemoryStream m = new MemoryStream())
            {
                image.Save(m, image.RawFormat);
                byte[] imageBytes = m.ToArray();

                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }

        #endregion

        #region Get Base64 Extension

        public static string GetBase64Extension(this string base64String)
        {
            var extension = string.Empty;

            var strings = base64String.Split(",");

            switch (strings[0])
            {
                //check image's extension
                case "data:image/jpeg;base64":
                    extension = ".jpeg";
                    break;
                case "data:image/png;base64":
                    extension = ".png";
                    break;
                case "data:image/jpg;base64":
                    extension = ".jpg";
                    break;
                case "data:image/pdf;base64":
                    extension = ".pdf";
                    break;
                default:
                    break;
            }

            return extension;
        }

        #endregion

        #region Sort Data by Column and Direction

        public static IQueryable Sort(this IQueryable collection, string sortBy, string sortDirection)
        {
            return collection.OrderBy(sortBy + (sortDirection == "asc" ? " ascending" : " descending"));
        }

        #endregion

        #region Get File Size from Bas64

        public static Int32 GetFileSizeFromBase64(this string base64String)
        {
            byte[] imageBytes = Convert.FromBase64String(base64String.Split(",")[1]);

            return imageBytes.Length;
        }

        #endregion

        #region Get CreditCard Type

        public static string GetCreditCardType(this string CreditCardNumber)
        {
            string result = string.Empty;

            Regex regAmex = new Regex("^3[47][0-9]{13}$");
            Regex regBCGlobal = new Regex("^(6541|6556)[0-9]{12}$");
            Regex regCarteBlanche = new Regex("^389[0-9]{11}$");
            Regex regDinersClub = new Regex("^3(?:0[0-5]|[68][0-9])[0-9]{4,}$");
            Regex regDiscover = new Regex("^65[4-9][0-9]{13}|64[4-9][0-9]{13}|6011[0-9]{12}|(622(?:12[6-9]|1[3-9][0-9]|[2-8][0-9][0-9]|9[01][0-9]|92[0-5])[0-9]{10})$");
            Regex regInstaPayment = new Regex("^63[7-9][0-9]{13}$");
            Regex regJCB = new Regex(@"^(?:2131|1800|35\d{3})\d{11}$");
            Regex regKoreanLocal = new Regex("^9[0-9]{15}$");
            Regex regLaser = new Regex("^(6304|6706|6709|6771)[0-9]{12,15}$");
            Regex regMaestro = new Regex("^(5018|5020|5038|6304|6759|6761|6763)[0-9]{8,15}$");
            Regex regMastercard = new Regex("^(5[1-5][0-9]{14}|2(22[1-9][0-9]{12}|2[3-9][0-9]{13}|[3-6][0-9]{14}|7[0-1][0-9]{13}|720[0-9]{12}))$");
            Regex regSolo = new Regex("^(6334|6767)[0-9]{12}|(6334|6767)[0-9]{14}|(6334|6767)[0-9]{15}$");
            Regex regSwitch = new Regex("^(4903|4905|4911|4936|6333|6759)[0-9]{12}|(4903|4905|4911|4936|6333|6759)[0-9]{14}|(4903|4905|4911|4936|6333|6759)[0-9]{15}|564182[0-9]{10}|564182[0-9]{12}|564182[0-9]{13}|633110[0-9]{10}|633110[0-9]{12}|633110[0-9]{13}$");
            Regex regUnionPay = new Regex("^(62[0-9]{14,17})$");
            Regex regVisa = new Regex("^4[0-9]{12}(?:[0-9]{3})?$");
            Regex regVisaMasterCard = new Regex("^(?:4[0-9]{12}(?:[0-9]{3})?|5[1-5][0-9]{14})$");

            // AmericanExpress
            if (regAmex.IsMatch(CreditCardNumber))
            {
                result = Models.Enums.CreditCardType.AmericanExpress.ToString();
            }

            // BCGlobal
            else if (regBCGlobal.IsMatch(CreditCardNumber))
            {
                result = Models.Enums.CreditCardType.BCGlobal.ToString();
            }

            // CarteBlanche
            else if (regCarteBlanche.IsMatch(CreditCardNumber))
            {
                result = Models.Enums.CreditCardType.CarteBlanche.ToString();
            }

            // DinersClub
            else if (regDinersClub.IsMatch(CreditCardNumber))
            {
                result = Models.Enums.CreditCardType.DinersClub.ToString();
            }

            // Discover
            else if (regDiscover.IsMatch(CreditCardNumber))
            {
                result = Models.Enums.CreditCardType.Discover.ToString();
            }

            // InstaPayment
            else if (regInstaPayment.IsMatch(CreditCardNumber))
            {
                result = Models.Enums.CreditCardType.InstaPayment.ToString();
            }

            // JCB
            else if (regJCB.IsMatch(CreditCardNumber))
            {
                result = Models.Enums.CreditCardType.JCB.ToString();
            }

            // KoreanLocal
            else if (regKoreanLocal.IsMatch(CreditCardNumber))
            {
                result = Models.Enums.CreditCardType.KoreanLocal.ToString();
            }

            // Laser
            else if (regLaser.IsMatch(CreditCardNumber))
            {
                result = Models.Enums.CreditCardType.Laser.ToString();
            }

            // Maestro
            else if (regMaestro.IsMatch(CreditCardNumber))
            {
                result = Models.Enums.CreditCardType.Maestro.ToString();
            }

            // Mastercard
            else if (regMastercard.IsMatch(CreditCardNumber))
            {
                result = Models.Enums.CreditCardType.Mastercard.ToString();
            }

            // Solo
            else if (regSolo.IsMatch(CreditCardNumber))
            {
                result = Models.Enums.CreditCardType.Solo.ToString();
            }

            // Switch
            else if (regSwitch.IsMatch(CreditCardNumber))
            {
                result = Models.Enums.CreditCardType.Switch.ToString();
            }

            // UnionPay
            else if (regUnionPay.IsMatch(CreditCardNumber))
            {
                result = Models.Enums.CreditCardType.UnionPay.ToString();
            }

            // Visa
            else if (regVisa.IsMatch(CreditCardNumber))
            {
                result = Models.Enums.CreditCardType.Visa.ToString();
            }

            // VisaMasterCard
            else if (regVisaMasterCard.IsMatch(CreditCardNumber))
            {
                result = Models.Enums.CreditCardType.VisaMasterCard.ToString();
            }
            // Invalid
            else
            {
                result = Models.Enums.CreditCardType.Invalid.ToString();
            }

            return result;
        }

        #endregion

        #region Get Enum Description Attribute Value

        public static string GetEnumDescriptionAttributeValue(this Enum value)
        {
            var attr = value.GetType().GetField(value.ToString())
                       .GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false)
                       .FirstOrDefault() as System.ComponentModel.DescriptionAttribute;

            return attr.Description;
        }

        #endregion

        #region Get Enum Values and Descriptions

        public static List<KeyValuePair<string, int>> GetEnumValuesAndDescriptions<T>()
        {
            Type enumType = typeof(T);

            if (enumType.BaseType != typeof(Enum))
                throw new ArgumentException("T is not System.Enum");

            List<KeyValuePair<string, int>> enumValList = new List<KeyValuePair<string, int>>();

            foreach (var e in Enum.GetValues(typeof(T)))
            {
                var fi = e.GetType().GetField(e.ToString());
                var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

                enumValList.Add(new KeyValuePair<string, int>((attributes.Length > 0) ? attributes[0].Description : e.ToString(), (int)e));
            }

            return enumValList;
        }

        #endregion

        #region Convert json string Values to Dictionary

        public static Dictionary<string, object> GetJsonStringValues(this object jsonData)
        {
            var result = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonData.ToString());

            return result;
        }

        #endregion

        #region Convert Json String to Csv

        public static string JsonToCsv(this string jsonContent)
        {
            var expandos = JsonSerializer.Deserialize<ExpandoObject[]>(jsonContent);

            using (var writer = new StringWriter())
            {
                using (var csv = new CsvWriter(writer, CultureInfo.CurrentCulture))
                {
                    csv.WriteRecords(expandos as IEnumerable<dynamic>);
                }

                return writer.ToString();
            }
        }

        #endregion

        #region Calculate SLA Duration

        public static int CalcuteSLADuration(DateTime uploadedTime)
        {
            TimeSpan businessStartTime = new TimeSpan(9, 0, 0);
            TimeSpan businessEndTime = new TimeSpan(19, 0, 0);

            var tz = TZConvert.GetTimeZoneInfo("AUS Eastern Standard Time");
            DateTime currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
            DateTime revisedUploadedTime = TimeZoneInfo.ConvertTimeFromUtc(uploadedTime, tz);

            // Calculation SLA Duration
            int duration = 0;
            while (revisedUploadedTime.Date <= currentTime.Date)
            {
                if (revisedUploadedTime.DayOfWeek != DayOfWeek.Saturday &&
                    revisedUploadedTime.DayOfWeek != DayOfWeek.Sunday)
                {
                    if (revisedUploadedTime.TimeOfDay > businessEndTime)
                    {
                        revisedUploadedTime = revisedUploadedTime.AddDays(1).Date + businessStartTime;
                        continue;
                    }

                    if (revisedUploadedTime.TimeOfDay < businessStartTime)
                    {
                        revisedUploadedTime = revisedUploadedTime.Date + businessStartTime;
                    }

                    int temp = 0;
                    if (revisedUploadedTime.Date < currentTime.Date ||
                        currentTime.TimeOfDay > businessEndTime)
                    {
                        temp = (int)businessEndTime.Subtract(revisedUploadedTime.TimeOfDay).TotalMinutes;
                    }
                    else
                    {
                        temp = (int)currentTime.TimeOfDay.Subtract(revisedUploadedTime.TimeOfDay).TotalMinutes;
                    }

                    if (temp > 0)
                    {
                        duration = duration + temp;
                    }
                }

                revisedUploadedTime = revisedUploadedTime.AddDays(1).Date + businessStartTime;
            }

            return duration;
        }

        #endregion

        #region Get Random OTP

        public static string GetRandomOTP()
        {
            Random generator = new Random();
            var otp = generator.Next(0, 1000000).ToString("D6");
            return otp;
        }

        #endregion

        #region Validate JSON

        public static bool IsValidJson<T>(this string strInput)
        {
            if (string.IsNullOrWhiteSpace(strInput)) return false;

            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(strInput);
                    return true;
                }
                catch // not valid
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region Get UTCTime For Image MetaData

        public static string GetUTCTimeForImageMetaData()
        {
            return DateTime.UtcNow.ToString("dd/MM/yyyyTHH:mm:ss").Replace("-", "/");
        }

        #endregion
    }

    public static class ExtensionMethods<T> where T : class
    {
        #region Get Deserialized Data

        public static async Task<T> GetDeserializedData(HttpClient client, string uri, string requestUri, string token)
        {
            T objData = null;

            client.BaseAddress = new Uri(uri);

            // Add an Accept header for JSON format
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Checking whether token exists
            if (!string.IsNullOrEmpty(token))
            {
                // Add an Accept header for Token
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Token", token);
            }

            // Get JsonResponse
            var jsonResponse = client.GetAsync(requestUri, new CancellationToken()).ConfigureAwait(false).GetAwaiter().GetResult();
            if (jsonResponse.IsSuccessStatusCode)
            {
                // Get json data
                var jsonData = jsonResponse.Content.ReadAsStringAsync().Result;

                // Get serializer options
                var serializerOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                // Deserialize json data to List
                objData = JsonSerializer.Deserialize<T>(jsonData, serializerOptions);
            }
            else
            {

            }

            return objData;
        }

        #endregion

        #region Get Deserialized Datas

        public static async Task<List<T>> GetDeserializedDatas(HttpClient client, string uri, string requestUri, string token)
        {
            var list = new List<T>();

            client.BaseAddress = new Uri(uri);

            // Add an Accept header for JSON format
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Checking whether token exists
            if (!string.IsNullOrEmpty(token))
            {
                // Add an Accept header for Token
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Token", token);
            }

            // Get JsonResponse
            var jsonResponse = client.GetAsync(requestUri, new CancellationToken()).ConfigureAwait(false).GetAwaiter().GetResult();
            if (jsonResponse.IsSuccessStatusCode)
            {
                // Get json data
                var jsonData = jsonResponse.Content.ReadAsStringAsync().Result;

                // Get serializer options
                var serializerOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                // Deserialize json data to List
                list = JsonSerializer.Deserialize<List<T>>(jsonData, serializerOptions);
            }
            else
            {

            }

            return list;
        }

        public static async Task<List<T>> GetSerializedDatas(HttpClient client, string uri, string requestUri, string token, object objects)
        {
            var list = new List<T>();

            client.BaseAddress = new Uri(uri);

            // Add an Accept header for JSON format
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Checking whether token exists
            if (!string.IsNullOrEmpty(token))
            {
                // Add an Accept header for Token
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Token", token);
            }

            var json = JsonSerializer.Serialize(objects);

            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var jsonResponse = await client.PostAsync(requestUri, data);

            // Checking whether JsonResponse is success
            if (jsonResponse.IsSuccessStatusCode)
            {
                // Get json data
                var jsonData = jsonResponse.Content.ReadAsStringAsync().Result;

                // Get serializer options
                var serializerOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                // Deserialize json data to List
                list = JsonSerializer.Deserialize<List<T>>(jsonData, serializerOptions);
            }
            else
            {

            }

            return list;
        }

        #endregion

        #region Post Json Datas

        public static async Task<string> PostJsonDatas(HttpClient client,
                                                       string uri,
                                                       string requestUri,
                                                       string token,
                                                       T objects)
        {
            try
            {
                client.BaseAddress = new Uri(uri);

                // Add an Accept header for JSON format
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Checking whether token exists
                if (!string.IsNullOrEmpty(token))
                {
                    // Add an Accept header for Token
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Token", token);
                }

                var json = JsonSerializer.Serialize(objects);

                var data = new StringContent(json, Encoding.UTF8, "application/json");

                var jsonResponse = await client.PostAsync(requestUri, data, new CancellationToken());

                // Get json data
                var result = jsonResponse.Content.ReadAsStringAsync().Result;

                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static async Task<ApiResponseWithStatusCode> PostJsonDatas(HttpClient client,
                                                                          string uri,
                                                                          string requestUri,
                                                                          Dictionary<string, object> httpHeaderValues,
                                                                          T objects,
                                                                          KeyValuePair<string, object>[] keyValuePairs = null)
        {
            var response = new ApiResponseWithStatusCode();
            HttpResponseMessage jsonResponse = null;

            client.BaseAddress = new Uri(uri);

            // Add an Accept header for JSON format
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            client.DefaultRequestHeaders.Clear();
            if (httpHeaderValues != null &&
                httpHeaderValues.Count() > 0)
            {
                httpHeaderValues.ToList()
                    .ForEach(
                        httpHeaderValue =>
                        {
                            client.DefaultRequestHeaders.Add(httpHeaderValue.Key, httpHeaderValue.Value.ToString());
                        });
            }

            // If request contains KeyValuePairs create response based on MultipartFormDataContent
            // Else create response based on StringContent
            if (keyValuePairs != null &&
                keyValuePairs.Count() > 0)
            {
                var multipartFormDataContent = new MultipartFormDataContent();
                keyValuePairs.ToList().ForEach(
                    KeyValuePair =>
                    {
                        multipartFormDataContent.Add(new StringContent(KeyValuePair.Value.ToString()), KeyValuePair.Key.ToString());
                    });

                jsonResponse = await client.PostAsync(requestUri, multipartFormDataContent, new CancellationToken());
            }
            else
            {
                var json = JsonSerializer.Serialize(objects);

                var data = new StringContent(json, Encoding.UTF8, "application/json");

                jsonResponse = await client.PostAsync(requestUri, data, new CancellationToken());
            }

            // Get json data
            var result = jsonResponse.Content.ReadAsStringAsync().Result;

            // Add JsonResponse and StatusCode
            response.JsonResponse = result;
            response.StatusCode = (int)jsonResponse.StatusCode;

            return response;
        }

        #endregion
    }
}
