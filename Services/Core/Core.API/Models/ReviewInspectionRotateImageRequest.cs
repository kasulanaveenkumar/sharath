using System;
using System.ComponentModel.DataAnnotations;

namespace Core.API.Models
{
    public class ReviewInspectionRotateImageRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "ImageId is required")]
        public Int64 ImageId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "RotationAngle is required")]
        public int RotationAngle { get; set; }
    }
}
