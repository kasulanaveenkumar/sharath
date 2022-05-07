using Core.API.Entities;
using Core.API.Entities.SP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.API.Repositories
{
    public interface IAdminRepository
    {
        public List<Models.TemplateSets> GetAllAssets();

        public List<Models.Lenders> GetAllLenders();

        public List<Models.ApplicationStatus> GetApplicationStatuses();

        public List<Usp_GetInspections> GetReviewInspectionsList(Models.AdminInspectionsRequest model, string userGuid, string companyGuid,
                                                                 out long recordsCount);

        public int CalcuteSLADuration(DateTime uploadedTime);

        public List<Usp_GetInspections> GetCompletedInspectionsList(Models.AdminInspectionsRequest model, string userGuid, string companyGuid,
                                                                    out long recordsCount);

        public Applications GetInspectionDetailsById(long inspectionId);

        public void SuspendInspection(Applications application, long userId);

        public void ReactiveInspection(Applications application, long userId);

        public void DataPurgeProcess(long inspectionId, long userId);

        public Models.CompaniesDashboardResponse GetCompaniesDashboardDatas(Models.GetCompanyDashboardDatasRequest model);
    }
}
