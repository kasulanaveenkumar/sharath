using System;
using System.ComponentModel.DataAnnotations;

namespace Config.API.Models
{
    public class SaveLenderVisibilityRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "BrokerCompanyId is required")]
        public Int64 BrokerCompanyId { get; set; }
        public bool IsVisible { get; set; }
    }
}
