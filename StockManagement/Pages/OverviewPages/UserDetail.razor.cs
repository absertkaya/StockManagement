using Blazored.Modal;
using Blazored.Modal.Services;
using Blazored.Toast.Services;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using StockManagement.Domain;
using StockManagement.Domain.IRepositories;
using StockManagement.Pages.ModalComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Pages.OverviewPages
{
    public class UserDetailBase : ComponentBase
    {
        [Inject]
        public IItemRepository Repository { get; set; }
        [Inject]
        private IUserRepository UserRepository { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public IModalService ModalService { get; set; }
        [Inject]
        public IToastService ToastService { get; set; }
        [Inject]
        public TelemetryClient Telemetry { get; set; }
        [Parameter]
        public string Id { get; set; }
        [CascadingParameter]
        protected Task<AuthenticationState> AuthenticationStateTask { get; set; }

        protected IList<Item> _items;
        protected IList<ItemUser> _itemusers;
        protected ADUser _user;
        private ADUser _loggedInUser;
        protected override async Task OnInitializedAsync()
        {
            try
            {
                var auth = await AuthenticationStateTask;
                _loggedInUser = UserRepository.GetByEmail(auth.User.Identity.Name);
                _itemusers = (await Repository.GetItemUsersByUser(Id)).Where(i => i.ToDate != null).ToList();
                _user = await UserRepository.GetUserDetailsAsync(Id);
                _items = await Repository.GetItemsByUserAsync(Id);
            }
            catch (Exception ex) {
                Telemetry.TrackException(ex);
                ToastService.ShowWarning("Fout bij het inladen van de data, herlaad de pagina.");
            }

        }

        protected void ReturnAll()
        {
            foreach (Item item in _items)
            {
                var iu = item.ReturnToStock(_loggedInUser);
                Repository.Save(item);
                _itemusers.Insert(0, iu);
            }
            _items = new List<Item>();
        }


        protected async Task ShowConfirmation()
        {
            var modal = ModalService.Show<Confirmation>("Retourneer alles");
            var res = await modal.Result;

            if (!res.Cancelled)
            {
                ReturnAll();
            }
        }


        protected void ReturnItem(Item item)
        {
            var iu = item.ReturnToStock(_loggedInUser);
            Repository.Save(item);
            _items.Remove(item);
            _itemusers.Insert(0, iu);
        }

        protected void AddSubscription()
        {
            var parameters = new ModalParameters();
            parameters.Add("User", _user);

            ModalService.Show<AddSubscription>("Abonnement", parameters);
        }

        protected void RowExpand(ItemUser iu)
        {
            var parameters = new ModalParameters();
            parameters.Add("ItemUser", iu);
            var opts = new ModalOptions()
            {
                HideHeader = true
            };
            
            ModalService.Show<ItemHistoryComponent>("History",parameters, opts);
        }

        protected void NavigateToItem(Item item)
        {
            NavigationManager.NavigateTo($"/itemhistoriek/{item.Id}/{_user.Id}");
        }
    }
}
