using Blazored.Modal;
using Blazored.Modal.Services;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using StockManagement.Domain;
using StockManagement.Domain.IRepositories;
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
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public IModalService ModalService { get; set; }

        [Parameter]
        public string Id { get; set; }

        protected IList<Item> _items;
        protected IList<ItemUser> _itemusers;
        protected ADUser _user;

        protected override async Task OnInitializedAsync()
        {
            _itemusers = (await Repository.GetItemUsersByUser(Id)).Where(i => i.ToDate != null).ToList();
            _user = (ADUser)await Repository.GetByIdAsync(typeof(ADUser), Id);
            _items = await Repository.GetItemsByUser(Id);
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
            NavigationManager.NavigateTo("/itemhistoriek/"+item.Id);
        }
    }
}
