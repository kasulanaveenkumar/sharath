using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Identity.Models.DriverLicenseOCR
{
    public class DriverLicenseOCRPostResponseData
    {
        public string Msg { get; set; }

        public Data Data { get; set; }
    }

    public class Data
    {

        public string Name { get; set; }

        public string MiddleName { get; set; }

        public string SurName { get; set; }

        public string Address { get; set; }

        public string LicenseNumber { get; set; }

        public string ExpiryDate { get; set; }

        public string DateOfBirth { get; set; }

        public string StateIssue { get; set; }

        public string Newaddress { get; set; }
    }

}
