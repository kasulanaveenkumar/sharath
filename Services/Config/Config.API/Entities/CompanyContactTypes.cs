using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Config.API.Entities
{
    public class CompanyContactTypes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int16 Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }
    }
}
