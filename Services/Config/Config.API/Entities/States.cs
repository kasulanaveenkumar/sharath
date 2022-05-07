using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Config.API.Entities
{
    public class States
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [StringLength(100)]
        public string Country { get; set; }

        [Required]
        [StringLength(20)]
        public string CountryCode { get; set; }

        [StringLength(100)]
        public string State { get; set; }
        
        [Required]
        [StringLength(20)]
        public string StateCode { get; set; }
    }
}
