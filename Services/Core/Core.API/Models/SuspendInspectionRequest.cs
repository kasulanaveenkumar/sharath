using System;
using System.ComponentModel.DataAnnotations;

namespace Core.API.Models
{
    public class SuspendInspectionRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "InspectionId is required")]
        public Int32 InspectionId { get; set; }
    }
}
