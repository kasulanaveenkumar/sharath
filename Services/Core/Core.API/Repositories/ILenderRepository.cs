using Core.API.Entities.SP;
using System.Collections.Generic;

namespace Core.API.Repositories
{
    public interface ILenderRepository
    {
        public List<Models.TemplateSets> GetAllAssets();

        public List<Models.Brokers> GetAllBrokers();

        public List<Models.ApplicationStatus> GetApplicationStatuses();

        public List<Usp_GetInspections> GetInspectionsList(Models.LenderInspectionsRequest model, string userGuid, string companyGuid);

        public List<Usp_GetInspections> GetCompletedInspectionsList(Models.LenderInspectionsRequest model, string userGuid, string companyGuid);
    }
}
