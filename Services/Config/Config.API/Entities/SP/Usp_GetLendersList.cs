using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Config.API.Entities.SP
{
    public class Usp_GetLendersList
    {
		public Int64 LenderId { get; set; }

		public string LenderCompanyGuid { get; set; }

		public string LenderName { get; set; }
			
		public string ABN { get; set; }
			
		public string Website { get; set; }
			
		public string Email { get; set; }
			
		public string LiveStatus { get; set; }
    }
}
