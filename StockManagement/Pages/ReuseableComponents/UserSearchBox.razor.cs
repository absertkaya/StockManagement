using Blazor.Extensions.Storage.Interfaces;
using Blazored.Toast.Services;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Microsoft.JSInterop;
using Newtonsoft.Json.Linq;
using StockManagement.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace StockManagement.Pages.ReuseableComponents
{
    public class UserSelectBoxBase : ComponentBase
    {
        [Inject]
        public IJSRuntime JSRuntime { get; set; }
        [Inject]
        public IConfiguration Configuration { get; set; }
        [Inject]
        public ProtectedApiCallHelper ProtectedApiCallHelper { get; set; }
        [Inject]
        public ISessionStorage SessionStorage { get; set; }
        [Inject]
        public TelemetryClient Telemetry { get; set; }
        [Inject]
        public IToastService ToastService { get; set; }
        public string UserId { get; set; }

        protected List<GraphUser> _colGraphUsers = new List<GraphUser>();
        protected GraphUser _selectedUser;


        protected override async Task OnAfterRenderAsync(bool firstrender)
        {
            if (firstrender)
            {
                try
                {
                    //if (await SessionStorage.GetItem<List<GraphUser>>("graphusers") == null)
                    //{
                        await ApiCall("https://graph.microsoft.com/v1.0/users?$top=999");
                    //    await SaveToSession();
                    //}
                    //else
                    //{
                    //    _colGraphUsers = await SessionStorage.GetItem<List<GraphUser>>("graphusers");
                    //}

                    StateHasChanged();
                } catch (Exception ex)
                {
                    Telemetry.TrackException(ex);
                }
            }
            
        }

        protected async Task ApiCall(string url)
        {
            try
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
                        url,
                        result.AccessToken
                        );
                await DisplayUsers(res);
                if (res.Properties().FirstOrDefault(p => p.Name == "@odata.nextLink") != null)
                {
                    await ApiCall(res.Properties().First(p => p.Name == "@odata.nextLink").Value.ToString());
                }
            } catch (Exception ex)
            {
                Telemetry.TrackException(ex);
                ToastService.ShowWarning("Fout bij het ophalen van de gebruikers.");
            }
        }

        private async Task SaveToSession()
        {
            await SessionStorage.SetItem("graphusers", _colGraphUsers);
        }


        public GraphUser GetSelectedUser()
        {
            return _selectedUser;
        }

        protected async Task<IEnumerable<GraphUser>> SearchUser(string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return _colGraphUsers;
            } 
            return await Task.FromResult(_colGraphUsers.Where(u => u.FirstAndLastName.ToLower().Contains(searchString.ToLower())));
        }

        protected async Task DisplayUsers(JObject result)
        {

            foreach (JProperty child in result.Properties().Where(p => !p.Name.StartsWith("@")))
            {
                _colGraphUsers.AddRange(
                    child.Value.ToObject<List<GraphUser>>()
                    );
            }
            if (_colGraphUsers.Count > 0)
            {
                UserId = _colGraphUsers
                    .OrderBy(x => x.DisplayName)
                    .FirstOrDefault().Id;
            }

            _colGraphUsers = _colGraphUsers
              .Where(u => u.Mail != null && u.GivenName != null && u.Surname != null && u.OfficeLocation != null && u.JobTitle != null)
              .ToList();

            await JSRuntime.InvokeVoidAsync("console.log", _colGraphUsers);
        }
    }
}
