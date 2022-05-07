using Common.Validations.Helper;
using System;
using System.ComponentModel.DataAnnotations;

namespace Core.API.Models
{
    public class LenderInspectionsRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "SortColumn is required")]
        [CommonStringValidator]
        public string SortColumn { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "SortDirection is required")]
        [CommonStringValidator]
        public string SortDirection { get; set; }

        [CommonStringValidator]
        public string AssetFilter { get; set; }

        [CommonStringValidator]
        public string BrokerFilter { get; set; }

        public Int32 StatusFilter { get; set; }

        [CommonStringValidator]
        public string FilterText { get; set; }

        public Int32 SkipData { get; set; }

        public Int32 LimitData { get; set; }

        public bool IsDisplayCompletedInspections { get; set; }
    }
}
