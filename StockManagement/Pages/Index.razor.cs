using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StockManagement.Domain;
using StockManagement.Domain.IRepositories;
using StockManagement.Graph;
using System.Net.Http;
using System.Threading.Tasks;

namespace StockManagement.Pages
{
    public class IndexBase : ComponentBase
    {
        [Inject]
        private IUserRepository UserRepository { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }
        [Inject]
        public AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        [Inject]
        public IConfiguration Configuration { get; set; }
        [Inject]
        public ProtectedApiCallHelper ProtectedApiCallHelper { get; set; }
        protected override async Task OnInitializedAsync()
        {

            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            if (UserRepository.GetByEmail(user.Identity.Name) == null)
            {
                IConfidentialClientApplication confidentialClientApplication =
                ConfidentialClientApplicationBuilder
                .Create(Configuration["AzureAd:ClientId"])
                .WithTenantId(Configuration["AzureAd:TenantId"])
                .WithClientSecret(Configuration["AzureAd:ClientSecret"])
                .Build();
                string[] scopes = new string[] { "https://graph.microsoft.com/.default" };
                AuthenticationResult result = null;
                result = await confidentialClientApplication.AcquireTokenForClient(scopes)
                    .ExecuteAsync();
                var httpClient = new HttpClient();
                var apiCaller = new ProtectedApiCallHelper(httpClient);
                var res = await apiCaller
                    .CallWebApiAndProcessResultASync(
                    $"https://graph.microsoft.com/v1.0/users/{user.Identity.Name}",
                    result.AccessToken
                    );
                SaveAndUpdateUser(res);
            }
        }

        private void SaveAndUpdateUser(JObject res)
        {
            GraphUser user = JsonConvert.DeserializeObject<GraphUser>(res.ToString());
            ADUser aduser = new ADUser(user);
            UserRepository.Save(aduser);
        }
    }
}
