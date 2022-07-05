using Microsoft.Extensions.DependencyInjection;
using SecureResource.Service.IdentityServer.Application.Services;
using SecureResource.Service.IdentityServer.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureResource.Service.IdentityServer.Infrastructure
{
    public static class Extensions
    {
        public static void AddInfrastructure(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IIdentityDataInMemoryService, IdentityDataInMemoryService>();
        }

    }
}
