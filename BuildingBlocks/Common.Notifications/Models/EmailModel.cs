using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Notifications.Models
{
    public class EmailModel
    {
        public string TemplateId { get; set; }

        public string ToEmail { get; set; } 
        
        public object TemplateData { get; set; }

        public string Subject { get; set; }

        /// <summary>
        /// Attachments in Base64 String
        /// </summary>
        public List<FileAttachments> FileAttachments { get; set; }
    }

    public class FileAttachments
    {
        public string FileName { get; set; }

        public string Base64FileData { get; set; }
    }
}
