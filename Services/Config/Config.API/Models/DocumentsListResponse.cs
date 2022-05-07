using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Config.API.Models
{
    public class DocumentsListResponse
    {
        public Int64 Id { get; set; }

        public string Name { get; set; }

        public Int16 Position { get; set; }

        public Int32 CategoryId { get; set; }

        public decimal AdditionalPrice { get; set; }
    }
}
