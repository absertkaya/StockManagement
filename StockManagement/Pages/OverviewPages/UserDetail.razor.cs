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

        [Parameter]
        public string Id { get; set; }

        protected IList<Item> _items;
        protected IList<ItemUser> _itemusers;
        protected ADUser _user;

        protected override async Task OnInitializedAsync()
        {
            _itemusers = await Repository.GetItemUsersByUser(Id);
            _user = (ADUser)await Repository.GetById(typeof(ADUser), Id);
            _items = await Repository.GetItemsByUser(Id);
        }

        protected DateTime GetFromDate(Item item)
        {
            return _itemusers.FirstOrDefault(i => i.Item == item && i.User == _user && i.ToDate == null).FromDate;
        }
    }
}
