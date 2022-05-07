using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Models.Core.Entities
{
    public class ADCompanies
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }

        public string CompanyGuid { get; set; }

        public string CompanyName { get; set; }

        public Int64 CompanyTypeId { get; set; }

        public bool IsPayer { get; set; }

        public bool ExcemptPayment { get; set; }

        public bool IsUseCompanyLogoForWebApp { get; set; }
    }
}
