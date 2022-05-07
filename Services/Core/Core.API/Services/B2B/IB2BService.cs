using Core.API.Entities;
using Core.API.Models;
using Core.API.Models.B2B;
using System;
using System.Collections.Generic;

namespace Core.API.Services.B2B
{
    public interface IB2BService
    {
        public List<AllInspectionResponse> GetInspectionsList(AllInspectionRequest model);

        public Applications GetInspectionDetails(long inspectionId, string companyGuid, string exterRef = "");

        public List<AppUsers> GetAppUsers(long inspectionId);

        public void CancelInspection(Models.B2B.CancelInspectionRequest model, Applications application);

        public void UpdateInspection(Applications application, UpdateInspectionRequest model, long userId);

        public void UpdateAppUsers(List<AppUsers> appUsers, UpdateInspectionRequest model);

        public string GetReport(Applications application);

        public long CreateNewInspection(CreateInspectionRequest model, string companyGuid,
                                                out string paymentFailedReason, out Exception errorMessage);

        public void SendInspectionCreatedEmail(long inspectionId);

        public void SendInspectionCreatedSms(long inspectionId);

        public int SendReminder(ReminderRequest model, long userId);

        public B2BWebHooks GetWebHookById(long id);

        public List<B2BWebHooks> GetWebHooksByCompany(string companyGuid);

        public List<WebHookSubscribed> GetAllWebBooksByCompany(string companyGuid);

        public void SubscribeWebHooks(WebHookSubscribe model, string companyGuid);

        public void UnsubscribeWebHooks(B2BWebHooks b2bWeHook);

        public void UnsubscribeAllWebHooks(List<B2BWebHooks> b2bWebHooks);
    }
}
