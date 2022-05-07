using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Config.API.Entities
{
    public class TemplateSetPlans
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }

        [Required]
        [StringLength(50)]
        public string PlanGuid { get; set; }

        [Required]
        [StringLength(50)]
        public string PlanName { get; set; }

        [StringLength(100)]
        public string PlanDescription { get; set; }

        public bool IsActive { get; set; }

        public Int16 PlanLevel { get; set; }

        // Need to remove below columns after sometime
        public Int64 TemplateSetId { get; set; }

        public Int16 MaxDocument { get; set; }

        public decimal Price { get; set; }

        public decimal LoanAmount { get; set; }

        public Int64 LenderCompanyId { get; set; }

        public bool IsDefaultActivated { get; set; }
    }
}
