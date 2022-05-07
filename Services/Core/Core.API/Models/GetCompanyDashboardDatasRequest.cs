using Common.Validations.Helper;

namespace Core.API.Models
{
    public class GetCompanyDashboardDatasRequest
    {
        [CommonStringValidator]
        public string LenderFilter { get; set; }

        [CommonStringValidator]
        public string FromDateFilter { get; set; }

        [CommonStringValidator]
        public string ToDateFilter { get; set; }
    }
}
