using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Core.Entities
{
    public class TemplateImageOverlayImages
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }

        public string OverlayImageUrl { get; set; }

        public Int16 ImageType { get; set; }

        public Int64 TemplateSetId { get; set; }
    }
}
