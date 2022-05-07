using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Models.Core.Entities
{
    public class AppImageReasons
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }

        public Int64 ApplicationId { get; set; }

        public Int64 AppImageId { get; set; }

        public Int64 ReasonId { get; set; }

        public Int64 ReasonType { get; set; }

    }
}
