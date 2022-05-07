using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.IO;
using System.Linq;
using Microsoft.Data.SqlClient.AlwaysEncrypted.AzureKeyVaultProvider;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Config.API.Entities
{
    public class ConfigContext : DbContext
    {
        private readonly string _clientSecret;
        private readonly string _clientId;
        private static Boolean isInitialized;
        private static ClientCredential _clientCredential;

        public ConfigContext(DbContextOptions<ConfigContext> options) : base(options)
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

        public DbSet<Companies> Companies { get; set; }
        public DbSet<CompanyTypes> CompanyTypes { get; set; }

        public DbSet<TemplateSets> TemplateSets { get; set; }
        public DbSet<BrokerLenderMappings> BrokerLenderMappings { get; set; }
        public DbSet<BrokerTemplateMappings> BrokerTemplateMappings { get; set; }
        public DbSet<States> States { get; set; }
        public DbSet<TemplateDocuments> TemplateDocuments { get; set; }
        public DbSet<TemplateImages> TemplateImages { get; set; }
        public DbSet<TemplateDocImageMappings> TemplateDocImageMappings { get; set; }
        public DbSet<TemplateSetDocMappings> TemplateSetDocMappings { get; set; }
        public DbSet<TemplateDocCategories> TemplateDocCategories { get; set; }

        public DbSet<CompanyContacts> CompanyContacts { get; set; }
        public DbSet<CompanyContactTypes> CompanyContactTypes { get; set; }

        public DbSet<TemplateDocOptions> TemplateDocOptions { get; set; }

        public DbSet<LenderTemplateSetDocMappings> LenderTemplateSetDocMappings { get; set; }
        public DbSet<LenderTemplateDocImageMappings> LenderTemplateDocImageMappings { get; set; }

        public DbSet<TemplateSetPlans> TemplateSetPlans { get; set; }
        public DbSet<TemplateSetPlanDocMappings> TemplateSetPlanDocMappings { get; set; }

        public DbSet<TemplateDocNoLenderPreferences> TemplateDocNoLenderPreferences { get; set; }

        public DbSet<LenderBlockedBrokerUsers> LenderBlockedBrokerUsers { get; set; }

        public DbSet<ErrorLogs> ErrorLogs { get; set; }

        public DbSet<TemplateSetPlanMappings> TemplateSetPlanMappings { get; set; }

        public DbSet<TemplateSetCustomPlanMappings> TemplateSetCustomPlanMappings { get; set; }

        public DbSet<TemplateSetCustomDocMappings> TemplateSetCustomDocMappings { get; set; }

        public DbSet<TemplateSetCustomImageMappings> TemplateSetCustomImageMappings { get; set; }

        public DbSet<LenderTemplateMappings> LenderTemplateMappings { get; set; }

        public DbSet<ADUsers> ADUsers { get; set; }

        public DbSet<StaCollectorDetails> StaCollectorDetails { get; set; }

        public DbSet<LenderVisibilityMappings> LenderVisibilityMappings { get; set; }

        public virtual DbSet<SP.Usp_GetInspectionPlanDetails> GetInspectionPlanDetails { get; set; }

        public virtual DbSet<SP.Usp_GetNewInspectionDocuments> GetNewInspectionDocuments { get; set; }

        public virtual DbSet<SP.Usp_GetLendersList> LendersList { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<SP.Usp_GetInspectionPlanDetails>(entity =>
            {
                entity.HasNoKey();
            });

            builder.Entity<SP.Usp_GetInspectionPlanDetails>().Metadata.SetIsTableExcludedFromMigrations(true);

            builder.Entity<SP.Usp_GetNewInspectionDocuments>(entity =>
            {
                entity.HasNoKey();
            });

            builder.Entity<SP.Usp_GetNewInspectionDocuments>().Metadata.SetIsTableExcludedFromMigrations(true);

            builder.Entity<SP.Usp_GetLendersList>(entity =>
            {
                entity.HasNoKey();
            });

            builder.Entity<SP.Usp_GetLendersList>().Metadata.SetIsTableExcludedFromMigrations(true);
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
