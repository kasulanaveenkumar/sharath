using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.API.Entities
{
    public class AppImages
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }

        public Int64 ApplicationId { get; set; }

        public Int64 AppDocumentId { get; set; }

        [StringLength(100)]
        public string ImageName { get; set; }

        public Int16 DocGroup { get; set; }

        public Int16 ImageType { get; set; }

        public Int16 ImageInternalStatus { get; set; }

        public Int16 ImageStatus { get; set; }

        [StringLength(200)]
        public string FilePath { get; set; }

        [StringLength(50)]
        public string FileName { get; set; }

        [StringLength(10)]
        public string Extension { get; set; }

        [StringLength(20)]
        public string SizeInKb { get; set; }
 
        public string ImageData { get; set; }

        [StringLength(500)]
        public string OtherFlagReason { get; set; }
        
        [StringLength(500)]
        public string OtherRejectReason { get; set; }

        public Int16 UserType { get; set; }

        public Int64 UpdatedBy { get; set; }

        public DateTime UpdatedTime { get; set; }

        public bool IsBypassRequested { get; set; }
     
        [StringLength(500)]
        public string BypassReason { get; set; }

        public bool IsSkipped { get; set; }

        public bool IsSkippable { get; set; }

        [StringLength(50)]
        public string BypassRequestedBy { get; set; }
    }
}
