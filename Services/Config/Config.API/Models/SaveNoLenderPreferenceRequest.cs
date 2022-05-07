using System;
using System.Collections.Generic;

namespace Config.API.Models
{
    public class SaveNoLenderPreferenceRequest
    {
        public string TemplateSetGuid { get; set; }

        public List<NoLenderPreference> NoLenderPreferences { get; set; }

        public bool IsPreferenceSaved { get; set; }
    }

    public class NoLenderPreference
    {
        public Int64 DocumentId { get; set; }

        public List<int> ImageTypes { get; set; }
    }
}
