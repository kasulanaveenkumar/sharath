using Common.AzureBlobUtility.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.API.Models
{
    public class InspectionGetImageResponse
    {
        public string ImageData { get; set; }

        public ImageProperties ImageProperties { get; set; }
    }
}
