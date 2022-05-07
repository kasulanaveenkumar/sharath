using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Extensions.Models
{
    public class ErrorLog
    {
        public ErrorLog()
        {
            ErrorTime = DateTime.UtcNow;
        }

        public string FilePath { get; set; }
        
        public string Member { get; set; }

        public string Message { get; set; }

        public string StackTrace { get; set; }

        public string CompleteException { get; set; }

        public string RequestData { get; set; }

        public string AdditionalDetails { get; set; }

        public DateTime ErrorTime { get; set; }
    }
}
