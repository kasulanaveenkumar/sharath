using Common.Models.Core.Entities;

namespace Core.API.Repositories
{
    public interface ILenderConfigurationRepository
    {
        public void BeginTransaction();

        public void CommitTransaction();

        public void RollbackTransaction();

        public LenderConfigurations GetLenderDetails(string companyGuid);

        public void SaveLenderDetails(LenderConfigurations lenderConfiguration);
    }
}
