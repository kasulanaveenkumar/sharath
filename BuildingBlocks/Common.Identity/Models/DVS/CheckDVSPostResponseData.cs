using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Identity.Models.DVS
{
    public class CheckDVSPostResponseData
    {
        public string Msg { get; set; }

        public Data Data { get; set; }
    }

    public class Data
    {
        public VerifyDocumentResult VerifyDocumentResult { get; set; }
    }

    public class VerifyDocumentResult
    {
        public string VerificationResultCode { get; set; }
    }
}
