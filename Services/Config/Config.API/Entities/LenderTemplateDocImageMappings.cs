using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Config.API.Entities
{
    public class LenderTemplateDocImageMappings
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }

        public Int64 LenderCompanyId { get; set; }

        public Int64 TemplateSetId { get; set; }

        public Int64 TemplateDocumentId { get; set; }

        public Int64 TemplateImageId { get; set; }

        public bool IsMandatory { get; set; }

        public bool IsSkippable { get; set; }

        public bool NotRequired { get; set; }
    }
}
