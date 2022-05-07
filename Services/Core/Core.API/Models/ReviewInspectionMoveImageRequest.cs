using System;
using System.ComponentModel.DataAnnotations;

namespace Core.API.Models
{
    public class ReviewInspectionMoveImageRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "InspectionId is required")]
        public Int64 InspectionId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "SourceImageId is required")]
        public Int64 SourceImageId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "DestinationImageId is required")]
        public Int64 DestinationImageId { get; set; }
    }
}
