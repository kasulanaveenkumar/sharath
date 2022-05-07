using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Config.API.Entities
{
    public class TemplateSetPlanMappings
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }

        public Int64 TemplateSetId { get; set; }

        public Int64 PlanId { get; set; }

        public decimal BasePrice { get; set; }

        public bool IsActive { get; set; }
    }
}
