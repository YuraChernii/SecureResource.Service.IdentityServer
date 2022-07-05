using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using SecureResource.Service.IdentityServer.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SecureResource.Service.IdentityServer.Infrastructure.Services
{
    public class IdentityDataInMemoryService : IIdentityDataInMemoryService
    {
        public List<Client> GetClients()
        {
            return new List<Client>()
            {
                new Client()
                {
                    ClientId = "oauthClient",
                    ClientName = "Yurii Chernii client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = new List<Secret>()
                    {
                        new Secret("12345678901".Sha256())
                    },
                    AllowedScopes = new List<string>()
                    {
                        "SecureResource.read"
                    }

                },
                new Client
                {
                    ClientId = "oidcClient",
                    ClientName = "Example Client Application",
                    ClientSecrets = new List<Secret> {new Secret("SuperSecretPassword".Sha256())}, // change me!
    
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = new List<string> {"http://localhost:5100/callback"},
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "role",
                        "SecureResource.read"
                    },
                    RequirePkce = true,
                    AllowPlainTextPkce = false,
                    RequireClientSecret = false
                }
            };
        }
        public List<TestUser> GetTestUser()
        {
            return new List<TestUser> {
            new TestUser {
                SubjectId = "5BE86359-073C-434B-AD2D-A3932222DABE",
                Username = "scott",
                Password = "password",
                Claims = new List<Claim> {
                    new Claim(JwtClaimTypes.Email, "scott@scottbrady91.com"),
                    new Claim(JwtClaimTypes.Role, "admin")
                }
            }
        };
        }
        public List<IdentityResource> GetIdentityResource()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource()
                {
                    Name = "role",
                    UserClaims = new List<string>(){"role"}
                }
            };
        }
        public List<ApiResource> GetApiResource()
        {
            return new List<ApiResource>()
            {
                new ApiResource()
                {
                    Name ="SecureResource.Service",
                    DisplayName = "SecureResource service",
                    Description="bla bla description",
                    Scopes = new List<string> { "SecureResource.read", "SecureResource.write"},
                    ApiSecrets = new List<Secret> { new Secret("ScopeSecret".Sha256()) },
                    UserClaims = new List<string> { "role" }
                }
            };
        }
        public List<ApiScope> GetApiScope()
        {
            return new List<ApiScope>()
            {
                new ApiScope("SecureResource.read","Scope that allows to read"),
                new ApiScope("SecureResource.write")
            };
        }
    }
}
