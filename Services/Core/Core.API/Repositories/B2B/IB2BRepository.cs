using Core.API.Entities;
using Core.API.Entities.SP;
using System.Collections.Generic;

namespace Core.API.Repositories.B2B
{
    public interface IB2BRepository
    {
        public List<Usp_B2B_GetInspections> GetInspectionsList(Models.B2B.AllInspectionRequest model);

        public Applications GetInspectionDetails(long inspectionId, string companyGuid, string externalRef = "");

        public Applications GetInspectionDetailsById(long inspectionId);

        public List<AppUsers> GetAppUsers(long inspectionId);

        public void CancelInspection(Applications application);

        public void UpdateInspection(Applications application);

        public void UpdateAppUsers(AppUsers appUsers);

        public long GetStateIdByCode(string stateCode);

        public void GetUsersToShare(ref List<Models.B2B.Brokers> users, string companyGuid);

        public B2BApiKeys ValidateB2BToken(string token);

        public ADUsers GetUserDetailsByEmail(string email);

        public void AddWebHook(B2BWebHooks b2bWebHook);

        public void DeleteWebHook(B2BWebHooks b2bWebHook);

        public void DeleteAllWebHooks(List<B2BWebHooks> b2bWebHooks);

        public B2BWebHooks GetWebHookById(long id);

        public List<B2BWebHooks> GetAllWebHooksByCompany(string companyGuid);

        public void SaveDbChanges();
    }
}
