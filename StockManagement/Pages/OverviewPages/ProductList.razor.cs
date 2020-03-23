using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using StockManagement.Domain;
using StockManagement.Domain.IRepositories;
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
        [Parameter]
        public int Id { get; set; }

        protected IEnumerable<Product> _products;
        protected Product _selectedProduct;
        protected string _category;

        protected override async Task OnInitializedAsync()
        {
            _category = ((Category) await Repository.GetByIdAsync(typeof(Category), Id)).CategoryName;
            _products = await Repository.GetByCategoryAsync(Id);
            foreach (Product prod in _products)
            {
                prod.AmountInStock = await Repository.GetAmountInStockValueAsync(prod.Id);
            }
        }

        protected async Task<IEnumerable<Product>> SearchProduct(string searchString)
        {
            return await Task.FromResult(_products.Where(u => u.Description.ToLower().Contains(searchString.ToLower())));
        }

        protected void NavigateToProductDetail()
        {
            if (_selectedProduct != null)
            {
                NavigationManager.NavigateTo("/itemlijst/" + _selectedProduct.Id);
            }
        }

        protected void NavigateToProductDetail(Product product)
        {

                NavigationManager.NavigateTo("/itemlijst/" + product.Id);

        }

    }
}
