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

        protected override async Task OnInitializedAsync()
        {
            if (Id != null)
            {
                _product =  (Product) await Repository.GetByIdAsync(typeof(Product), Id);
                _items = await Repository.GetByProductAsync(_product.Id);
                
            }
            if (SupplierId != null)
            {
                _supplier = (Supplier) await Repository.GetByIdAsync(typeof(Supplier), SupplierId);
                _items = await Repository.GetBySupplierAsync(_supplier.Id);
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

        protected void RowExpand(Item item)
        {
            var parameters = new ModalParameters();
            parameters.Add(nameof(Item), item);
            var options = new ModalOptions()
            {
                HideHeader = true
            };
            var modal = ModalService.Show<ItemComponent>("Item Detail", parameters, options);
        }
    }
}
