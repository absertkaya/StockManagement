using Blazored.Toast.Services;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using StockManagement.Domain;
using StockManagement.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Pages.OverviewPages
{
    public class ProductListBase : ComponentBase
    {
        [Inject]
        public IItemRepository Repository { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }
        [Inject]
        public IToastService ToastService { get; set; }
        [Inject]
        public TelemetryClient Telemetry { get; set; }
        [Parameter]
        public int Id { get; set; }

        private IEnumerable<Product> _products;
        protected List<Product> _filteredProducts;
        protected List<Product> toRenderList;
        protected string _filterString = "";
        protected string _category;

        private bool sortProductNameDesc;
        private bool sortProductNumberDesc;
        private bool sortAmountInStockDesc = true;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                //_category = ((Category)await Repository.GetByIdAsync(typeof(Category), Id))?.CategoryName;
                _products = await Repository.GetByCategoryAsync(Id);
                //foreach (Product prod in _products)
                //{
                //    prod.AmountInStock = await Repository.GetAmountInStockValueAsync(prod.Id);
                //}
                _filteredProducts = new List<Product>(_products);
                toRenderList = _filteredProducts.GetRange(0, 50);
            } catch (Exception ex)
            {
                Telemetry.TrackException(ex);
                ToastService.ShowWarning("Fout bij het inladen van de data, herlaad de pagina.");
            }

        }

        public void Next50()
        {
            try
            {
                toRenderList = _filteredProducts.GetRange(_filteredProducts.IndexOf(toRenderList[toRenderList.Count - 1]) + 1, 50);
            }
            catch (Exception ex)
            {
                toRenderList = _filteredProducts.GetRange(0, 50);
            }
        }

        public void Previous50()
        {
            try
            {
                toRenderList = _filteredProducts.GetRange(_filteredProducts.IndexOf(toRenderList[0]) - 50, 50);
            }
            catch (Exception ex)
            {
                toRenderList = _filteredProducts.GetRange(_filteredProducts.Count - 50, 50);
            }
        }

        protected void SortByProductNumber()
        {
            if (!sortProductNumberDesc)
            {
                _filteredProducts = _filteredProducts.OrderBy(p => p.ProductNumber).ToList();
            } else
            {
                _filteredProducts = _filteredProducts.OrderByDescending(p => p.ProductNumber).ToList();
            }
            sortProductNumberDesc = !sortProductNumberDesc;
        }

        protected void SortByProductName()
        {
            if (!sortProductNameDesc)
            {
                _filteredProducts = _filteredProducts.OrderBy(p => p.Description).ToList();
            }
            else
            {
                _filteredProducts = _filteredProducts.OrderByDescending(p => p.Description).ToList();
            }
            sortProductNameDesc = !sortProductNameDesc;
        }

        protected void SortByAmountInStock()
        {
            if (!sortAmountInStockDesc)
            {
                _filteredProducts = _filteredProducts.OrderBy(p => p.AmountInStock).ToList();
            }
            else
            {
                _filteredProducts = _filteredProducts.OrderByDescending(p => p.AmountInStock).ToList();
            }
            sortAmountInStockDesc = !sortAmountInStockDesc;
        }

        protected void Filter()
        {
            _filteredProducts = _products.Where(i => (i.Description.Trim().ToLower() + i.ProductNumber.Trim().ToLower())
                .Contains(_filterString.Trim().ToLower())).ToList();
        }

        protected void NavigateToProductDetail(Product product)
        {
                NavigationManager.NavigateTo("/itemlijst/" + product.Id);
        }

    }
}
