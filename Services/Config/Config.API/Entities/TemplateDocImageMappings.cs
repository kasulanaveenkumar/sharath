using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Config.API.Entities
{
    public class TemplateDocImageMappings
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }

        public Int64 TemplateDocumentId { get; set; }

        public Int64 TemplateImageId { get; set; }

        public Int16 Position { get; set; }
        
        public bool IsMandatory { get; set; }

        public bool IsDefaultSelected { get; set; }
    }
}
