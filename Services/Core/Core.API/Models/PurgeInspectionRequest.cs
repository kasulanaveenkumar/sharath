using System;
using System.ComponentModel.DataAnnotations;

namespace Core.API.Models
{
    public class PurgeInspectionRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "InspectionId is Required")]
        public Int64 InspectionId { get; set; }
    }
}
