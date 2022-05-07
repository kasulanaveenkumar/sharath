using Common.Extensions;
using Config.API.Entities;
using Config.API.Models.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Config.API.Repositories
{
    public class DataRepository : IDataRepository
    {
        private readonly ConfigContext dbContext;

        #region Constructor

        public DataRepository(ConfigContext context)
        {
            dbContext = context;
        }

        #endregion

        #region Save Users

        public void SaveUsers(List<SaveUsersRequest> model)
        {
            if (model != null &&
                model.Count() > 0)
            {
                // Delete User
                var usersToBeDeleted = model.Where(u => u.IsDeleted == true).ToList();
                if (usersToBeDeleted != null &&
                    usersToBeDeleted.Count() > 0)
                {
                    usersToBeDeleted.ForEach(
                        userToDelete =>
                    {
                        var userDetail = dbContext.ADUsers.FirstOrDefault(u => u.UserGuid == userToDelete.UserGuid &&
                                                                               u.CompanyGuid == userToDelete.CompanyGuid);
                        if (userDetail != null)
                        {
                            dbContext.ADUsers.Remove(userDetail);
                        }
                    });
                }

                // Add User or
                // Update User
                var activeUsers = model.Where(u => u.IsDeleted == false).ToList();
                if (activeUsers != null &&
                    activeUsers.Count() > 0)
                {
                    activeUsers.ForEach(
                        user =>
                        {
                            if (!string.IsNullOrEmpty(user.CompanyGuid))
                            {
                                var userDetails = dbContext.ADUsers.FirstOrDefault(u => u.UserGuid == user.UserGuid &&
                                                                                        u.CompanyGuid == user.CompanyGuid);
                                if (userDetails == null)
                                {
                                    dbContext.ADUsers.Add(
                                        new ADUsers()
                                        {
                                            UserId = user.UserId,
                                            UserGuid = user.UserGuid,
                                            CompanyGuid = user.CompanyGuid,
                                            Name = user.Name,
                                            SurName = user.SurName,
                                            Mobile = user.Mobile,
                                            Email = user.Email,
                                            UserTypeId = user.UserTypeId,
                                            IsActive = user.IsActive,
                                            IsPrimary = user.IsPrimary
                                        });
                                }
                                else
                                {
                                    userDetails.UserId = user.UserId;
                                    userDetails.UserGuid = user.UserGuid;
                                    userDetails.Name = user.Name;
                                    userDetails.SurName = user.SurName;
                                    userDetails.Mobile = user.Mobile;
                                    userDetails.Email = user.Email;
                                    userDetails.UserTypeId = user.UserTypeId;
                                    userDetails.IsActive = user.IsActive;
                                    dbContext.Entry(userDetails).State = EntityState.Modified;
                                }
                            }
                            else
                            {
                                var userDetails = dbContext.ADUsers.Where(u => u.UserGuid == user.UserGuid).ToList();
                                if (userDetails != null &&
                                    userDetails.Count() > 0)
                                {
                                    userDetails.ForEach(
                                        userDetail =>
                                        {
                                            if (!string.IsNullOrEmpty(user.Name))
                                            {
                                                userDetail.Name = user.Name;
                                            }
                                            if (!string.IsNullOrEmpty(user.SurName))
                                            {
                                                userDetail.SurName = user.SurName;
                                            }
                                            if (!string.IsNullOrEmpty(user.Mobile))
                                            {
                                                userDetail.Mobile = user.Mobile;
                                            }
                                            if (!string.IsNullOrEmpty(user.Email))
                                            {
                                                userDetail.Email = user.Email;
                                            }
                                            userDetail.IsActive = user.IsActive;
                                            dbContext.Entry(userDetails).State = EntityState.Modified;
                                        });
                                }
                            }
                        });
                }

                dbContext.SaveChanges();
            }
        }

        #endregion

        #region Save CompanyDetails

        public void SaveCompanyDetails(SaveCompanyDetailsRequest model, string token)
        {
            if (model != null)
            {
                using (var client = new HttpClient())
                {
                    var requestUri = "api/v1/Data/savecompanydetails";
                    var coreApiUrl = Startup.AppConfiguration.GetSection("BaseURL").GetSection("CoreApiURL").Value;
                    var responseData = ExtensionMethods<SaveCompanyDetailsRequest>
                                       .PostJsonDatas(client, coreApiUrl, requestUri, token, model)
                                       .Result;
                }

                using (var client = new HttpClient())
                {
                    var requestUri = "api/v1/Data/savecompanydetails";
                    var userApiUrl = Startup.AppConfiguration.GetSection("BaseURL").GetSection("UserApiURL").Value;
                    var responseData = ExtensionMethods<SaveCompanyDetailsRequest>
                                       .PostJsonDatas(client, userApiUrl, requestUri, token, model)
                                       .Result;
                }
            }
        }

        #endregion

        #region Get User Details By Email

        public GetUserDetailsResponse GetUserDetailsByEmail(string email, string token)
        {
            GetUserDetailsResponse responseData = null;

            using (var client = new HttpClient())
            {
                var requestUri = "api/v1/Data/getuserdetails/" + email;
                var userApiUrl = Startup.AppConfiguration.GetSection("BaseURL").GetSection("UserApiURL").Value;
                responseData = ExtensionMethods<GetUserDetailsResponse>
                               .GetDeserializedData(client, userApiUrl, requestUri, token)
                               .Result;
            }

            return responseData;
        }

        #endregion
    }
}
