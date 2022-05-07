using Core.API.Entities;
using Core.API.Entities.SP;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Core.API.Repositories.B2B
{
    public class B2BRepository : IB2BRepository
    {
        private readonly CoreContext dbContext;

        #region Constructor

        public B2BRepository(Entities.CoreContext context)
        {
            dbContext = context;
        }

        #endregion

        #region Get Inspections List

        public List<Usp_B2B_GetInspections> GetInspectionsList(Models.B2B.AllInspectionRequest model)
        {
            var companyGuidParam = new SqlParameter { ParameterName = "CompanyGuid", SqlDbType = SqlDbType.VarChar, Value = model.CompanyGuid };

            var emailFilter = string.IsNullOrEmpty(model.Email)
                            ? (object)DBNull.Value
                            : model.Email;
            var emailParam = new SqlParameter { ParameterName = "Email", SqlDbType = SqlDbType.VarChar, Value = emailFilter };

            var externalRefFilter = string.IsNullOrEmpty(model.ExternalRef)
                                  ? (object)DBNull.Value
                                  : model.ExternalRef;
            var externalRefParam = new SqlParameter { ParameterName = "ExternalRef", SqlDbType = SqlDbType.VarChar, Value = externalRefFilter };

            var responses = dbContext.B2BInspectionsList.FromSqlRaw<Usp_B2B_GetInspections>
                           (
                              "Usp_B2B_GetInspections @CompanyGuid, @Email, @ExternalRef",
                              companyGuidParam, emailParam, externalRefParam
                           )
                           .ToList();

            return responses;
        }

        #endregion

        #region Get Inspection Details

        public Applications GetInspectionDetails(long inspectionId, string companyGuid, string externalRef = "")
        {
            // Get Inspection Details
            var response = (from a in dbContext.Applications
                            where ((inspectionId > 0 && a.Id == inspectionId) ||
                                    (externalRef.Length > 0 && a.ExternalRefNumber == externalRef)) &&
                                  a.BrokerCompanyGuid == companyGuid
                            select a).FirstOrDefault();

            return response;
        }

        public Applications GetInspectionDetailsById(long inspectionId)
        {
            // Get Inspection Details
            var response = (from a in dbContext.Applications
                            where a.Id == inspectionId
                            select a).FirstOrDefault();

            return response;
        }


        #endregion

        #region Get AppUsers

        public List<AppUsers> GetAppUsers(long inspectionId)
        {
            var responses = (from au in dbContext.AppUsers
                             where au.ApplicationId == inspectionId
                             select au).ToList();

            return responses;
        }

        #endregion

        #region Cancel Inspection

        public void CancelInspection(Applications application)
        {
            dbContext.Entry(application).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

        #endregion

        #region Update Inspection

        public void UpdateInspection(Applications application)
        {
            dbContext.Entry(application).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

        #endregion

        #region Update AppUsers

        public void UpdateAppUsers(AppUsers appUsers)
        {
            dbContext.Entry(appUsers).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

        #endregion

        #region Get State Detail By Code

        public long GetStateIdByCode(string stateCode)
        {
            var stateId = (from s in dbContext.ADStates
                           where s.StateCode == stateCode
                           select s.Id).FirstOrDefault();

            return stateId;
        }

        #endregion

        #region Get Users To Share

        public void GetUsersToShare(ref List<Models.B2B.Brokers> users, string companyGuid)
        {
            // Get Broker Users
            users.ForEach(
                user =>
                {
                    var userDetail = dbContext.ADUsers.FirstOrDefault(u => u.CompanyGuid == companyGuid && u.Email == user.Email);
                    if (userDetail != null)
                    {
                        user.UserGuid = userDetail.UserGuid;
                        user.UserId = userDetail.UserId;
                    }
                });
        }

        #endregion

        #region Validate B2BToken

        public B2BApiKeys ValidateB2BToken(string token)
        {
            B2BApiKeys response = null;

            var apiKeysDetails = token.Replace("amx ", "").Split(":");
            if (apiKeysDetails.Length == 2)
            {
                response = (from b2bApiKey in dbContext.B2BApiKeys
                            where b2bApiKey.APIKey == apiKeysDetails[0] &&
                                  b2bApiKey.APISecret == apiKeysDetails[1] &&
                                  b2bApiKey.IsDeleted == false
                            select b2bApiKey).FirstOrDefault();
            }

            return response;
        }

        #endregion

        #region Get User Details By Email

        public Entities.ADUsers GetUserDetailsByEmail(string email)
        {
            var response = dbContext.ADUsers.FirstOrDefault(u => u.Email == email);

            return response;
        }

        #endregion

        #region Add WebHook

        public void AddWebHook(B2BWebHooks b2bWebHook)
        {
            dbContext.B2BWebHooks.Add(b2bWebHook);
            dbContext.SaveChanges();
        }

        #endregion

        #region Delete WebHook

        public void DeleteWebHook(B2BWebHooks b2bWebHook)
        {
            dbContext.B2BWebHooks.Remove(b2bWebHook);
            dbContext.SaveChanges();
        }

        #endregion

        #region Delete All WebHooks

        public void DeleteAllWebHooks(List<B2BWebHooks> b2bWebHooks)
        {
            dbContext.B2BWebHooks.RemoveRange(b2bWebHooks);
            dbContext.SaveChanges();
        }

        #endregion

        #region Get WebHook By Id

        public B2BWebHooks GetWebHookById(long id)
        {
            var response = (from wh in dbContext.B2BWebHooks
                            where wh.Id == id
                            select wh).FirstOrDefault();

            return response;
        }

        #endregion

        #region Get All WebHooks By Company

        public List<B2BWebHooks> GetAllWebHooksByCompany(string companyGuid)
        {
            var responses = (from wh in dbContext.B2BWebHooks
                            where wh.CompanyGuid == companyGuid
                            select wh).ToList();

            return responses;
        }

        #endregion

        #region Save Db Changes

        public void SaveDbChanges()
        {
            dbContext.SaveChanges();
        }

        #endregion
    }
}