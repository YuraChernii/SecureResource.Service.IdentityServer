using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Test;
using IdentityServerHost.Quickstart.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
//using SecureResource.Library;
using SecureResource.Service.IdentityServer.Infrastructure;
using SecureResource.Service.IdentityServer.Infrastructure.Services;
using SecureResource.Service.IdentityServer.Middlewares;

namespace IdentityServer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            //const string connectionString = @"Data Source=(LocalDb)\MSSQLLocalDB;database=IdentityServer4.Quickstart.EntityFramework-4.0.0;trusted_connection=yes;";

            //Class1 d = new Class1();

            string connStr = Configuration.GetConnectionString("SQLServerDB");

            services.AddInfrastructure();

            services.AddControllersWithViews();
            //Server=tcp:secure-resource.database.windows.net,1433;Initial Catalog=IdentityServer-database;Persist Security Info=False;User ID=mikeke373737@gmail.com@secure-resource;Password={your_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
            //services.AddIdentityServer()
            //    .AddInMemoryClients(identityDataService.GetClients())
            //    .AddTestUsers(identityDataService.GetTestUser())
            //    .AddInMemoryIdentityResources(identityDataService.GetIdentityResource())
            //    .AddInMemoryApiResources(identityDataService.GetApiResource())
            //    .AddInMemoryApiScopes(identityDataService.GetApiScope())
            //    .AddDeveloperSigningCredential();

            Action<DbContextOptionsBuilder> dbCtx = (ctx => ctx.UseSqlServer(connStr));

            var identityDataService = new IdentityDataInMemoryService();
            services.AddIdentityServer()
                    .AddTestUsers(identityDataService.GetTestUser())
                    .AddDeveloperSigningCredential()
                    .AddConfigurationStore(o =>
                    {
                        o.ConfigureDbContext = dbCtx;
                    })
                    .AddOperationalStore(o =>
                    {
                        o.ConfigureDbContext = dbCtx;
                    });

            services.AddSingleton<ICorsPolicyService>((container) => {
                var logger = container.GetRequiredService<ILogger<DefaultCorsPolicyService>>();
                return new DefaultCorsPolicyService(logger)
                {
                    AllowAll = true
                };
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCookiePolicy(new CookiePolicyOptions
            {
                HttpOnly = HttpOnlyPolicy.None,
                MinimumSameSitePolicy = SameSiteMode.None,
                Secure = CookieSecurePolicy.Always
            });

            //InitializeDatabase(app);

            app.UseMiddleware<ErrorLoggingMiddleware>();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseIdentityServer(); //adds the authentication middleware

            app.UseEndpoints(endpoints =>
            {
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapDefaultControllerRoute();
                });
            });
        }

        private void InitializeDatabase(IApplicationBuilder app)
        {
            var identityDataService = new IdentityDataInMemoryService();

            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();
                if (!context.Clients.Any())
                {
                    foreach (var client in identityDataService.GetClients())
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }
                
                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in identityDataService.GetIdentityResource())
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var resource in identityDataService.GetApiResource())
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }


                if (!context.ApiScopes.Any())
                {
                    foreach (var resource in identityDataService.GetApiScope())
                    {
                        context.ApiScopes.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
            }
        }
    }
}
