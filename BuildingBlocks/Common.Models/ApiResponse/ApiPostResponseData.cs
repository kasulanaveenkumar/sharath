using System;
using System.Net;

namespace Common.Models.ApiResponse
{
    public class ApiPostResponseData
    {
        public long CompanyId { get; set; }

        public string CompanyGUID { get; set; }

        public string InvalidCardDetails { get; set; }

        public string ErrorMessage { get; set; }

        public Exception ExceptionMessage { get; set; }

        public object Message { get; set; }

        public string UserGuid { get; set; }

        public string Result { get; set; }
    }
}
