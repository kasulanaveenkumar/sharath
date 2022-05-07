using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Config.API.Models
{
    public class TemplateHelpImagesResponse
    {
        public string HelpImageUrl { get; set; }

        public Int16 ImageType { get; set; }

        public Int64 TemplateSetId { get; set; }
    }
}
