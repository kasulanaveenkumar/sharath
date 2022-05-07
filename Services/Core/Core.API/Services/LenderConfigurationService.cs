using Common.Models.Core.Entities;
using Core.API.Repositories;

namespace Core.API.Services
{
    public class LenderConfigurationService : ILenderConfigurationService
    {
        public readonly ILenderConfigurationRepository lenderConfigurationRepository;
        public readonly IDataRepository dataRepository;

        #region Constructor

        public LenderConfigurationService(ILenderConfigurationRepository repository, IDataRepository dataRepository)
        {
            this.lenderConfigurationRepository = repository;
            this.dataRepository = dataRepository;
        }

        #endregion

        #region Get Company Details

        public ADCompanies GetCompanyDetails(string companyGuid)
        {
            var response = dataRepository.GetCompanyDetails(companyGuid);

            return response;
        }

        #endregion

        #region Get Lender Details

        public Models.LenderConfigurationResponse GetLenderDetails(string companyGuid)
        {
            Models.LenderConfigurationResponse response = null;

            // Get Lender Configuration Details by CompanyGuid
            var lenderConfigurationDetails = lenderConfigurationRepository.GetLenderDetails(companyGuid);
            if (lenderConfigurationDetails != null)
            {
                response = new Models.LenderConfigurationResponse();

                // Lender Reference
                response.LenderRefPrefix = lenderConfigurationDetails.LenderRefPrefix;

                // Legal details
                // Custom Terms and Conditions
                response.AdditionalTnC = lenderConfigurationDetails.AdditionalTnC;

                // Visibility
                // Auto Lodgement of Reports
                response.IsReportRequired = lenderConfigurationDetails.IsReportRequired;
                // Email Address for Lodgement of Reports
                response.ReportEmailAddress = lenderConfigurationDetails.ReportEmailAddress;

                // Permissions
                response.IsBSAllowed = lenderConfigurationDetails.IsBSAllowed;
                response.IsNonOwnerAllowed = lenderConfigurationDetails.IsNonOwnerAllowed;

                // Integration
                response.IsIllionIntegrationEnabled = lenderConfigurationDetails.IsIllionIntegrationEnabled;
                response.IsAPIIntegrationEnabled = lenderConfigurationDetails.IsAPIIntegrationEnabled;

                // Awaited Reference
                response.IsAllowAwaitedRef = lenderConfigurationDetails.IsAllowAwaitedRef;
            }

            return response;
        }

        #endregion

        #region Save Lender Details

        public void SaveLenderDetails(Models.LenderConfigurationRequest model)
        {
            // Get Lender Configuration Details by CompanyGuid
            var lenderConfigurationDetails = lenderConfigurationRepository.GetLenderDetails(model.LenderCompanyGuid);

            var lenderConfiguration = lenderConfigurationDetails == null
                                    ? new Common.Models.Core.Entities.LenderConfigurations()
                                    : lenderConfigurationDetails;

            lenderConfiguration.LenderCompanyGuid = model.LenderCompanyGuid;

            // Lender Reference
            lenderConfiguration.LenderRefPrefix = model.LenderRefPrefix;

            // Legal details
            // Custom Terms and Conditions
            lenderConfiguration.AdditionalTnC = model.AdditionalTnC;

            // Visibility
            // Auto Lodgement of Reports
            lenderConfiguration.IsReportRequired = model.IsReportRequired;

            // Email Address for Lodgement of Reports
            lenderConfiguration.ReportEmailAddress = model.ReportEmailAddress;

            // Integration
            lenderConfiguration.IsIllionIntegrationEnabled = model.IsIllionIntegrationEnabled;
            lenderConfiguration.IsAPIIntegrationEnabled = model.IsAPIIntegrationEnabled;

            // Permissions
            lenderConfiguration.IsBSAllowed = model.IsBSAllowed;
            lenderConfiguration.IsNonOwnerAllowed = model.IsNonOwnerAllowed;

            // Awaited Reference
            lenderConfiguration.IsAllowAwaitedRef = model.IsAllowAwaitedRef;

            lenderConfigurationRepository.SaveLenderDetails(lenderConfiguration);
        }

        #endregion
    }
}
