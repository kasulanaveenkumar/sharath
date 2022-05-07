using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.ApiResponse
{
    public class ApiResponseWithStatusCode
    {
        public string JsonResponse { get; set; }

        public int StatusCode { get; set; }
    }
}
