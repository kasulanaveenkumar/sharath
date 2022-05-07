using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.API.Entities
{
    public class ADTemplateSetPlans
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }

        public string TemplateGuid { get; set; }

        public string PlanGuid { get; set; }

        public decimal Price { get; set; }

        public bool IsDefaultActivated { get; set; }
    }
}
