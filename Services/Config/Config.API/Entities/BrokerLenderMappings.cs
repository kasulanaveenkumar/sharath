using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Config.API.Entities
{
    public class BrokerLenderMappings
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long BrokerCompanyId { get; set; }

        public long LenderCompanyId { get; set; }

    }
}
