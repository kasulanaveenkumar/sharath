using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Core.API.Entities
{
    public class PaymentLogs
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }

        public Int64 ApplicationId { get; set; }

        public Int32 InspectionType { get; set; }

        public decimal Amount { get; set; }

        public DateTime PaymentTime { get; set; }

        public Int32 PaymentStatus { get; set; }

        public string FailedReason { get; set; }

        public string TransactionId { get; set; }

        public bool IsPaidByCard { get; set; }

        public bool IsInvoiced { get; set; }

        public string InvoiceCompanyGuid { get; set; }

        public decimal TaxAmount { get; set; }
    }
}
