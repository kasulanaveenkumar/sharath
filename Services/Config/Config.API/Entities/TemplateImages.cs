using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Config.API.Entities
{
    public class TemplateImages
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(1000)]
        public string Description { get; set; }

        public Int16 ImageType { get; set; }

        public Int16 DocGroup { get; set; }

        public int TemplateDocCategoryId { get; set; }

        [Required]
        [StringLength(500)]
        public string WarningMessage { get; set; }

        public bool AllowSkip { get; set; }
    }
}
