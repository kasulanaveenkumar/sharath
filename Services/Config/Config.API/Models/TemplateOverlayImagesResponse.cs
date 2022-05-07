using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Config.API.Models
{
    public class TemplateOverlayImagesResponse
    {
        public string OverlayImageUrl { get; set; }

        public Int16 ImageType { get; set; }

        public Int64 TemplateSetId { get; set; }
    }
}
