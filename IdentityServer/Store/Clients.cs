using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static IdentityModel.OidcConstants;

namespace IdentityServer.Store
{
    internal class Clients
    {
        public static IEnumerable<Client> Get()
        {
            return new List<Client> {
            new Client {
                ClientId = "apiClient",
                ClientName = "Micro Service Web API",
                AllowedGrantTypes = IdentityServer4.Models.GrantTypes.ClientCredentials,
                ClientSecrets = new List<Secret> {
                    new Secret("superSecretPassword".Sha256())},
                AllowedScopes = new List<string> {"API.read"}
            },
            new Client {
                ClientId = "openIdConnectClient",
                ClientName = "Example Implicit Client Application",
                AllowedGrantTypes = IdentityServer4.Models.GrantTypes.Implicit,
                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "role",
                    "API.write"
                },
               // AllowedCorsOrigins = new[] {"http://localhost:32779"},
                RedirectUris = new List<string> {"http://localhost:2020/signin-oidc"},
                PostLogoutRedirectUris = new List<string> { "http://localhost:2020/" }
                //RedirectUris = new List<string> {"http://localhost:6060/signin-oidc"},
                //PostLogoutRedirectUris = new List<string> { "http://localhost:6060/" }
            }
        };
        }
    }
}
