using Common.Validations.Helper;
using System;
using System.ComponentModel.DataAnnotations;

namespace Config.API.Models
{
    public class AssetsWorkWithResponse
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "TemplateSetId is Required")]
        public Int64 TemplateSetId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "TemplateSetGUID is Required")]
        public string TemplateSetGUID { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "TemplateName is Required")]
        [MaxLength(100, ErrorMessage = "Template Name should not exceed more than 100 characters")]
        [CommonStringValidator]
        public string TemplateName { get; set; }

        public bool IsMapped { get; set; }

    }
}
