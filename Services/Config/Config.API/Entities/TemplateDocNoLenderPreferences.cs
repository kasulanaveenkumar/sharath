using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Config.API.Entities
{
    public class TemplateDocNoLenderPreferences
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }

        public Int64 TemplateSetId { get; set; }

        public Int64 TemplateSetPlanId { get; set; }

        [StringLength(50)]
        public string UserGuid { get; set; }
        
        [StringLength(50)]
        public string CompanyGuid { get; set; }

        public string Preference { get; set; }

        public bool IsPreferenceSaved { get; set; }
    }
}
