using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Config.API.Entities
{
    public class TemplateSetDocMappings
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }

        public Int64 TemplateSetId { get; set; }

        public Int64 TemplateDocumentId { get; set; }

        public bool IsMandatory { get; set; }

        public Int16 Position { get; set; }

        public bool IsDefaultSelected { get; set; }

        public bool HasAdditionalData { get; set; }
    }
}
