using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.AzureBlobUtility.Models
{
    public class ImageProperties
    {
        public string Lattitude { get; set; }

        public string Longitude { get; set; }

        public string CapturedTime { get; set; }

        public bool IsFakeLocation { get; set; }

        public string Dimension { get; set; }

        public string Address { get; set; }
    }
}
