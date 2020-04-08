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

        private bool sortSerialNumberDesc;
        private bool sortStatusDesc;
        private bool sortHostnameDesc;
        private bool sortUserDesc = true;

        protected bool _hasHostnames;

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
                _hasHostnames = _items.Any(i => i.Hostname != null);
                _filteredItems = new List<Item>(_items).OrderBy(i => i.SerialNumber);
            } catch (Exception ex)
            {
                Telemetry.TrackException(ex);
                ToastService.ShowWarning("Fout bij het inladen van de data, herlaad de pagina.");
            }

        }

        protected void SortBySerialNumber()
        {
            if (!sortSerialNumberDesc)
            {
                _filteredItems = _filteredItems.OrderBy(i => i.SerialNumber);
            } else
            {
                _filteredItems = _filteredItems.OrderByDescending(i => i.SerialNumber);
            }

            sortSerialNumberDesc = !sortSerialNumberDesc;
        }

        protected void SortByInStock()
        {
            if (!sortStatusDesc)
            {
                _filteredItems = _filteredItems.OrderBy(i => i.ItemStatus);
            } else
            {
                _filteredItems = _filteredItems.OrderByDescending(i => i.ItemStatus);
            }

            sortStatusDesc = !sortStatusDesc;
        }

        protected void SortByHostname()
        {
            if (!sortHostnameDesc)
            {
                _filteredItems = _filteredItems.OrderBy(i => i.Hostname);
            }
            else
            {
                _filteredItems = _filteredItems.OrderByDescending(i => i.Hostname);
            }

            sortHostnameDesc = !sortHostnameDesc;
        }

        protected void SortByUser()
        {
            if (!sortUserDesc)
            {
                _filteredItems = _filteredItems.OrderBy(i => i.ADUser?.Mail);
            }
            else
            {
                _filteredItems = _filteredItems.OrderByDescending(i => i.ADUser?.Mail);
            }

            sortUserDesc = !sortUserDesc;
        }

        protected void Filter()
        {
            _filteredItems = _items.Where(i => (i.SerialNumber + i.Hostname).Trim().ToLower()
            .Contains(_filterString.Trim().ToLower()) && (i.ItemStatus == _selectedStatus || _selectedStatus == null));
        }

        protected void NavigateToItemHistory(Item item)
        {
            if (SupplierId == null)
            {
                NavigationManager.NavigateTo($"{(Route == "beheer"?"beheer":null)}/itemhistoriek/{item.Id}");
            } else
            {
                NavigationManager.NavigateTo($"/leverancier/{SupplierId}/itemhistoriek/{item.Id}");
            }
            
        }
    }
}
