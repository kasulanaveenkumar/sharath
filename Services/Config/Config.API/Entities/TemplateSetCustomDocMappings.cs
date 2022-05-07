using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Config.API.Entities
{
    public class TemplateSetCustomDocMappings
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }

        public Int64 CustomPlanId { get; set; }

        public string StateCode { get; set; }

        public Int64 StateId { get; set; }

        public Int64 TemplateDocumentId { get; set; }

        public bool IsSelected { get; set; }

        public bool IsAdditionalDataRequired { get; set; }

        public bool IsAdditionalDataMandatory { get; set; }
    }
}
