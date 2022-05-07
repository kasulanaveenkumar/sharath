using System;
using System.Collections.Generic;

namespace Core.API.Models.B2B
{
    public class NewInspectionDetailsResponse
    {
        public NewInspectionDetailsResponse()
        {
            Lenders = new List<LendersWorkWithResponse>();

            Assets = new List<AssetsWorkWithResponse>();

            States = new List<StateOptionsResponse>();

            BrokerUsers = new List<BrokerUsers>();

            TemplateDetails = new TemplateDetails();
        }

        public bool ExemptPayment { get; set; }

        public List<LendersWorkWithResponse> Lenders { get; set; }

        public List<AssetsWorkWithResponse> Assets { get; set; }

        public List<StateOptionsResponse> States { get; set; }

        public List<BrokerUsers> BrokerUsers { get; set; }

        public TemplateDetails TemplateDetails { get; set; }
    }

    public class LendersWorkWithResponse
    {
        public string LenderGUID { get; set; }

        public string LenderName { get; set; }

        public bool IsPayer { get; set; }

        public bool IsMapped { get; set; }

        public bool ExcemptPayment { get; set; }
    }

    public class AssetsWorkWithResponse
    {
        public Int64 TemplateSetId { get; set; }

        public string TemplateSetGUID { get; set; }

        public string TemplateName { get; set; }

        public bool IsMapped { get; set; }

    }

    public class StateOptionsResponse
    {
        public long StateID { get; set; }

        public string StateCode { get; set; }
    }

    public class TemplateDetails
    {
        public TemplateDetails()
        {
            DocumentDetails = new List<AssetDocListResponse>();
        }
        public decimal BasePrice { get; set; }

        public List<AssetDocListResponse> DocumentDetails { get; set; }

        public bool IsPreferenceSaved { get; set; }
    }

    public class AssetDocListResponse
    {
        public AssetDocListResponse()
        {
            ImageDetails = new List<AssetImageListResponse>();
        }
        public Int64 DocumentId { get; set; }

        public string DocumentName { get; set; }

        public string DocDescription { get; set; }

        public decimal AdditionalPrice { get; set; }

        public string WarningMessage { get; set; }

        public int DisplayPosition { get; set; }

        public int Position { get; set; }

        public bool IsAdditionalDataRequired { get; set; }

        public bool IsAdditionalDataMandatory { get; set; }

        public List<AssetImageListResponse> ImageDetails { get; set; }

        public string DocumentRequired { get; set; }
    }

    public class AssetImageListResponse
    {
        public int ImageType { get; set; }

        public string ImageName { get; set; }

        public int DocGroup { get; set; }

        public int Position { get; set; }

        public string Description { get; set; }

        public bool IsMandatory { get; set; }

        public bool IsDefaultSelected { get; set; }

        public bool IsCheckboxDisabled { get; set; }

        public string WarningMessage { get; set; }
    }
}
