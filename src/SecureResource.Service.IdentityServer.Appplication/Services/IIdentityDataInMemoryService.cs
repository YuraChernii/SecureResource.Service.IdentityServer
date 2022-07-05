using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;

namespace SecureResource.Service.IdentityServer.Application.Services
{
    public interface IIdentityDataInMemoryService
    {
        public List<Client> GetClients();
        public List<TestUser> GetTestUser();
        public List<IdentityResource> GetIdentityResource();
        public List<ApiResource> GetApiResource();
        public List<ApiScope> GetApiScope();

    }
}
