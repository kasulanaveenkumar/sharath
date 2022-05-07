using Common.Validations.Helper;
using System;
using System.ComponentModel.DataAnnotations;

namespace Core.API.Models
{
    public class ReminderRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Message is required")]
        [CommonStringValidator]
        public string Message { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "InspectionId is required")]
        public Int64 InspectionId { get; set; }
    }
}
