using System.ComponentModel.DataAnnotations;

namespace Config.API.Models
{
    public class InspectionPlansRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "LenderGuid is required")]
        public string LenderGuid { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "TemplateGuid is required")]
        public string TemplateGuid { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "StateId is required")]
        public long StateId { get; set; }
    }
}
