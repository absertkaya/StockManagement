using Microsoft.AspNetCore.Components;
using StockManagement.Domain;
using StockManagement.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockManagement.Pages.OverviewPages
{
    public class ItemListBase : ComponentBase
    {
        [Inject]
        public IItemRepository Repository { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Parameter]
        public int Id { get; set; }

        protected IList<Item> _items;
        protected string _productName;

        protected bool _deleteFail;

        protected override async Task OnInitializedAsync()
        {
            Product prod = (Product)await Repository.GetById(typeof(Product), Id);

            _productName = prod.Description;
            _items = prod.Items;
        }

        protected void GetHistory(int id)
        {
            NavigationManager.NavigateTo("/itemhistoriek/" + id);
        }

        protected void EditItem(int id)
        {
            NavigationManager.NavigateTo("/itemform/in/id/" + id);
        }

        protected void DeleteItem(Item item)
        {
            try
            {
                Repository.Delete(item);
                _items.Remove(item);
            }
            catch (Exception ex)
            {
                _deleteFail = true;
            }
        }
    }
}
