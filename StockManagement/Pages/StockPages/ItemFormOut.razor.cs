using Blazor.Extensions.Storage.Interfaces;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using StockManagement.Domain;
using StockManagement.Domain.IRepositories;
using StockManagement.Domain.IServices;
using StockManagement.Graph;
using StockManagement.Pages.ReuseableComponents;
using System;
using System.Threading.Tasks;

namespace StockManagement.Pages.StockPages
{
    public class ItemFormOutBase : ComponentBase
    {

        [Inject]
        public IItemRepository Repository { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        private IUserRepository UserRepository { get; set; }
        [Inject]
        public IToastService ToastService { get; set; }
        [Inject]
        public MailService MailService { get; set; }
        [Parameter]
        public string SerialNr { get; set; }

        [CascadingParameter]
        protected Task<AuthenticationState> authenticationStateTask { get; set; }


        protected UserSearchBox _userSelect;

        protected Item _item;
        protected EditContext _editContext;
        protected bool _submitFail;

        protected bool _invalidSerialnr;

        protected override void OnInitialized()
        {
            _item = Repository.GetBySerialNr(SerialNr);
            _editContext = new EditContext(_item);
        }

        protected async Task Submit()
        {
            
            try
            {
                GraphUser graphUser = _userSelect.GetSelectedUser();
                ADUser aduser = (ADUser)await Repository.GetByIdAsync(typeof(ADUser), graphUser.Id);
                if (aduser == null)
                {
                    aduser = new ADUser(graphUser);
                    Repository.Save(aduser);
                }
                _item.ADUser = aduser;
                _item.InStock = false;

                var authState = await authenticationStateTask;
                var assigner = authState.User;

                ADUser adAssigner = UserRepository.GetByEmail(assigner.Identity.Name);
                ItemUser history = new ItemUser()
                {
                    User = aduser,
                    Item = _item,
                    AssignedBy = adAssigner
                };

                Repository.Save(history);
                Repository.Save(_item);

                ToastService.ShowSuccess("Item uit stock gehaald, er zijn nog " + _item.Product.Items.Count + " items in stock.");
                NavigationManager.NavigateTo("updatesucces/out/" + _item.Product?.Id, true);
            }
            catch (Exception ex)
            {
                _submitFail = true;
            }

            try
            {
                var aantal = await Repository.GetAmountInStockValueAsync(_item.Id);
                if (aantal < 5)
                {
                    MailService.SendMail($"Stock {_item.Product.Description} laag", $"Dit product heeft {aantal} items in stock.", "stockmanagervgd@gmail.com");
                }
            } catch (Exception ex)
            {
                //Log
            }

        }
    }
}
