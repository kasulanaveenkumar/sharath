using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Config.API.Entities
{
    public class LenderTemplateSetDocMappings
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }

        public Int64 LenderCompanyId { get; set; }

        public Int64 TemplateSetId { get; set; }

        public Int64 TemplateDocumentId { get; set; }

        public string UploadOptions { get; set; }
    }

    public class UploadOption
    {
        public List<DirectOption> DirectOptions { get; set; }

        public List<StateOption> StateOptions { get; set; }
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
