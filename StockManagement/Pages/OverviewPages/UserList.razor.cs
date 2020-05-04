using Blazored.Toast.Services;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
using StockManagement.Domain;
using StockManagement.Domain.IRepositories;
using StockManagement.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StockManagement.Pages.OverviewPages
{
    public class UserListBase : ComponentBase
    {
        [Inject]
        private IUserRepository UserRepository { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public TelemetryClient Telemetry { get; set; }
        [Inject]
        public IConfiguration Configuration { get; set; }
        [Inject]
        public IToastService ToastService { get; set; }

        private List<GraphUser> _graphUsers = new List<GraphUser>();
        protected IEnumerable<GraphUser> _filteredUsers;
        protected ADUser _selectedUser;
        protected string _filterString = "";

        private bool sortFirstNameDesc;
        private bool sortLastNameDesc;
        
        protected override async Task OnInitializedAsync()
        {
            await ApiCall("https://graph.microsoft.com/v1.0/users?$top=999");
            _filteredUsers = new List<GraphUser>(_graphUsers).OrderBy(u => u.GivenName).ThenBy(u => u.Surname);
        }

        private async Task ApiCall(string url)
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
                ProcessGraphUsers(res);
                if (res.Properties().FirstOrDefault(p => p.Name == "@odata.nextLink") != null)
                {
                    await ApiCall(res.Properties().First(p => p.Name == "@odata.nextLink").Value.ToString());
                }
            }
            catch (Exception ex)
            {
                Telemetry.TrackException(ex);
                ToastService.ShowWarning("Fout bij het ophalen van de gebruikers.");
            }
        }

        private void ProcessGraphUsers(JObject result)
        {

            foreach (JProperty child in result.Properties().Where(p => !p.Name.StartsWith("@")))
            {
                _graphUsers.AddRange(
                    child.Value.ToObject<List<GraphUser>>()
                    );
            }

            _graphUsers = _graphUsers
              .Where(u => u.Mail != null && u.GivenName != null && u.Surname != null && (u.JobTitle != null && u.OfficeLocation != null || u.JobTitle == null && u.OfficeLocation != null || u.JobTitle != null && u.OfficeLocation == null))
              .ToList();
        }

        protected void SortByFirstName()
        {
            if (!sortFirstNameDesc)
            {
                _filteredUsers = _filteredUsers.OrderBy(u => u.GivenName);
            } 
            else
            {
                _filteredUsers = _filteredUsers.OrderByDescending(u => u.GivenName);
            }

            sortFirstNameDesc = !sortFirstNameDesc;
        }

        protected void SortByLastName()
        {
            if (!sortLastNameDesc)
            {
                _filteredUsers = _filteredUsers.OrderBy(u => u.Surname);
            }
            else
            {
                _filteredUsers = _filteredUsers.OrderByDescending(u => u.Surname);
            }

            sortLastNameDesc = !sortLastNameDesc;
        }



        protected void Filter()
        {
            _filteredUsers = _graphUsers.Where(u => Regex.Replace(u.GivenName + u.Surname + u.Mail, " ", "").ToLower().Contains(Regex.Replace(_filterString.ToLower(), " ", "")));
        }

        protected void NavigateToUserDetail(GraphUser user)
        {
            if (! UserRepository.ADUserExists(user.Id))
            {
                var aduser = new ADUser(user);
                UserRepository.Save(aduser);
            }
            NavigationManager.NavigateTo("/gebruiker/" + user.Id);
        }
    }
}
