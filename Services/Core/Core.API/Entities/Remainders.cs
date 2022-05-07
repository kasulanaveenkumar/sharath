using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Core.API.Entities
{
    public class Remainders
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }

        [Required]
        [StringLength(50)]
        public string RemainderGuid { get; set; }

        public string RemainderTemplate { get; set; }

        public bool IsActive { get; set; }
    }
}
