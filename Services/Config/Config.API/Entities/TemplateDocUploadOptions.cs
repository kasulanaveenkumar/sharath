using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Config.API.Entities
{
    public class TemplateDocOptions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }

        public Int64 TemplateDocumentId { get; set; }

        /// <summary>
        /// 1 - Direct Options
        /// 2 - Options Based on State
        /// </summary>
        public Int16 OptionType { get; set; }

        [StringLength(100)]
        public string OptionName { get; set; }

        public Int16 ActionId { get; set; }
    }
}
