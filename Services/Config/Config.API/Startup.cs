using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Config.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            //Configuration = configuration;
            AppConfiguration = configuration;
        }

        //public IConfiguration Configuration { get; }

        public static IConfiguration AppConfiguration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(365);
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Config.API", Version = "v1" });
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            });

            services.AddCors(feature =>
                feature.AddPolicy(
                    "CorsPolicy",
                    apiPolicy => apiPolicy
                                    .AllowAnyHeader()
                                    .AllowAnyMethod()
                                    .SetIsOriginAllowed(host => true)
                                    .AllowCredentials()
                                ));

            services.AddDbContext<Entities.ConfigContext>(o => o.UseSqlServer(AppConfiguration.GetConnectionString("ConfigDB")), ServiceLifetime.Transient);

            services.AddScoped<Repositories.ICompanyRepository, Repositories.CompanyRepository>();
            services.AddScoped<Services.ICompanyService, Services.CompanyService>();

            services.AddScoped<Repositories.IAssetRepository, Repositories.AssetRepository>();
            services.AddScoped<Services.IAssetService, Services.AssetService>();

            services.AddScoped<Repositories.IInspectionRepository, Repositories.InspectionRepository>();
            services.AddScoped<Services.IInspectionService, Services.InspectionService>();

            services.AddScoped<Repositories.ILenderRepository, Repositories.LenderRepository>();
            services.AddScoped<Services.ILenderService, Services.LenderService>();

            services.AddScoped<Repositories.IErrorRepository, Repositories.ErrorRepository>();
            services.AddScoped<Services.IErrorService, Services.ErrorService>();

            services.AddScoped<Repositories.IDataRepository, Repositories.DataRepository>();
            services.AddScoped<Services.IDataService, Services.DataService>();

            services.AddScoped<Services.B2B.IB2BService, Services.B2B.B2BService>();
            services.AddScoped<Repositories.B2B.IB2BRepository, Repositories.B2B.B2BRepository>();

            services.AddScoped<Common.EventMessages.IMessageSender, Common.EventMessages.AzureBus.AzureBusMessageSender>();
            services.AddSingleton<Common.EventMessages.IMessageReceiver, Common.EventMessages.AzureBus.AzureBusMessageReceiver>();

            // needed to load configuration from appsettings.json
            services.AddOptions();

            // needed to store rate limit counters and ip rules
            services.AddMemoryCache();

            //load general configuration from appsettings.json
            services.Configure<IpRateLimitOptions>(AppConfiguration.GetSection("IpRateLimiting"));

            // inject counter and rules stores
            services.AddInMemoryRateLimiting();
            //services.AddDistributedRateLimiting<AsyncKeyLockProcessingStrategy>();
            //services.AddDistributedRateLimiting<RedisProcessingStrategy>();
            //services.AddRedisRateLimiting();

            // Add framework services.
            services.AddMvc();

            // https://github.com/aspnet/Hosting/issues/793
            // the IHttpContextAccessor service is not registered by default.
            // the clientId/clientIp resolvers use it.
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // configuration (resolvers, counter key builders)
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            // inject counter and rules distributed cache stores
            services.AddSingleton<IIpPolicyStore, DistributedCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, DistributedCacheRateLimitCounterStore>();

            // Disble automatic 400 bad request response
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger(c =>
                {
                    c.RouteTemplate = "/swagger/{documentName}/swagger.json";
                });
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Config.API v1"));
            }

            app.UseHsts();

            // Automatic Migration for Database
            UpdateDatabase(app);

            // Load Default Welcome Page
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseWelcomePage("/wwwroot/index.html");

            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Xss-Protection", "1");
                context.Response.Headers.Add("X-Frame-Options", "DENY");
                context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                await next.Invoke();
            });

            app.UseCors("CorsPolicy");

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private static void UpdateDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<Entities.ConfigContext>())
                {
                    context.Database.Migrate();
                }
            }
        }
    }
}
