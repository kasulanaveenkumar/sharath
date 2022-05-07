using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Core.API.Entities
{
    public class AppDocuments
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }

        public Int64 ApplicationId { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        public DateTime UpdatedDate { get; set; }

        public Int16 DocStatus { get; set; }

        public bool IsAdditionalDataRequired { get; set; }

        public bool IsAdditionalDataMandatory { get; set; }
    }
}
