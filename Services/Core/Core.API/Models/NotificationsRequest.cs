using System.ComponentModel.DataAnnotations;

namespace Core.API.Models
{
    public class NotificationsRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "NotificationsGuid is required")]
        public string NotificationsGuid { get; set; }

        public bool IsSelected { get; set; }
    }
}
