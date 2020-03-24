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

        private IEnumerable<Product> _products;
        protected IEnumerable<Product> _filteredProducts;
        protected string _filterString = "";
        protected string _category;


        protected override async Task OnInitializedAsync()
        {
            _category = ((Category) await Repository.GetByIdAsync(typeof(Category), Id)).CategoryName;
            _products = await Repository.GetByCategoryAsync(Id);
            foreach (Product prod in _products)
            {
                prod.AmountInStock = await Repository.GetAmountInStockValueAsync(prod.Id);
            }
            _filteredProducts = new List<Product>(_products);
        }

        protected void Filter()
        {
            _filteredProducts = _products.Where(i => (i.Description.Trim().ToLower() + i.ProductNumber.Trim().ToLower())
                .Contains(_filterString.Trim().ToLower()));
        }

        protected void NavigateToProductDetail(Product product)
        {
                NavigationManager.NavigateTo("/itemlijst/" + product.Id);
        }

    }
}
