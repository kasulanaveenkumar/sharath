using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Config.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseKestrel((options) =>
                    {
                        // Do not add the Server HTTP header.
                        options.AddServerHeader = false;
                    });
                });
    }
}
