using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Config.API.Models
{
    public class LenderBlockedBrokerUsersRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "BrokerCompanyGuid is Required")]
        public string BrokerCompanyGuid { get; set; }

        public List<BlockedUsers> BlockedUsers { get; set; }
    }

    public class BlockedUsers
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "UserGuid is Required")]
        public string UserGuid { get; set; }

        public bool IsAllowed { get; set; }
    }
}
