using Common.Models.Core.Entities;

namespace Core.API.Services
{
    public interface ILenderConfigurationService
    {
        public ADCompanies GetCompanyDetails(string companyGuid);

        public Models.LenderConfigurationResponse GetLenderDetails(string companyGuid);

        public void SaveLenderDetails(Models.LenderConfigurationRequest model);
    }
}
