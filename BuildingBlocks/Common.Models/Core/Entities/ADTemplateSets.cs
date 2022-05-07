using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Models.Core.Entities
{
    public class ADTemplateSets
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }

        public Int64 TemplateSetId { get; set; }

        public string TemplateSetGUID { get; set; }

        public string TemplateName { get; set; }
    }
}
