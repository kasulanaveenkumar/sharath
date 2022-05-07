using System.Collections.Generic;
using Newtonsoft.Json;

namespace Common.Validations.Models
{
    public class ValidationError
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Field { get; }

        public string Message { get; }

        public ValidationError(string field, string message)
        {
            Field = !string.IsNullOrEmpty(field) ? field : null;

            Message = message;
        }
    }

    public class ValidationResultModel
    {
        public string Message { get; set; }

        public List<ValidationError> Errors { get; set; }
    }
}
