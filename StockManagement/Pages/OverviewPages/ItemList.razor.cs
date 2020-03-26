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
using Microsoft.ApplicationInsights;

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
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public TelemetryClient Telemetry { get; set; }

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
            try
            {
                if (Id != null)
                {
                    _product = (Product)await Repository.GetByIdAsync(typeof(Product), Id);
                    _items = await Repository.GetByProductAsync(_product.Id);

                }
                if (SupplierId != null)
                {
                    _supplier = (Supplier)await Repository.GetByIdAsync(typeof(Supplier), SupplierId);
                    _items = await Repository.GetBySupplierAsync(_supplier.Id);
                }
                _filteredItems = new List<Item>(_items);
            } catch (Exception ex)
            {
                Telemetry.TrackException(ex);
                ToastService.ShowWarning("Fout bij het inladen van de data, herlaad de pagina.");
            }

        }

        protected void Filter()
        {
            _filteredItems = _items.Where(i => i.SerialNumber.Trim().ToLower()
            .Contains(_filterString.Trim().ToLower()) && (i.ItemStatus == _selectedStatus || _selectedStatus == null));
            
        }

        protected void NavigateToItemHistory(Item item)
        {
            NavigationManager.NavigateTo("/itemhistoriek/" + item.Id);
        }
    }
}
