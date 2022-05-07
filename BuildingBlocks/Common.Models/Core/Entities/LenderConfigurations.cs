using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Models.Core.Entities
{
    public class LenderConfigurations
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }

        [Required]
        [StringLength(50)]
        public string LenderCompanyGuid { get; set; }

        public string AdditionalTnC { get; set; }

        public string CorrectInfoStatement { get; set; }

        public bool IsNonOwnerAllowed { get; set; }

        public bool IsRoadworthyAllowed { get; set; }

        public bool IsBSAllowed { get; set; }
        public bool AllowViewBS { get; set; }

        public bool IsReportRequired { get; set; }
        //This is to decide whether the lender wants the Sales Invoice

        public string ReportEmailAddress { get; set; }

        public string ReportImageUrl { get; set; }

        public bool BypassHourMeter { get; set; }

        public bool IsSalesInvoiceRequired { get; set; }
        //This is to enable/disable the skip option for sales Invoice  in App

        public bool IsByPassSalesInvoiceAllowed { get; set; }
        //This is to decide whether the lender wants the AdditionalData to be displayed in App

        public bool IsAdditionalDataRequired { get; set; }
        //This is to enable/disable the skip option for AdditionalData  in App

        public bool IsByPassAdditionalDataAllowed { get; set; }

        public bool IsIllionIntegrationEnabled { get; set; }
        public bool IsAPIIntegrationEnabled { get; set; }

        [StringLength(500)]
        public string LenderRefPrefix { get; set; }

        public bool IsAllowAwaitedRef { get; set; }

        public bool IsForceLenderRefFormat { get; set; }
    }
}
