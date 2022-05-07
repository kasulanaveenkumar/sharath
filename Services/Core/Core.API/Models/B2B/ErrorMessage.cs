using System.Collections.Generic;

namespace Core.API.Models.B2B
{
    public class ErrorMessage
    {
        public string Message { get; set; }

        public List<string> ErrorMessages { get; set; }

    }
}
