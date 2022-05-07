using Core.API.Models;

namespace Core.API.Services
{
    public interface IAdminService
    {
        public AdminInspectionsFilterResponse GetInspectionsFilter();

        public AdminReviewInspectionsResponse GetReviewInspectionsList(AdminInspectionsRequest model, string userGuid, string companyGuid);

        public int GetReverseTimerUpdateStatus();

        public void SuspendInspection(long inspectionId, long userId);

        public void ReactiveInspection(long inspectionId, long userId);

        public void PurgeInspection(long inspectionId, long userId);

        public AdminCompletednspectionsResponse GetCompletedInspectionsList(AdminInspectionsRequest model, string userGuid, string companyGuid);

        public CompaniesDashboardResponse GetCompaniesDashboardDatas(GetCompanyDashboardDatasRequest model);
    }
}
