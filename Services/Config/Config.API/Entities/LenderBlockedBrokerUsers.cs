using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Config.API.Entities
{
    public class LenderBlockedBrokerUsers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long BrokerCompanyId { get; set; }

        public long LenderCompanyId { get; set; }

        [StringLength(50)]
        public string UserGuid { get; set; }
    }
}
