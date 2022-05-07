using Common.Validations.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core.API.Models
{
    public class ReviewInspectionSaveRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "InspectionId is required")]
        public Int64 InspectionId { get; set; }

        public List<ReviewInspectionImages> Images { get; set; }

        public bool IsSaveDraft { get; set; }

        public bool IsSendAutomaticReport { get; set; }
    }

    public class ReviewInspectionImages
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "ImageId is required")]
        public Int64 ImageId { get; set; }

        public List<int> SelectedFlagReasons { get; set; }

        public List<int> SelectedRejectedReasons { get; set; }

        public string ImageInternalStatus { get; set; }

        public string OtherFlagReason { get; set; }

        public string OtherRejectReason { get; set; }

        [MaxLength(500, ErrorMessage = "BypassNotes should not exceed more than 500 characters")]
        [CommonStringValidator]
        public string BypassNotes { get; set; }

        public string ImageData { get; set; }
    }
}
