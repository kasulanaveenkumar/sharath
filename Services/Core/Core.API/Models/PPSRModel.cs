using Common.Validations.Helper;
using System.ComponentModel.DataAnnotations;

namespace Core.API.Models
{
    public class PPSRModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "InspectionId is required")]
        public int InspectionId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "PPSRImageType is required")]
        public int PPSRImageType { get; set; }

        [CommonStringValidator]
        public string HinNumber { get; set; }

        [CommonStringValidator]
        public string VinNumber { get; set; }

        public bool IsPESearchBySerial { get; set; }
    }
}
