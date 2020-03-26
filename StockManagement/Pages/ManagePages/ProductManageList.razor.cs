using Blazored.Modal.Services;
using Blazored.Toast.Services;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using StockManagement.Domain;
using StockManagement.Domain.IRepositories;
using StockManagement.Domain.IServices;
using StockManagement.Pages.ModalComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Pages.ManagePages
{
    public class ProductManageListBase : ComponentBase
    {
        [Parameter]
        public int Id { get; set; }

        [Inject]
        public IItemRepository Repository { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public IModalService ModalService { get; set; }
        [Inject]
        public IToastService ToastService { get; set; }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }
        [Inject]
        public TelemetryClient Telemetry { get; set; }
        protected Category _category;
        private IList<Product> _products;
        protected IEnumerable<Product> _filteredProducts;
        protected string _filterString = "";

        protected bool _deleteFailProduct;
        protected bool _hasItems;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                _category = (Category)await Repository.GetByIdAsync(typeof(Category), Id);
                _products = await Repository.GetByCategoryAsync(Id);
                _filteredProducts = new List<Product>(_products);
                if (_products != null)
                {
                    foreach (Product prod in _products)
                    {
                        prod.AmountInStock = await Repository.GetAmountInStockValueAsync(prod.Id);
                    }
                }
            } catch (Exception ex)
            {
                Telemetry.TrackException(ex);
                ToastService.ShowWarning("Probleem bij het inladen van de data, herlaad de pagina.");
            }
        }

        protected void Filter()
        {
            _filteredProducts = _products.Where(p => (p.Description + p.ProductNumber).Trim().ToLower().Contains(_filterString.Trim().ToLower()));
        }

        protected void NavigateToProductDetail(Product prod)
        {
            NavigationManager.NavigateTo("/beheer/itemlijst/" + prod.Id);
        }

        private void DeleteProduct(Product product)
        {
            if (product.Items.Count == 0)
            {
                try
                {
                    Repository.Delete(product);
                    _products.Remove(product);
                    _category.Products.Remove(product);
                    Telemetry.TrackEvent("ProductDelete");
                }
                catch (Exception ex)
                {
                    Telemetry.TrackException(ex);
                    ToastService.ShowError("Kon product niet verwijderen.");
                }
            } else
            {
                Telemetry.TrackEvent("ProductDeleteFail");
                ToastService.ShowWarning("Product heeft nog items.");
            }
        }

        protected async Task ShowConfirmation(Product product)
        {
            var modal = ModalService.Show<Confirmation>("Delete Confirm");
            var res = await modal.Result;
            if (! res.Cancelled)
            {
                DeleteProduct(product);
            }
        }


    }
}
