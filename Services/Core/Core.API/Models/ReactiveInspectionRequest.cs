using System;
using System.ComponentModel.DataAnnotations;

namespace Core.API.Models
{
    public class ReactiveInspectionRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "InspectionId is required")]
        public Int64 InspectionId { get; set; }
    }
}
