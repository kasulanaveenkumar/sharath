using System;
using System.ComponentModel.DataAnnotations;

namespace Config.API.Models.Asset
{
    public class AssetDocListRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "TemplateGuid is required")]
        public string TemplateGuid { get; set; }

        public string planGuid { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "LenderGuid is required")]
        public string LenderGuid { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "StateId is required")]
        public Int64 StateId { get; set; }

        public bool IsIncludeNoLenderPreference { get; set; }
    }
}
