using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Models.Core.Entities
{
    public class CoreConfigs
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        public string Value { get; set; }

        [StringLength(500)]
        public string Comments { get; set; }
    }
}
