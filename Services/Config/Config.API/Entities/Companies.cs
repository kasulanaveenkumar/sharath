using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Config.API.Entities
{
    public class Companies
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }

        [Required]
        [StringLength(50)]
        public string CompanyGuid { get; set; }

        [Required]
        [StringLength(100)]
        public string CompanyName { get; set; }

        [StringLength(100)]
        public string CompanyLogoURL { get; set; }

        [StringLength(500)]
        public string CompanyDescription { get; set; }

        [StringLength(100)]
        public string RegisteredName { get; set; }

        [StringLength(50)]
        public string ABN { get; set; }

        [StringLength(255)]
        public string CompanyAddress { get; set; }

        [StringLength(255)]
        public string City { get; set; }

        [StringLength(255)]
        public string State { get; set; }

        [StringLength(20)]
        public string ZIPCode { get; set; }

        [StringLength(255)]
        public string Website { get; set; }

        [StringLength(255)]
        public string Email { get; set; }

        /// <summary>
        /// 1 - Broker | 2 - Lender
        /// </summary>
        public Int64 CompanyTypeId { get; set; }

        public bool IsPayer { get; set; }

        public Int64 CreatedBy { get; set; }

        public DateTime CreatedTime { get; set; }

        public Int64 UpdatedBy { get; set; }

        public DateTime? UpdatedTime { get; set; }


        [StringLength(100)]
        public string ContractLocation { get; set; }
        public DateTime? SignDate { get; set; }
        public DateTime? GoLiveDate { get; set; }
        /// <summary>
        /// 0 - All | 1 - Self | 2 - Selective
        /// </summary>
        public Int16 LenderVisibility { get; set; }

        public bool ExcemptPayment { get; set; }

        public bool AllowOnlyInvitedUser { get; set; }

        [StringLength(50)]
        public string PaymentCustomerId { get; set; }

        [StringLength(50)]
        public string PrimaryPaymentId { get; set; }

        public bool IsUseCompanyLogoForWebApp { get; set; }
    }
}
