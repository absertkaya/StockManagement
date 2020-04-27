using Blazored.Modal;
using Blazored.Modal.Services;
using Blazored.Toast.Services;
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
using StockManagement.Pages.ModalComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StockManagement.Pages.MiscPages
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
        [Inject]
        public IToastService ToastService { get; set; }
        [Inject]
        public IModalService ModalService { get; set; }

        protected ADUser aduser;
        private IList<Item> _items;

        protected string _toAddress;
        protected string _subject;
        protected string _body;

        protected int? _selectedTemplate;

        private ADUser stockUser;

        private string _mailToString = "";
        protected IList<MailTemplate> _templates;

        public string MailToString {
            get { return Regex.Replace(_mailToString, "\n", "%0D%0A"); }
            set { _mailToString = value; }
        }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                aduser = (ADUser)await ItemRepository.GetByIdAsync(typeof(ADUser), Id);
                _items = await ItemRepository.GetItemsByUserAsync(Id);
                _templates = await ItemRepository.GetAllAsync<MailTemplate>();
                var auth = await AuthenticationStateTask;
                stockUser = UserRepository.GetByEmail(auth.User.Identity.Name);
            } catch (Exception ex)
            {
                ToastService.ShowError("Fout bij het inladen van de data.");
            }

        }

        protected void SelectTemplate(ChangeEventArgs e)
        {
            _selectedTemplate = int.Parse(e.Value.ToString());
            MailTemplate template = _templates.FirstOrDefault(t => t.Id == _selectedTemplate);
            if (template != null)
            {
                string[] result = template.BuildMail(aduser.FullName, stockUser.FullName, stockUser.OfficeRole, _items);
                _subject = result[0];
                _body = result[1];
                FireStringChange();
            }
        }

        protected async Task ShowDeleteConfirm()
        {
            var modal = ModalService.Show<Confirmation>("Verwijder mailtemplate");
            var res = await modal.Result;

            if (!res.Cancelled)
            {
                var template = _templates.FirstOrDefault(t => t.Id == _selectedTemplate);
                DeleteTemplate(template);
            }
        }

        private void DeleteTemplate(MailTemplate template)
        {
            try
            {
                ItemRepository.Delete(template);
                _templates.Remove(template);
                _selectedTemplate = null;
                ToastService.ShowSuccess("Template verwijderd.");
            } catch (Exception ex)
            {
                ToastService.ShowError("Kon template niet verwijderen.");
            }
        }

        protected async Task AddTemplate()
        {

            var modal = ModalService.Show<AddMailTemplate>("Nieuwe template");
            var res = await modal.Result;

            if (!res.Cancelled)
            {
                _templates.Add((MailTemplate)res.Data);
            }
        }

        protected async Task EditTemplate()
        {
            var template = _templates.FirstOrDefault(t => t.Id == _selectedTemplate);
            var parameters = new ModalParameters();
            parameters.Add("MailTemplate", template);
            var modal = ModalService.Show<AddMailTemplate>("Edit template", parameters);
            var res = await modal.Result;

            if (!res.Cancelled)
            {
                template = (MailTemplate)res.Data;
                if (template != null)
                {
                    string[] result = template.BuildMail(aduser.FullName, stockUser.FullName, stockUser.OfficeRole, _items);
                    _subject = result[0];
                    _body = result[1];
                    FireStringChange();
                }
            }
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
