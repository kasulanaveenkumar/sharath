using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Config.API.Models.Asset
{
    public class TemplateDetails
    {
        public TemplateDetails()
        {
            DocumentDetails = new List<AssetDocListResponse>();
        }
        public decimal BasePrice { get; set; }

        public List<AssetDocListResponse> DocumentDetails { get; set; }

        public bool IsPreferenceSaved { get; set; }
    }

    public class AssetDocListResponse
    {
        public AssetDocListResponse()
        {
            ImageDetails = new List<AssetImageListResponse>();
        }
        public Int64 DocumentId { get; set; }

        public string DocumentName { get; set; }

        public string DocDescription { get; set; }

        public decimal AdditionalPrice { get; set; }

        public string WarningMessage { get; set; }

        public int DisplayPosition { get; set; }

        public int Position { get; set; }

        public bool IsAdditionalDataRequired { get; set; }

        public bool IsAdditionalDataMandatory { get; set; }

        public List<AssetImageListResponse> ImageDetails { get; set; }

        public string DocumentRequired { get; set; }
    }

    public class AssetImageListResponse
    {
        public int ImageType { get; set; }

        public string ImageName { get; set; }
        
        public int DocGroup { get; set; }

        public int Position { get; set; }

        public string Description { get; set; }

        public bool IsMandatory { get; set; }
        
        public bool IsDefaultSelected { get; set; }

        public bool IsCheckboxDisabled { get; set; }

        public string WarningMessage { get; set; }
    }
}
