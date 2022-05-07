using Common.Extensions;
using Core.API.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Core.API.Repositories
{
    public class ErrorRepository : IErrorRepository
    {
        private readonly CoreContext dbContext;

        #region Constructor

        public ErrorRepository(CoreContext context)
        {
            dbContext = context;
        }

        #endregion

        #region Save Error

        public string SaveError(Exception ex, object requestData, string userGuid, string companyGuid,
                                [CallerMemberName] string member = "", [CallerFilePath] string filePath = "")
        {
            if (ex.GetType() == typeof(DbUpdateException))
            {
                var changedEntries = dbContext.ChangeTracker.Entries();
                foreach (var entry in changedEntries)
                {
                    switch (entry.State)
                    {
                        case EntityState.Modified:
                            entry.CurrentValues.SetValues(entry.OriginalValues);
                            entry.State = EntityState.Unchanged;
                            break;

                        case EntityState.Added:
                            entry.State = EntityState.Detached;
                            break;

                        case EntityState.Deleted:
                            entry.State = EntityState.Unchanged;
                            break;
                    }
                }
            }

            ErrorLogs errorLog = new ErrorLogs();
            var error = CustomExceptionHandler.Parse(ex);
            errorLog.Member = member;
            errorLog.FilePath = filePath;
            errorLog.Message = error.Message;
            errorLog.StackTrace = error.StackTrace;
            errorLog.CompleteException = error.CompleteException;
            errorLog.AdditionalDetails = error.AdditionalDetails;

            // Set Request Data
            if (requestData != null)
                errorLog.RequestData = JsonSerializer.Serialize(requestData);

            errorLog.UserGuid = userGuid;
            errorLog.CompanyGuid = companyGuid;
            errorLog.ErrorTime = error.ErrorTime;

            dbContext.ErrorLogs.Add(errorLog);
            dbContext.SaveChanges();

            return "CRE_" + errorLog.Id;
        }

        #endregion
    }
}
