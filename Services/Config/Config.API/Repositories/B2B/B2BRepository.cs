using Config.API.Entities;
using Config.API.Entities.SP;
using Config.API.Models;
using Config.API.Models.Asset;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Config.API.Repositories.B2B
{
    public class B2BRepository : IB2BRepository
    {
        private readonly ConfigContext dbContext;

        #region Constructor

        public B2BRepository(ConfigContext context)
        {
            dbContext = context;
        }

        #endregion

        #region Get GetBrokerLenderMappings

        public List<BrokerLenderMappings> GetBrokerLenderMappings(long companyId)
        {
            // Get BrokerLenderMappings
            var responses = dbContext.BrokerLenderMappings
                            .Where(lender => lender.BrokerCompanyId == companyId)
                            .ToList();

            return responses;
        }

        #endregion

        #region Get Lender Companies

        public List<Companies> GetLenderCompanies()
        {
            // Get Lender Companies
            var responses = dbContext.Companies
                            .Where(c => c.CompanyTypeId == (int)Enums.CompanyTypes.Lender)
                            .ToList();

            return responses;
        }

        #endregion

        #region Get BrokerTemplateMappings

        public List<BrokerTemplateMappings> GetBrokerTemplateMappings(long companyId)
        {
            // Get BrokerTemplateMappings
            var responses = dbContext.BrokerTemplateMappings
                            .Where(template => template.CompanyId == companyId)
                            .ToList();

            return responses;
        }

        #endregion

        #region Get TemplateSets

        public List<TemplateSets> GetTemplateSets()
        {
            // Get TemplateSets
            var responses = dbContext.TemplateSets.Where(t => t.IsActive == true).ToList();

            return responses;
        }

        #endregion

        #region Get Company By Guid

        public Companies GetCompanyByGuid(string companyGuid)
        {
            // Get Company By Guid
            var response = dbContext.Companies.FirstOrDefault(c => c.CompanyGuid == companyGuid);

            return response;
        }

        #endregion

        #region Get States

        public List<StateOptionsResponse> GetStates()
        {
            // Get State Option
            var responses = (from st in dbContext.States
                             select new StateOptionsResponse()
                             {
                                 StateID = st.Id,
                                 StateCode = st.StateCode
                             }).ToList();

            return responses;
        }

        #endregion

        #region Get TemplateSet Detail By Guid

        public TemplateSets GetTemplateSetDetailByGuid(string templateSetGuid)
        {
            // Get TemplateSet Detail By Guid
            var response = (from t in dbContext.TemplateSets
                            where t.TemplateSetGuid == templateSetGuid
                            select t).FirstOrDefault();

            return response;
        }

        #endregion

        #region Get AssetDoc Details

        public List<Usp_GetNewInspectionDocuments> GetAssetDocDetails(AssetDocListRequest model, string userGuid, string companyGuid)
        {
            var param = new SqlParameter[]
            {
                new SqlParameter("LenderCompanyGuid", model.LenderGuid),
                new SqlParameter("TemplateSetGuid", model.TemplateGuid),
                new SqlParameter("PlanGuid", model.planGuid),
                new SqlParameter("StateId", model.StateId)
            };

            var response = dbContext.GetNewInspectionDocuments.FromSqlRaw<Usp_GetNewInspectionDocuments>
                           (
                             "usp_GetNewInspectionDocuments @LenderCompanyGuid, @TemplateSetGuid, @PlanGuid, @StateId",
                             param
                           )
                           .ToList();
            return response;
        }

        #endregion

        #region Get TemplateDocNoLenderPreference

        public TemplateDocNoLenderPreferences GetTemplateDocNoLenderPreference(long templateSetId, string userGuid, string companyGuid)
        {
            // Get TemplateDocNoLenderPreference
            var response = dbContext.TemplateDocNoLenderPreferences.FirstOrDefault(t => t.TemplateSetId == templateSetId &&
                                                                                        t.UserGuid == userGuid &&
                                                                                        t.CompanyGuid == t.CompanyGuid);

            return response;
        }

        #endregion
    }
}
