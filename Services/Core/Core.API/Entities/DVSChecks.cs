using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.API.Entities
{
    public class DVSChecks
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }

        public DateTime RequestDate { get; set; }

        public int? Result { get; set; }

        public string Message { get; set; }

        public Int64 ApplicationId { get; set; }
    }
}
