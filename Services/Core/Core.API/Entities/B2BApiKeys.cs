using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.API.Entities
{
    public class B2BApiKeys
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }

        public string Name { get; set; }

        public bool IsWriteEnabled { get; set; }

        public bool IsReadEnabled { get; set; }

        public string AllowedIPs { get; set; }

        [MaxLength(50)]
        public string APIKey { get; set; }

        [MaxLength(50)]
        public string APISecret { get; set; }

        public string CompanyGuid { get; set; }

        public bool IsDeleted { get; set; }

        public long CreatedBy { get; set; }

        public DateTime CreatedTime { get; set; }

        public Int64 UpdatedBy { get; set; }

        public DateTime? UpdatedTime { get; set; }
    }
}
