using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Config.API.Models
{
    public class LenderAssetConfig
    {
        public Int64 TemplateDocumentId { get; set; }

        public string DocumentName { get; set; }

        public Int16 Position { get; set; }

        public List<TemplateImage> TemplateImages { get; set; }

        public Int16 OptionType { get; set; }

        public List<DirectOption> DirectOptions { get; set; }

        public List<StateOption> StateOptions { get; set; }
    }

    public class TemplateImage
    {
        public Int64 ImageId { get; set; }
        public string ImageName { get; set; }
        public bool IsMandatory { get; set; }
        public bool IsDefaultSelected { get; set; }
        public Int16 Position { get; set; }
    }

    public class DirectOption
    {
        public string OptionName { get; set; }

        public Int16 ActionId { get; set; }
    }

    public class StateOption
    {
        public string StateCode { get; set; }

        public bool IsMandatory { get; set; }

        public bool IsSkippable { get; set; }

        public bool NotRequired { get; set; }
    }
}
