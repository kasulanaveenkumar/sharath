using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Config.API.Entities
{
    public class StaCollectorDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }

        [StringLength(100)]
        public string CompanyName { get; set; }

        [StringLength(255)]
        public string CompanyAddress { get; set; }

        [StringLength(500)]
        public string CompanyDescription { get; set; }

        [StringLength(50)]
        public string LogoGuid { get; set; }

        [StringLength(100)]
        public string UserFirstName { get; set; }

        [StringLength(100)]
        public string UserLastName { get; set; }

        [StringLength(100)]
        public string UserEmail { get; set; }

        [StringLength(20)]
        public string UserMobileNumber { get; set; }

        public Int32 CollectorAggregatorRole { get; set; }

        public string ErrorMessage { get; set; }
    }
}
