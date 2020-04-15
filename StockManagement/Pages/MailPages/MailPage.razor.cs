using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using StockManagement.Data.Repositories;
using StockManagement.Domain;
using StockManagement.Domain.IRepositories;
using StockManagement.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StockManagement.Pages.MailPages
{
    public class MailPageBase : ComponentBase
    {
        [CascadingParameter]
        protected Task<AuthenticationState> AuthenticationStateTask { get; set; }
        [Parameter]
        public string Id { get; set; }

        [Inject]
        public IItemRepository ItemRepository { get; set; }
        [Inject]
        private IUserRepository UserRepository { get; set; }
        [Inject]
        public IConfiguration Configuration { get; set; }

        protected ADUser aduser;
        private IEnumerable<Item> _items;

        protected string _toAddress;
        protected string _subject;
        protected string _body;

        private string _mailToString = "";

        public string MailToString {
            get { return Regex.Replace(_mailToString, "\n", "%0D%0A"); }
            set { _mailToString = value; }
        }

        
        protected override async Task OnInitializedAsync()
        {
            aduser = (ADUser) await ItemRepository.GetByIdAsync(typeof(ADUser), Id);
            _items = await ItemRepository.GetItemsByUserAsync(Id);
            await InitStrings();
        }

        private async Task InitStrings()
        {
            var auth = await AuthenticationStateTask;
            var stockUser = UserRepository.GetByEmail(auth.User.Identity.Name);

            _subject = $"Uitdienst {aduser.FirstName} {aduser.LastName}";

            string itemsString = "";

            foreach (Item item in _items)
            {
                itemsString += $"    - {item.Product.Description} {(item.SerialNumber == "Geen serienummer beschikbaar." ? "" : $"(S/N: {item.SerialNumber})")}\n";
            }

            _body = $"Beste [Placeholder],\n\n" +
                    $"Volgende zaken verwachten we van collega {aduser.FirstName} {aduser.LastName}:\n\n" +
                    itemsString +
                    $"\nAlvast bedankt om dit na te gaan bij uitdienst.\n\n" +
                    $"Met vriendelijke groeten\n\n" +
                    $"{stockUser.FirstName} {stockUser.LastName}\n" +
                    $"{stockUser.OfficeRole}\n\n";

            MailToString = $"mailto:{_toAddress}?subject={_subject}&body={_body}";
        }

        protected void FireStringChange()
        {
            MailToString = $"mailto:{_toAddress}?subject={_subject}&body={_body}";
        }

        protected async Task Submit()
        {
            string[] scopes = new string[] { "https://graph.microsoft.com/.default" };
            IConfidentialClientApplication confidentialClientApplication =
                    ConfidentialClientApplicationBuilder
                .Create(Configuration["AzureAd:ClientId"])
                .WithTenantId(Configuration["AzureAd:TenantId"])
                .WithClientSecret(Configuration["AzureAd:ClientSecret"])
                .Build();

            AuthorizationCodeProvider authProvider = new AuthorizationCodeProvider(confidentialClientApplication, scopes);
            GraphServiceClient graphClient = new GraphServiceClient(authProvider);
            var message = new Message
            {
                Subject = _subject,
                Body = new ItemBody()
                {
                    ContentType = BodyType.Text,
                    Content = _body
                },
                ToRecipients = new List<Recipient>()
                {
                    new Recipient()
                    {
                        EmailAddress = new EmailAddress
                        {
                            Address = _toAddress
                        }
                    }
                }
            };
            await graphClient.Me.SendMail(message, null).Request().PostAsync();

        }
    }
}
