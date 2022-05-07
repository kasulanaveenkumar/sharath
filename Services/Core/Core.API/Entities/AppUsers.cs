using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.API.Entities
{
    public class AppUsers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }

        public Int64 ApplicationId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(50)]
        public string SurName { get; set; }

        [StringLength(256)]
        public string Email { get; set; }

        [StringLength(16)]
        public string PhoneNumber { get; set; }

        public Int16 Role { get; set; }

        [Required]
        [StringLength(50)]
        public string UserGuid { get; set; }

        [StringLength(10)]
        public string LoginOTP { get; set; }

        public DateTime? OTPGeneratedTime { get; set; }
    }
}
