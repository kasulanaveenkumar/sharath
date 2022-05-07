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
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Core.API.Swagger.B2B;

namespace Core.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            AppConfiguration = configuration;
        }

        public static IConfiguration AppConfiguration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore().AddRazorViewEngine();

            services.AddControllers();

            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(365);
            });

            services.AddSwaggerGen(
                c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = "Verimoto B2B API",
                        Description = "FireFlash version of B2B API",
                        TermsOfService = new Uri("https://lakeba.com/terms-of-use-and-privacy-policy/"),
                        Version = "v1"
                    });
                    c.EnableAnnotations();
                    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                    var filePath = System.IO.Path.Combine(System.AppContext.BaseDirectory, "Core.API.xml");
                    c.IncludeXmlComments(filePath);
                    c.OperationFilter<CustomHeaderAttribute>();
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

            services.AddDbContext<Entities.CoreContext>(o => o.UseSqlServer(AppConfiguration.GetConnectionString("CoreDB")));

            services.AddScoped<Services.IInspectionService, Services.InspectionService>();
            services.AddScoped<Repositories.IInspectionRepository, Repositories.InspectionRepository>();

            services.AddScoped<Services.INotificationsService, Services.NotificationsService>();
            services.AddScoped<Repositories.INotificationsRepository, Repositories.NotificationsRepository>();

            services.AddScoped<Services.ILenderConfigurationService, Services.LenderConfigurationService>();
            services.AddScoped<Repositories.ILenderConfigurationRepository, Repositories.LenderConfigurationRepository>();

            services.AddScoped<Services.IAdminService, Services.AdminService>();
            services.AddScoped<Repositories.IAdminRepository, Repositories.AdminRepository>();

            services.AddScoped<Services.ILenderService, Services.LenderService>();
            services.AddScoped<Repositories.ILenderRepository, Repositories.LenderRepository>();

            services.AddScoped<Services.IErrorService, Services.ErrorService>();
            services.AddScoped<Repositories.IErrorRepository, Repositories.ErrorRepository>();

            services.AddScoped<Services.IDataService, Services.DataService>();
            services.AddScoped<Repositories.IDataRepository, Repositories.DataRepository>();

            services.AddScoped<Services.B2B.IB2BService, Services.B2B.B2BService>();
            services.AddScoped<Repositories.B2B.IB2BRepository, Repositories.B2B.B2BRepository>();

            services.AddScoped<Common.EventMessages.IMessageSender, Common.EventMessages.AzureBus.AzureBusMessageSender>();
            services.AddSingleton<Common.EventMessages.IMessageReceiver, Common.EventMessages.AzureBus.AzureBusMessageReceiver>();

            services.AddScoped<Helper.IViewRenderService, Helper.ViewRenderService>();

            services.AddScoped<AuthorizeAttribute>();

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
            }

            app.UseSwagger(
                c =>
                {
                    c.RouteTemplate = "/swagger/{documentName}/swagger.json";
                });
            app.UseSwaggerUI(
                c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Core.API v1");
                    c.DefaultModelsExpandDepth(-1); // Remove Schemas at Bottom in Swagger UI
                });

            app.UseHsts();

            // Automatic Migration for Database
            UpdateDatabase(app);

            // Load Default Welcome Page
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseWelcomePage("/wwwroot/index.html");

            //Install the dependencies packages for HTML to PDF conversion in Linux
            //string shellFilePath = System.IO.Path.Combine(env.ContentRootPath, "dependenciesInstall.sh");
            //InstallDependecies(shellFilePath);

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

        private void InstallDependecies(string shellFilePath)
        {
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = "-c " + shellFilePath,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                }
            };
            process.Start();
            process.WaitForExit();
        }

        private static void UpdateDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<Entities.CoreContext>())
                {
                    context.Database.Migrate();
                }
            }
        }
    }
}
