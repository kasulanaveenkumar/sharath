using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.API.Models.Data
{
    public class LenderWorkWithResponse
    {
        public string LenderGUID { get; set; }

        public string LenderName { get; set; }

        public bool IsPayer { get; set; }

        public bool ExcemptPayment { get; set; }

        public bool IsMapped { get; set; }
    }
}
