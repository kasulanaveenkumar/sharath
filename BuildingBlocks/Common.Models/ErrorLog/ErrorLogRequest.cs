using System;

namespace Common.Models.ErrorLog
{
    public class ErrorLogRequest
    {
        public Exception ErrorMessage { get; set; }

        public object Model { get; set; }
    }
}
