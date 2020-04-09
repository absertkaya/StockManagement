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
    public class ItemListComponentBase : ComponentBase
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Parameter]
        public string Route { get; set; }
        [Parameter]
        public int? SupplierId { get; set; }
        [Parameter]
        public IEnumerable<Item> Items { get; set; }

        protected Product _product;
        protected Supplier _supplier;
        protected ItemStatus? _selectedStatus;

        private bool sortSerialNumberDesc;
        private bool sortStatusDesc;
        private bool sortHostnameDesc;
        private bool sortUserDesc = true;

        protected bool _hasHostnames;

        protected override void OnParametersSet()
        {
            _hasHostnames = Items.Any(i => i.Hostname != null);
        }

        protected void SortBySerialNumber()
        {
            if (!sortSerialNumberDesc)
            {
                Items = Items.OrderBy(i => i.SerialNumber);
            }
            else
            {
                Items = Items.OrderByDescending(i => i.SerialNumber);
            }

            sortSerialNumberDesc = !sortSerialNumberDesc;
        }

        protected void SortByInStock()
        {
            if (!sortStatusDesc)
            {
                Items = Items.OrderBy(i => i.ItemStatus);
            }
            else
            {
                Items = Items.OrderByDescending(i => i.ItemStatus);
            }

            sortStatusDesc = !sortStatusDesc;
        }

        protected void SortByHostname()
        {
            if (!sortHostnameDesc)
            {
                Items = Items.OrderBy(i => i.Hostname);
            }
            else
            {
                Items = Items.OrderByDescending(i => i.Hostname);
            }

            sortHostnameDesc = !sortHostnameDesc;
        }

        protected void SortByUser()
        {
            if (!sortUserDesc)
            {
                Items = Items.OrderBy(i => i.ADUser?.Mail);
            }
            else
            {
                Items = Items.OrderByDescending(i => i.ADUser?.Mail);
            }

            sortUserDesc = !sortUserDesc;
        }

        protected void NavigateToItemHistory(Item item)
        {
            if (SupplierId == null)
            {
                NavigationManager.NavigateTo($"{(Route == "beheer" ? "beheer" : null)}/itemhistoriek/{item.Id}");
            }
            else
            {
                NavigationManager.NavigateTo($"/leverancier/{SupplierId}/itemhistoriek/{item.Id}");
            }

        }
    }
}
