using Microsoft.AspNetCore.Components;
using StockManagement.Domain;
using StockManagement.Domain.IRepositories;
using StockManagement.Pages.ModalComponents;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazored.Modal;
using Blazored.Modal.Services;
using Blazored.Toast.Services;

namespace StockManagement.Pages.OverviewPages
{
    public class ItemListBase : ComponentBase
    {
        [Inject]
        public IItemRepository Repository { get; set; }
        [Inject]
        public IToastService ToastService { get; set; }
        [Parameter]
        public int? Id { get; set; }
        [Parameter]
        public string Route { get; set; }
        [Parameter]
        public int? SupplierId { get; set; }

        protected Product _product;
        protected Supplier _supplier;
        protected IList<Item> _items;

        protected override void OnInitialized()
        {
            if (Id != null)
            {
                _product = (Product)Repository.GetById(typeof(Product), Id);
                _items = _product.Items;
            }
            if (SupplierId != null)
            {
                _supplier = (Supplier)Repository.GetById(typeof(Supplier), SupplierId);
                _items = _supplier.Items;
            }
        }

        protected void DeleteItem(Item item)
        {
            try
            {
                Repository.Delete(item);
                item.Supplier.Items.Remove(item);
                item.Product.Items.Remove(item);
                StateHasChanged();
            } catch (Exception ex)
            {
                ToastService.ShowError("Kon item niet verwijderen.");
            }
        }
    }
}
