using Microsoft.Azure.KeyVault;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using StockManagement.Domain.IServices;
using System;

using System.Threading.Tasks;

namespace StockManagement.Data.Services
{
    public class KeyVaultService : IKeyVaultService
    {
        private readonly KeyVaultClient _kvClient;
        public IConfiguration Configuration { get; set; }

        public KeyVaultService(IConfiguration configuration)
        {
            Configuration = configuration;
            _kvClient = new KeyVaultClient(GetToken);
        }

        public string GetSecret(string key)
        {
            return _kvClient.GetSecretAsync(Configuration["KeyVaultUrl"], key).GetAwaiter().GetResult()?.Value;
        }


        public async Task<string> GetSecretAsync(string key)
        {
            return (await _kvClient.GetSecretAsync(Configuration["KeyVaultUrl"], key))?.Value;
        }

        public async Task<string> GetToken(string authority, string resource, string scope)
        {
            var authContext = new AuthenticationContext(authority);
            ClientCredential clientCred = new ClientCredential(Configuration["AzureAd:ClientId"],
                Configuration["AzureAd:ClientSecret"]);
            AuthenticationResult result = await authContext.AcquireTokenAsync(resource, clientCred);

            if (result == null)
                throw new InvalidOperationException("Failed to obtain the JWT token");

            return result.AccessToken;
        }

    }
}
