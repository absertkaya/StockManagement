using Microsoft.AspNetCore.Components;
using StockManagement.Domain;
using StockManagement.Domain.IRepositories;
using StockManagement.Pages.DialogComponents;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazored.Modal;
using Blazored.Modal.Services;

namespace StockManagement.Pages.OverviewPages
{
    public class ItemListBase : ComponentBase
    {
        [Inject]
        public IItemRepository Repository { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public IModalService Modal { get; set; }
        [Parameter]
        public int? Id { get; set; }
        [Parameter]
        public string Route { get; set; }
        [Parameter]
        public int? SupplierId { get; set; }

        protected Product _product;
        protected Supplier _supplier;
        protected Item _itemToDelete;
        protected bool _deleteFail;

        protected override void OnInitialized()
        {
            if (Id != null)
            {
                _product = (Product)Repository.GetById(typeof(Product), Id);
            }
            if (SupplierId != null)
            {
                _supplier = (Supplier)Repository.GetById(typeof(Supplier), SupplierId);
            }
        }

        protected void DeleteItem(Item item)
        {
            Repository.Delete(item);
            item.Supplier.Items.Remove(item);
            item.Product.Items.Remove(item);
            StateHasChanged();
        }
    }
}
