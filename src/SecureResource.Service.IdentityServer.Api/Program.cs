using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IdentityServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddDebug();
                    //logging.AddAzureWebAppDiagnostics();
                })
            .ConfigureAppConfiguration((context, config) =>
            {
                if (!context.HostingEnvironment.IsDevelopment())
                {
                    //config.ConfigureProductionKeyVault();
                }
                else
                {
                    //config.ConfigureDevelopmentKeyVault();
                }
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                string dsn = string.Empty;
                if (webBuilder.GetSetting("Environment") == Environments.Production)
                {
                    dsn = "https://d63100a0921f4ee1b1890a1da4f15cc8@o1295482.ingest.sentry.io/6521122";
                }
                //webBuilder.UseSentry(o =>
                //{
                //    o.Dsn = dsn;
                //    // When configuring for the first time, to see what the SDK is doing:
                //    o.Debug = true;
                //    // Set TracesSampleRate to 1.0 to capture 100% of transactions for performance monitoring.
                //    // We recommend adjusting this value in production.
                //    o.TracesSampleRate = 1.0;
                //});

                webBuilder.UseStartup<Startup>();
            });
    }
}
