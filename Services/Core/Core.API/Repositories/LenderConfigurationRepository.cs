using Microsoft.EntityFrameworkCore;
using System.Linq;
using Core.API.Entities;

namespace Core.API.Repositories
{
    public class LenderConfigurationRepository : ILenderConfigurationRepository
    {
        private readonly CoreContext dbContext;

        #region Constructor

        public LenderConfigurationRepository(CoreContext context)
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

        #region Get Lender Details

        public Common.Models.Core.Entities.LenderConfigurations GetLenderDetails(string companyGuid)
        {
            // Get Lender Details
            var response = dbContext.LenderConfigurations.FirstOrDefault(l => l.LenderCompanyGuid == companyGuid);

            return response;
        }

        #endregion

        #region Save Lender Details

        public void SaveLenderDetails(Common.Models.Core.Entities.LenderConfigurations lenderConfiguration)
        {
            if (lenderConfiguration.Id == 0)
            {
                dbContext.LenderConfigurations.Add(lenderConfiguration);
            }
            else
            {
                dbContext.Entry(lenderConfiguration).State = EntityState.Modified;
            }

            dbContext.SaveChanges();
        }

        #endregion
    }
}
