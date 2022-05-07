using Common.Validations.Helper;
using System;
using System.ComponentModel.DataAnnotations;

namespace Common.Identity.Models.DVS
{
    public class CheckDVSRequest : KreanoDVSCheckModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "InspectionId is required")]
        public Int64 InspectionId { get; set; }
    }

    public class KreanoDVSCheckModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "BirthDate is required")]
        [CommonStringValidator]
        public string BirthDate { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "GivenName is required")]
        [CommonStringValidator]
        public string GivenName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "FamilyName is required")]
        [CommonStringValidator]
        public string FamilyName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "LicenceNumber is required")]
        [CommonStringValidator]
        public string LicenceNumber { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "StateOfIssue is required")]
        [CommonStringValidator]
        public string StateOfIssue { get; set; }
    }
}
