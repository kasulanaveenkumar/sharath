using Common.Models.Core.Entities;
using Core.API.Entities;
using Core.API.Models.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Core.API.Repositories
{
    public class DataRepository : IDataRepository
    {
        private readonly CoreContext dbContext;

        #region Constructor

        public DataRepository(CoreContext context)
        {
            dbContext = context;
        }

        #endregion

        #region Save Company Details

        public void SaveCompanyDetails(SaveCompanyDetailsRequest model)
        {
            if (model != null)
            {
                var companyDetails = dbContext.ADCompanies.FirstOrDefault(c => c.CompanyGuid == model.CompanyGuid);
                if (companyDetails == null)
                {
                    dbContext.ADCompanies.Add(
                        new ADCompanies()
                        {
                            CompanyGuid = model.CompanyGuid,
                            CompanyName = model.CompanyName,
                            CompanyTypeId = model.CompanyTypeId,
                            IsPayer = model.IsPayer,
                            ExcemptPayment = model.ExcemptPayment
                        });
                }
                else
                {
                    companyDetails.CompanyGuid = model.CompanyGuid;
                    companyDetails.CompanyName = model.CompanyName;
                    companyDetails.CompanyTypeId = model.CompanyTypeId;
                    companyDetails.IsPayer = model.IsPayer;
                    companyDetails.ExcemptPayment = model.ExcemptPayment;
                }

                dbContext.SaveChanges();
            }
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
                                        new Entities.ADUsers()
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

        #region Get User Details

        public Entities.ADUsers GetUserDetailsByUserGuid(string userGuid)
        {
            var response = dbContext.ADUsers.FirstOrDefault(c => c.UserGuid == userGuid);

            return response;
        }

        public List<Entities.ADUsers> GetUsersDetailByUserGuid(List<string> userGuids)
        {
            var responses = (from u in dbContext.ADUsers
                             where userGuids.Contains(u.UserGuid)
                             select u).ToList();

            return responses;
        }

        public Entities.ADUsers GetUserDetailsByUserId(long userId)
        {
            var response = dbContext.ADUsers.FirstOrDefault(c => c.UserId == userId);

            return response;
        }

        #endregion

        #region Get Company Details

        public ADCompanies GetCompanyDetails(string companyGuid)
        {
            var response = dbContext.ADCompanies.FirstOrDefault(c => c.CompanyGuid == companyGuid);

            return response;
        }

        #endregion
    }
}
