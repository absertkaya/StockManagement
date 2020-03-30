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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StockManagement.Shared
{
    public class MainLayoutBase : LayoutComponentBase
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

        protected bool authorized;

        protected override async Task OnInitializedAsync()
        {

            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            ADUser aduser = UserRepository.GetByEmail(user.Identity.Name);

            if (aduser == null || aduser.StockRole == null)
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
                var resLogin = await apiCaller
                    .CallWebApiAndProcessResultASync(
                        $"https://graph.microsoft.com/v1.0/users/{user.Identity.Name}",
                        result.AccessToken
                    );

                if (!await CheckGroups(apiCaller, result, user, resLogin))
                {
                    authorized = false;
                    NavigationManager.NavigateTo("/accessdenied");
                    return;
                }
            } else
            {
                authorized = true;
            }
        }

        private async Task<bool> CheckGroups(ProtectedApiCallHelper apiCaller, AuthenticationResult result, ClaimsPrincipal user, JObject resLogin)
        {
            var resAdminGroup = await apiCaller
                .CallWebApiAndProcessResultASync(
                    $"https://graph.microsoft.com/v1.0/groups/a410ab6b-b60a-458b-bca9-053c0a7edbf9/members",
                    result.AccessToken
                );

            foreach (JProperty child in resAdminGroup.Properties().Where(p => !p.Name.StartsWith("@")))
            {
                if (child.Value.ToObject<List<GraphUser>>().Any(x => x.Mail == user.Identity.Name))
                {
                    SaveAndUpdateUser(resLogin, StockRole.ADMIN);
                    return true;
                }
            }

            var resStockerGroup = await apiCaller
                .CallWebApiAndProcessResultASync(
                    $"https://graph.microsoft.com/v1.0/groups/9ca8c40b-328c-4fe6-a2bf-ed9544611c71/members",
                    result.AccessToken
                );

            foreach (JProperty child in resStockerGroup.Properties().Where(p => !p.Name.StartsWith("@")))
            {
                if (child.Value.ToObject<List<GraphUser>>().Any(x => x.Mail == user.Identity.Name))
                {
                    SaveAndUpdateUser(resLogin, StockRole.STOCKUSER);
                    return true;
                }
            }
            return false;
        }

        private void SaveAndUpdateUser(JObject res, StockRole role)
        {
            GraphUser user = JsonConvert.DeserializeObject<GraphUser>(res.ToString());
            ADUser aduser = new ADUser(user);
            aduser.StockRole = role;
            UserRepository.Save(aduser);
        }
    }
}
