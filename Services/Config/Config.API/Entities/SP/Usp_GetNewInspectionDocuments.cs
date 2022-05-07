using System;

namespace Config.API.Entities.SP
{
    public class Usp_GetNewInspectionDocuments
    {
        public Int64 DocId { get; set; }

        public string DocName { get; set; }

        public decimal AdditionalPrice { get; set; }

        public string DocDescription { get; set; }

        public string DocWarningMessage { get; set; }

        public Int16 DocPosition { get; set; }

        public bool IsAdditionalDataRequired { get; set; }

        public bool IsAdditionalDataMandatory { get; set; }

        public Int16 ImageType { get; set; }

        public string ImageName { get; set; }

        public Int16 DocGroup { get; set; }

        public Int16 ImagePosition { get; set; }

        public string ImageDescription { get; set; }

        public bool IsMandatory { get; set; }

        public bool IsDefaultSelected { get; set; }

        public bool IsCheckboxDisabled { get; set; }

        public string ImageWarningMessage { get; set; }

        public decimal BasePrice { get; set; }
    }
}
