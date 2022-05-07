using Common.Models.Core.Entities;
using Core.API.Entities.SP;
using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient.AlwaysEncrypted.AzureKeyVaultProvider;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Core.API.Entities
{
    public class CoreContext : DbContext
    {
        private static Boolean isInitialized;
        private static ClientCredential _clientCredential;
        private readonly string _clientSecret;
        private readonly string _clientId;

        public CoreContext(DbContextOptions<CoreContext> options) : base(options)
        {
            if (!isInitialized)
            {
                InitializeAzureKeyVaultProvider();
                isInitialized = true;
            }

            Database.SetCommandTimeout(120);

            // Getting EncryptionSettings Values
            _clientId = Startup.AppConfiguration.GetSection("EncryptionSettings").GetSection("ClientId").Value;
            _clientSecret = Startup.AppConfiguration.GetSection("EncryptionSettings").GetSection("ClientSecretKey").Value;

            _clientCredential = new ClientCredential(_clientId, _clientSecret);
        }

        public DbSet<Applications> Applications { get; set; }
        public DbSet<AppDocuments> AppDocuments { get; set; }
        public DbSet<AppImages> AppImages { get; set; }
        public DbSet<AppImageReasons> AppImageReasons { get; set; }
        public DbSet<AppStakeholders> AppStakeholders { get; set; }
        public DbSet<AppUsers> AppUsers { get; set; }
        public DbSet<LenderConfigurations> LenderConfigurations { get; set; }
        public DbSet<ReasonMappings> ReasonMappings { get; set; }
        public DbSet<Reasons> Reasons { get; set; }

        public DbSet<AppActivities> AppActivities { get; set; }
        public DbSet<AppActivityLogs> AppActivityLogs { get; set; }

        public DbSet<NotificationEvents> NotificationEvents { get; set; }
        public DbSet<Notifications> Notifications { get; set; }
        public DbSet<NotificationUserMappings> NotificationUserMappings { get; set; }

        public DbSet<Remainders> Remainders { get; set; }
        public DbSet<CoreConfigs> CoreConfigs { get; set; }

        public DbSet<PaymentLogs> PaymentLogs { get; set; }

        public DbSet<ErrorLogs> ErrorLogs { get; set; }

        public DbSet<TemplateImageHelpImages> TemplateImageHelpImages { get; set; }

        public DbSet<TemplateImageOverlayImages> TemplateImageOverlayImages { get; set; }

        public DbSet<DVSChecks> DVSChecks { get; set; }

        public DbSet<IllionIntegrationDetails> IllionIntegrationDetails { get; set; }

        public DbSet<ADCompanies> ADCompanies { get; set; }
        public DbSet<ADTemplateSets> ADTemplateSets { get; set; }
        public DbSet<ADUsers> ADUsers { get; set; }
        public DbSet<ADTemplateSetPlans> ADTemplateSetPlans { get; set; }
        public DbSet<ADUserTypes> ADUserTypes { get; set; }
        public DbSet<ADStates> ADStates { get; set; }
        public DbSet<ADTemplateSetLenderPlans> ADTemplateSetLenderPlans { get; set; }

        public DbSet<B2BApiKeys> B2BApiKeys { get; set; }
        public DbSet<B2BWebHooks> B2BWebHooks { get; set; }

        public virtual DbSet<Usp_GetInspections> InspectionsList { get; set; }
        public virtual DbSet<Usp_GetCompaniesDashboardDatas> CompaniesDashboardDatas { get; set; }

        public virtual DbSet<Usp_B2B_GetInspections> B2BInspectionsList { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Usp_GetInspections>(entity =>
            {
                entity.HasNoKey();
            });

            builder.Entity<Usp_GetInspections>().Metadata.SetIsTableExcludedFromMigrations(true);

            builder.Entity<Usp_GetCompaniesDashboardDatas>(entity =>
            {
                entity.HasNoKey();
            });

            builder.Entity<Usp_GetCompaniesDashboardDatas>().Metadata.SetIsTableExcludedFromMigrations(true);

            builder.Entity<Usp_B2B_GetInspections>(entity =>
            {
                entity.HasNoKey();
            });

            builder.Entity<Usp_B2B_GetInspections>().Metadata.SetIsTableExcludedFromMigrations(true);
        }

        private static void InitializeAzureKeyVaultProvider()
        {
            SqlColumnEncryptionAzureKeyVaultProvider azureKeyVaultProvider =
              new SqlColumnEncryptionAzureKeyVaultProvider(GetToken);

            Dictionary<string, SqlColumnEncryptionKeyStoreProvider> providers =
              new Dictionary<string, SqlColumnEncryptionKeyStoreProvider>();

            providers.Add(SqlColumnEncryptionAzureKeyVaultProvider.ProviderName, azureKeyVaultProvider);
            SqlConnection.RegisterColumnEncryptionKeyStoreProviders(providers);
        }

        private async static Task<string> GetToken(string authority, string resource, string scope)
        {
            var authContext = new AuthenticationContext(authority);
            AuthenticationResult result = await authContext.AcquireTokenAsync(resource, _clientCredential);

            if (result == null)
                throw new InvalidOperationException("Failed to obtain the access token");
            return result.AccessToken;
        }
    }
}
