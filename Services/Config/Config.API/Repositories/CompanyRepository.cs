using Config.API.Entities;
using Config.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Data;
using Config.API.Entities.SP;

namespace Config.API.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly ConfigContext dbContext;

        #region Constructor

        public CompanyRepository(ConfigContext context)
        {
            dbContext = context;
        }

        #endregion

        public void BeginTransaction()
        {
            dbContext.Database.BeginTransaction();
        }

        public void CommitTransaction()
        {
            dbContext.Database.CommitTransaction();
        }

        public void RollbackTransaction()
        {
            dbContext.Database.RollbackTransaction();
        }

        #region Get Broker Companies

        public List<Companies> GetBrokerCompanies()
        {
            // Get Broker Companies
            var responses = dbContext.Companies
                            .Where(c => c.CompanyTypeId == (int)Enums.CompanyTypes.Broker)
                            .ToList();

            return responses;
        }

        #endregion

        #region Get Companies List

        public List<CompaniesListResponse> GetCompaniesList()
        {
            var responses = (from c in dbContext.Companies
                             where c.CompanyTypeId == (int)Enums.CompanyTypes.Broker
                             join u in dbContext.ADUsers
                             on c.CompanyGuid equals u.CompanyGuid
                             where u.IsPrimary == true
                             orderby c.Id descending
                             select new CompaniesListResponse()
                             {
                                 CompanyGuid = c.CompanyGuid,
                                 CompanyId = c.Id,
                                 CompanyName = c.CompanyName,
                                 ABN = c.ABN,
                                 UsersCount = dbContext.ADUsers.Count(u => u.CompanyGuid == c.CompanyGuid),
                                 LiveStatus = "Yes"
                             }).ToList();

            var primaryUsers = dbContext.ADUsers.Where(a => responses.Select(c => c.CompanyGuid).Contains(a.CompanyGuid) &&
                                                            a.IsPrimary == true).ToList();
            responses.ForEach(
                company =>
                {
                    var primaryUser = primaryUsers.FirstOrDefault(u => u.CompanyGuid == company.CompanyGuid);
                    if (primaryUser != null)
                    {
                        company.PrimaryContactName = string.Join(" ", primaryUser.Name, primaryUser.SurName);
                        company.PrimaryContactEmail = primaryUser.Email;
                    }
                    else
                    {
                        company.PrimaryContactName = "";
                        company.PrimaryContactEmail = "";
                    }
                });

            return responses;
        }

        #endregion

        #region Get All Brokers

        public List<Models.AllCompaniesResponse> GetAllBrokers()
        {
            // Get All Brokers
            var responses = (from c in dbContext.Companies
                             where c.CompanyTypeId == (long)Enums.CompanyTypes.Broker
                             select new Models.AllCompaniesResponse()
                             {
                                 CompanyGuid = c.CompanyGuid,
                                 CompanyName = c.CompanyName
                             }).ToList();

            return responses;
        }

        #endregion

        #region Save Company Details

        public void SaveCompanyDetails(Entities.Companies company)
        {
            dbContext.Companies.Add(company);
            dbContext.SaveChanges();
        }

        #endregion

        #region Update Company Details

        public void UpdateCompanyDetails(Entities.Companies companyDetails)
        {
            dbContext.Entry(companyDetails).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

        public void UpdateCompanyDatas(Entities.Companies companyDetails)
        {
            dbContext.Entry(companyDetails).State = EntityState.Modified;
        }

        #endregion

        #region Get Lender Companies

        public List<Entities.Companies> GetLenderCompanies()
        {
            // Get Lender Companies
            var responses = dbContext.Companies
                            .Where(c => c.CompanyTypeId == (int)Enums.CompanyTypes.Lender)
                            .OrderBy(c => c.CompanyName)
                            .ToList();

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

        #region Get Company By Name

        public Companies GetCompanyByName(string name, string companyGuid)
        {
            // Get Company By Name
            Entities.Companies response = null;
            if (string.IsNullOrEmpty(companyGuid))
            {
                response = dbContext.Companies.FirstOrDefault(c => c.CompanyName == name);
            }
            else
            {
                response = dbContext.Companies.FirstOrDefault(c => c.CompanyName == name &&
                                                                   c.CompanyGuid != companyGuid);
            }

            return response;
        }

        #endregion

        #region Get Lenders List

        public List<Usp_GetLendersList> GetLendersList()
        {
            var param = new object[0];
            var responses = dbContext.LendersList.FromSqlRaw<Usp_GetLendersList>
                            (
                               "Usp_GetLendersList",
                               param
                            )
                            .ToList();

            return responses;
        }

        #endregion

        #region Get Lender By Name

        public Companies GetLenderByName(string lenderName, string lenderCompanyGuid)
        {
            // Get Lender By Name
            Companies response = null;
            if (string.IsNullOrEmpty(lenderCompanyGuid))
            {
                response = dbContext.Companies.FirstOrDefault(c => c.CompanyTypeId == (int)Enums.CompanyTypes.Lender &&
                                                                   c.CompanyName == lenderName);
            }
            else
            {
                response = dbContext.Companies.FirstOrDefault(c => c.CompanyTypeId == (int)Enums.CompanyTypes.Lender &&
                                                                   c.CompanyName == lenderName &&
                                                                   c.CompanyGuid != lenderCompanyGuid);
            }

            return response;
        }

        #endregion

        #region Get TemplateSets

        public List<TemplateSets> GetTemplateSets()
        {
            // Get TemplateSets
            var responses = dbContext.TemplateSets
                            .Where(t => t.IsActive == true)
                            .OrderBy(t => t.Name)
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

        #region Add BrokerTemplateMappings

        public void AddBrokerTemplateMappings(long companyId, long templateId)
        {
            // Add BrokerTemplateMappings
            var mapping = new BrokerTemplateMappings();
            mapping.CompanyId = companyId;
            mapping.TemplateId = templateId;
            dbContext.BrokerTemplateMappings.Add(mapping);
        }

        #endregion

        #region Remove BrokerTemplateMappings

        public void RemoveBrokerTemplateMappings(long companyId)
        {
            // Remove BrokerTemplateMappings
            var responses = dbContext.BrokerTemplateMappings
                .Where(tm => tm.CompanyId == companyId)
                .ToList();
            dbContext.BrokerTemplateMappings.RemoveRange(responses);
        }

        #endregion

        #region Get BrokerLenderMappings

        public List<BrokerLenderMappings> GetBrokerLenderMappings(long companyId)
        {
            // Get BrokerLenderMappings
            var responses = dbContext.BrokerLenderMappings
                            .Where(lender => lender.BrokerCompanyId == companyId)
                            .ToList();

            return responses;
        }

        public List<BrokerLenderMappings> GetBrokerLenderMappingsByLenderCompanyId(long lenderCompanyId)
        {
            // Get BrokerLenderMappings
            var responses = dbContext.BrokerLenderMappings
                .Where(lender => lender.LenderCompanyId == lenderCompanyId)
                .ToList();

            return responses;
        }

        #endregion

        #region Add BrokerLenderMappings

        public void AddBrokerLenderMappings(long brokerCompanyId, long lenderCompanyId)
        {
            // Add BrokerLenderMappings
            var mapping = new BrokerLenderMappings();
            mapping.BrokerCompanyId = brokerCompanyId;
            mapping.LenderCompanyId = lenderCompanyId;
            dbContext.BrokerLenderMappings.Add(mapping);
        }

        #endregion

        #region Remove BrokerLenderMappings

        public void RemoveBrokerLenderMappings(long brokerCompanyId)
        {
            // Remove BrokerLenderMappings
            var responses = dbContext.BrokerLenderMappings
                            .Where(lm => lm.BrokerCompanyId == brokerCompanyId)
                            .ToList();
            dbContext.BrokerLenderMappings.RemoveRange(responses);
        }

        public void RemoveMappedLenders(long lenderCompanyId)
        {
            // Remove BrokerLenderMappings
            var responses = dbContext.BrokerLenderMappings
                            .Where(lm => lm.LenderCompanyId == lenderCompanyId)
                            .ToList();
            dbContext.BrokerLenderMappings.RemoveRange(responses);
        }

        #endregion

        #region Get LenderTemplateMappings

        public List<LenderTemplateMappings> GetLenderTemplateMappings(long companyId)
        {
            // Get LenderTemplateMappings
            var responses = dbContext.LenderTemplateMappings
                            .Where(template => template.CompanyId == companyId)
                            .ToList();

            return responses;
        }

        #endregion

        #region Add LenderTemplateMappings

        public void AddLenderTemplateMappings(long companyId, long templateId)
        {
            // Add LenderTemplateMappings
            var mapping = new LenderTemplateMappings();
            mapping.CompanyId = companyId;
            mapping.TemplateId = templateId;
            dbContext.LenderTemplateMappings.Add(mapping);
        }

        #endregion

        #region Remove LenderTemplateMappings

        public void RemoveLenderTemplateMappings(long companyId)
        {
            // Remove LenderTemplateMappings
            var responses = dbContext.LenderTemplateMappings
                .Where(tm => tm.CompanyId == companyId)
                .ToList();
            dbContext.LenderTemplateMappings.RemoveRange(responses);
        }

        #endregion

        #region Get Mapped TemplateSets

        public List<long> GetMappedTemplateSets(List<string> assetsWorkWith)
        {
            // Get Mapped TemplateSets
            var responses = dbContext.TemplateSets
                            .Where(t => assetsWorkWith.Contains(t.TemplateSetGuid))
                            .Select(t => t.Id)
                            .ToList();

            return responses;
        }

        #endregion

        #region Get Mapped Lenders

        public List<long> GetMappedLenders(List<string> lendersWorkWith)
        {
            // Get Mapped Lenders
            var responses = dbContext.Companies
                            .Where(c => lendersWorkWith.Contains(c.CompanyGuid))
                            .Select(c => c.Id)
                            .ToList();

            return responses;
        }

        #endregion

        #region Get CompanyContacts

        public List<Models.CompanyContacts> GetCompanyContacts(long companyId)
        {
            var responses = (from cct in dbContext.CompanyContactTypes
                             join cc in dbContext.CompanyContacts
                             .Where(c => c.CompanyId == companyId)
                             on cct.Id equals cc.CompanyContactTypeId
                             into CompanyContacts
                             from c in CompanyContacts.DefaultIfEmpty()
                             select new Models.CompanyContacts()
                             {
                                 Name = c.Name,
                                 SurName = c.SurName,
                                 Email = c.Email,
                                 Mobile = c.Mobile,
                                 CompanyContactTypeId = c != null
                                                      ? c.CompanyContactTypeId
                                                      : cct.Id
                                 //Name = c != null
                                 //     ? c.Name
                                 //     : "",
                                 //SurName = c != null
                                 //        ? c.SurName
                                 //        : "",
                                 //Email = c != null
                                 //      ? c.Email
                                 //      : "",
                                 //Mobile = c != null
                                 //       ? c.Mobile
                                 //       : "",
                                 //CompanyContactTypeId = c != null
                                 //                     ? c.CompanyContactTypeId
                                 //                     : cct.Id
                             })
                             .ToList();

            return responses;
        }

        #endregion

        #region Add CompanyContacts

        public void AddCompanyContacts(long companyId, Models.CompanyContacts contact)
        {
            // Add CompanyContacts
            dbContext.CompanyContacts.Add(
                new Entities.CompanyContacts()
                {
                    CompanyId = companyId,
                    Name = contact.Name,
                    SurName = contact.SurName,
                    Email = contact.Email,
                    Mobile = contact.Mobile,
                    CompanyContactTypeId = contact.CompanyContactTypeId
                });
        }

        #endregion

        #region Remove CompanyContacts

        public void RemoveCompanyContacts(long companyId)
        {
            // Remove CompanyContacts
            var responses = dbContext.CompanyContacts
                            .Where(c => c.CompanyId == companyId)
                            .ToList();
            dbContext.CompanyContacts.RemoveRange(responses);
        }

        #endregion

        #region Save DbChanges

        public void SaveDbChanges()
        {
            dbContext.SaveChanges();
        }

        #endregion

        #region Get Companies By Domain

        public List<Companies> GetCompaniesByDomain(string domain)
        {
            // Get Broker Companies
            var companiesList = (from c in dbContext.Companies
                                 where c.CompanyTypeId == (int)Enums.CompanyTypes.Broker
                                 select c).ToList();

            // Get Companies By Domain
            var responses = (from c in companiesList
                             where c.Email.Contains(domain)
                             select c).ToList();

            return responses;
        }

        #endregion

        #region Get All Assets

        public List<AssetsWorkWithResponse> GetAllAssets(bool isSupportTeam = false)
        {
            var responses = new List<AssetsWorkWithResponse>();

            // Get All Active Assets List
            var templatesList = (from ts in dbContext.TemplateSets
                                 where ts.IsActive == true
                                 orderby ts.Name
                                 select ts).ToList();

            // If UserRole is SupportTeam
            if (isSupportTeam)
            {
                // Adding Default Item
                responses.Add(
                    new AssetsWorkWithResponse()
                    {
                        TemplateName = "All",
                        TemplateSetGUID = "-1",
                        TemplateSetId = -1
                    });
            }

            // Adding Existing Assets
            templatesList.ForEach(
                ts =>
                {
                    responses.Add(
                        new AssetsWorkWithResponse()
                        {
                            TemplateName = ts.Name,
                            TemplateSetGUID = ts.TemplateSetGuid,
                            TemplateSetId = ts.Id
                        });
                });

            return responses;
        }

        #endregion

        #region Get All Lenders

        public List<LendersWorkWithResponse> GetAllLenders(bool isSupportTeam, bool isAddEditCompany)
        {
            var responses = new List<LendersWorkWithResponse>();

            // Get All Lenders List
            var lendersList = (from c in dbContext.Companies
                               where c.CompanyTypeId == (long)Enums.CompanyTypes.Lender
                               orderby c.CompanyName
                               select c).ToList();

            // If not added or edited Company
            if (!isAddEditCompany)
            {
                // If UserRole is SupportTeam
                if (isSupportTeam)
                {
                    // Add Default Item
                    responses.Add(
                        new LendersWorkWithResponse()
                        {
                            LenderName = "All",
                            LenderGUID = "-1",
                            IsPayer = false
                        });
                }
                else
                {
                    lendersList = lendersList.Where(l => l.LenderVisibility != (int)Models.Enums.LenderVisibilities.Self).ToList();
                }
            }
            else
            {
                lendersList = lendersList.Where(l => l.LenderVisibility != (int)Models.Enums.LenderVisibilities.Self).ToList();
            }

            // Adding Existing Lenders
            lendersList.ForEach(
                lender =>
                {
                    responses.Add(
                        new LendersWorkWithResponse()
                        {
                            LenderName = lender.CompanyName,
                            LenderGUID = lender.CompanyGuid,
                            IsPayer = lender.IsPayer
                        });
                });

            return responses;
        }

        #endregion

        #region Get State Option

        public List<StateOptionsResponse> GetStateoption()
        {
            // Get State Option
            var responses = (from st in dbContext.States
                             orderby st.StateCode
                             select new StateOptionsResponse()
                             {
                                 StateID = st.Id,
                                 StateCode = st.StateCode
                             }).ToList();

            return responses;
        }

        #endregion

        #region Get Invalid Users Email

        public string GetInvalidUsersEmail(List<string> usersList, long userTypeId)
        {
            var response = string.Empty;

            // Get Users List From DB
            var users = (from u in dbContext.ADUsers
                         where u.UserTypeId > 0 &&
                               u.UserTypeId != userTypeId
                         select u.Email).ToList();

            var result = users.Intersect(usersList);
            if (result != null &&
                result.Count() > 0)
            {
                response = string.Join(" , ", result);
            }

            return response;
        }

        #endregion

        #region Get Invalid AssetsWorkWith

        public string GetInvalidAssetsWorkWith(List<string> assetsWorkWith)
        {
            var response = string.Empty;

            // Get Template List From DB
            var templateSets = (from t in dbContext.TemplateSets
                                where t.IsActive == true
                                select t).ToList();

            // Check Asset
            List<string> invalidAssetsToUpdate = new List<string>();
            assetsWorkWith.ForEach(
                asset =>
                {
                    if (templateSets.Count(t => t.TemplateSetGuid == asset) == 0)
                    {
                        invalidAssetsToUpdate.Add(asset);
                    }
                });

            if (invalidAssetsToUpdate != null &&
                invalidAssetsToUpdate.Count() > 0)
            {
                response = string.Join(" , ", invalidAssetsToUpdate);
            }

            return response;
        }

        #endregion

        #region Get Invalid LendersWorkWith

        public string GetInvalidLendersWorkWith(List<string> lendersWorkWith)
        {
            var response = string.Empty;

            // Get Lenders List From DB
            var lenders = (from c in dbContext.Companies
                           where c.CompanyTypeId == (int)Enums.CompanyTypes.Lender
                           select c).ToList();

            // Check Lender
            List<string> invalidLendersToUpdate = new List<string>();
            lendersWorkWith.ForEach(
                lender =>
                {
                    if (lenders.Count(l => l.CompanyGuid == lender) == 0)
                    {
                        invalidLendersToUpdate.Add(lender);
                    }
                });

            if (invalidLendersToUpdate != null &&
                invalidLendersToUpdate.Count() > 0)
            {
                response = string.Join(" , ", invalidLendersToUpdate);
            }

            return response;
        }

        #endregion

        #region Get User Details By Email

        public ADUsers GetUserDetailsByEmail(string email)
        {
            var response = (from u in dbContext.ADUsers
                            where u.Email == email
                            select u
                           ).FirstOrDefault();

            return response;
        }

        #endregion

        #region Get User Details By CompanyGuid

        public List<ADUsers> GetUserDetailsByCompanyGuid(string companyGuid)
        {
            var response = (from u in dbContext.ADUsers
                            where u.CompanyGuid == companyGuid
                            select u
                           ).ToList();

            return response;
        }

        #endregion

        #region Get Invalid Company Contacts

        public string GetInvalidCompanyContactTypes(List<Models.CompanyContacts> companyContacts)
        {
            var response = string.Empty;

            // Get Company Contact Types List From DB
            var contactTypes = (from cct in dbContext.CompanyContactTypes
                                select cct
                               ).ToList();

            // Check Asset
            List<int> invalidContactsToUpdate = new List<int>();
            companyContacts.ForEach(
                contact =>
                {
                    if (contactTypes.Count(c => c.Id == contact.CompanyContactTypeId) == 0)
                    {
                        invalidContactsToUpdate.Add(contact.CompanyContactTypeId);
                    }
                });

            if (invalidContactsToUpdate != null &&
                invalidContactsToUpdate.Count() > 0)
            {
                response = string.Join(" , ", invalidContactsToUpdate);
            }

            return response;
        }

        #endregion

        #region Get Sta Collector Details

        public List<StaCollectorDetails> GetStaCollectorDetails()
        {
            var responses = (from c in dbContext.StaCollectorDetails
                             select c).ToList();

            return responses;
        }

        #endregion

        #region Get Lender Visibility Mappings

        public List<LenderVisibilityMappings> GetLenderVisibilityMappings(long companyId)
        {
            var responses = (from lvm in dbContext.LenderVisibilityMappings
                             where lvm.BrokerCompanyId == companyId
                             select lvm).ToList();

            return responses;
        }

        #endregion

        #region Update Sta Collector Details

        public void UpdateStaCollectorDetails(List<StaCollectorDetails> companyInfos)
        {
            dbContext.StaCollectorDetails.UpdateRange(companyInfos);
        }

        #endregion

        #region Save Changes

        public void SaveChanges()
        {
            dbContext.SaveChanges();
        }

        #endregion
    }
}
