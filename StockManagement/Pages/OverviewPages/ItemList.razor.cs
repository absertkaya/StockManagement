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
using System.Linq;

namespace StockManagement.Pages.OverviewPages
{
    public class ItemListBase : ComponentBase
    {
        [Inject]
        public IItemRepository Repository { get; set; }
        [Inject]
        public IToastService ToastService { get; set; }
        [Inject]
        public IModalService ModalService { get; set; }
        [Parameter]
        public int? Id { get; set; }
        [Parameter]
        public string Route { get; set; }
        [Parameter]
        public int? SupplierId { get; set; }

        protected Product _product;
        protected Supplier _supplier;
        protected IList<Item> _items;
        protected IEnumerable<Item> _filteredItems;
        protected string _filterString = "";
        protected bool _instock;
        protected ItemStatus? _selectedStatus;
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
            _filteredItems = new List<Item>(_items);
        }

        protected void Filter()
        {
            _filteredItems = _items.Where(i => i.SerialNumber.Trim().ToLower()
            .Contains(_filterString.Trim().ToLower()) && (i.ItemStatus == _selectedStatus || _selectedStatus == null));
            
        }

        protected void CheckInStock()
        {
            _instock = !_instock;
            Filter();
        }

        private void DeleteItem(Item item)
        {
            try
            {
                Repository.Delete(item);
                _items.Remove(item);
                item.Supplier.Items.Remove(item);
                item.Product.Items.Remove(item);
                Filter();
            } catch (Exception ex)
            {
                ToastService.ShowError("Kon item niet verwijderen.");
            }
        }

        protected async Task ShowConfirmation(Item item)
        {
            var modal = ModalService.Show<Confirmation>("Delete Confirmation");
            var res = await modal.Result;

            if (!res.Cancelled)
            {
                DeleteItem(item);
            }
        }
    }
}
