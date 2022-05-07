using System.Collections.Generic;

namespace Config.API.Models.Lender
{
    public class LenderAssetConfigResponse
    {
        public LenderAssetConfigResponse()
        {
            Documents = new List<DocConfig>();
        }

        public List<DocConfig> Documents { get; set; }
    }
}
