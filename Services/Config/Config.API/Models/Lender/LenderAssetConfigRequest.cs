using System;
using System.Collections.Generic;

namespace Config.API.Models.Lender
{
    public class LenderAssetConfigRequest
    {
        public string TemplateSetGuid { get; set; }

        public string CompanyGuid { get; set; }

        public List<DocConfig> Documents { get; set; }
    }

    public class DocConfig
    {
        public Int64 TemplateDocumentId { get; set; }

        public string DocumentName { get; set; }

        public List<DirectOption> DirectOptions { get; set; }

        public List<StateOption> StateOptions { get; set; }

        public List<DocImageConfig> DocImages { get; set; }
    }

    public class DocImageConfig
    {
        public Int64 TemplateImageId { get; set; }

        public bool IsMandatory { get; set; }

        public bool IsSkippable { get; set; }

        public bool NotRequired { get; set; }
    }
}
