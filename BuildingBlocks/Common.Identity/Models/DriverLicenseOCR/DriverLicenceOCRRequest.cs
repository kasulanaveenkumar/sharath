using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Common.Identity.Models.DriverLicenseOCR
{
    public class DriverLicenceOCRRequest
    {
        public KeyValuePair<string, object>[] DriverLicenceOCRImages { get; set; }
    }
}
