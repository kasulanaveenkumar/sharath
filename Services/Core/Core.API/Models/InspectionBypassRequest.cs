using Common.Validations.Helper;
using System;
using System.ComponentModel.DataAnnotations;

namespace Core.API.Models
{
    public class InspectionBypassRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "InspectionId is required")]
        public Int64 InspectionId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "ImageId is required")]
        public Int64 ImageId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "BypassReason is required")]
        [MaxLength(500, ErrorMessage = "BypassReason should not exceed more than 500 characters")]
        [CommonStringValidator]
        public string ByPassReason { get; set; }
    }
}
