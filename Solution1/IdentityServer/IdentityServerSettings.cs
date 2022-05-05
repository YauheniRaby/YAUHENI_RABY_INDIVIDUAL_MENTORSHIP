using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityServer
{
    public static class IdentityServerSettings
    {
        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                new ApiScope("weather_api", "WeatherAPI")
            };

        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new List<ApiResource>
            {
                new ApiResource("weather_api", "WeatherAPI")
                {
                    Scopes = {"weather_api"}
                }
            };

        public static IEnumerable<Client> Clients (ICollection<string> urls) =>
            new List<Client>
            {
                new Client
                {
                    ClientId = "swagger_api",
                    ClientName = "Swagger API",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    RequireClientSecret = false,
                    AllowedScopes = {"weather_api"},
                    AllowedCorsOrigins = urls,
                    AccessTokenLifetime = 86400,
                }
            };
    }
}
