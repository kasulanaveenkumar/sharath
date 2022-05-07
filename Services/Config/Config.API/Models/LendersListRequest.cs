using Common.Validations.Helper;
using System.ComponentModel.DataAnnotations;

namespace Config.API.Models
{
    public class LendersListRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "SortColumn is required")]
        [CommonStringValidator]
        public string SortColumn { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "SortDirection is required")]
        [CommonStringValidator]
        public string SortDirection { get; set; }
    }
}
