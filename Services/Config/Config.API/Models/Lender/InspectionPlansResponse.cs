using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Config.API.Models.Lender
{
    public class TemplateSetandStates
    {
        public List<TemplateSets> TemplateSets { get; set; }

        public List<Models.StateOptionsResponse> StateOptions { get; set; }
    }

    public class TemplateSets
    {
        public string TemplateSetGuid { get; set; }

        public string TemplateName { get; set; }
    }

    public class AssetDocumentsListResponse
    {
        public Int64 Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string WarningMessage { get; set; }

        [JsonIgnore]
        public Int32 CategoryId { get; set; }

        [JsonIgnore]
        public Int32 Position { get; set; }

        public bool HasAdditionalData { get; set; }

        public Int16 MinimumPlanLevelToInclude { get; set; }

        public List<AssetImagesListResponse> ImageDetails { get; set; }
    }

    public class AssetImagesListResponse
    {
        public Int64 Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string WarningMessage { get; set; }

        public int ImageType { get; set; }

        public bool AllowSkip { get; set; }
    }
}
