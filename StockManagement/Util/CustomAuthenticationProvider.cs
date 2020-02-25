using Microsoft.AspNetCore.Components.Authorization;
using StockManagement.Graph;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Components;

using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;


namespace StockManagement.Util
{
    public class CustomAuthenticationProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        public CustomAuthenticationProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public override async Task<AuthenticationState>
            GetAuthenticationStateAsync()
        {
            ClaimsPrincipal user;
            // Call the GetUser method to get the status
            // This only sets things like the AuthorizeView
            // and the AuthenticationState CascadingParameter
            GraphUser result = JsonConvert.DeserializeObject<GraphUser>(await (await _httpClient.GetAsync("https://graph.microsoft.com/v1.0/users/me")).Content.ReadAsStringAsync());
            // Was a UserName returned?
            if (result.Mail != "")
            {
                // Create a ClaimsPrincipal for the user
                var identity = new ClaimsIdentity(new[]
                {
                   new Claim(ClaimTypes.Name, result.Mail),
                }, "AzureAdAuth");
                user = new ClaimsPrincipal(identity);
            }
            else
            {
                user = new ClaimsPrincipal(); // Not logged in
            }
            return await Task.FromResult(new AuthenticationState(user));
        }
    }
}
