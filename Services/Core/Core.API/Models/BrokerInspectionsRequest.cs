using Common.Validations.Helper;
using System;
using System.ComponentModel.DataAnnotations;

namespace Core.API.Models
{
    public class BrokerInspectionsRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "SortColumn is required")]
        [CommonStringValidator]
        public string SortColumn { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "SortDirection is required")]
        [CommonStringValidator]
        public string SortDirection { get; set; }

        public string AssetFilter { get; set; }

        public string LenderFilter { get; set; }

        public Int32 StatusFilter { get; set; }

        public string FilterText { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "SkipData is required")]
        [CommonStringValidator]
        public Int32 SkipData { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "LimitData is required")]
        [CommonStringValidator]
        public Int32 LimitData { get; set; }
    }
}
