using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using StockManagement.Domain;
using StockManagement.Domain.IRepositories;
using StockManagement.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace StockManagement.Pages.MailPages
{
    public class MailPageBase : ComponentBase
    {
        [Parameter]
        public string Id { get; set; }

        [Inject]
        public IItemRepository ItemRepository { get; set; }
        [Inject]
        public IConfiguration Configuration { get; set; }

        protected ADUser aduser;
        private IEnumerable<Item> _items;

        protected string _toAddress;
        protected string _subject;
        protected string _body;

        protected override async Task OnInitializedAsync()
        {
            aduser = (ADUser) await ItemRepository.GetByIdAsync(typeof(ADUser), Id);
            _items = await ItemRepository.GetItemsByUserAsync(Id);
            InitStrings();
        }

        private void InitStrings()
        {
            _subject = $"Uitdienst {aduser.FirstName} {aduser.LastName}";

            string itemsString = "";

            foreach (Item item in _items)
            {
                itemsString += $" - {item.Product.Description}\n";
            }

            _body = $"Beste [Placeholder],\n\n" +
                    $"Volgende zaken verwachten we van collega {aduser.FirstName} {aduser.LastName}:\n\n" +
                    itemsString +
                    $"\nAlvast bedankt om dit na te gaan bij uitdienst.\n\n" +
                    $"Met vriendelijke groeten";
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
