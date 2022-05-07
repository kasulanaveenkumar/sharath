using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Core.API.Entities
{
    public class ErrorLogs
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }

        [StringLength(1000)]
        public string Member { get; set; }

        [StringLength(1000)]
        public string FilePath { get; set; }

        public string Message { get; set; }

        public string StackTrace { get; set; }

        public string CompleteException { get; set; }

        public string AdditionalDetails { get; set; }

        public string RequestData { get; set; }

        [StringLength(50)]
        public string UserGuid { get; set; }

        [StringLength(50)]
        public string CompanyGuid { get; set; }

        public DateTime ErrorTime { get; set; }
    }
}
