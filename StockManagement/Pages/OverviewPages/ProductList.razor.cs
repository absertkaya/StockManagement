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
            _category = ((Category)await Repository.GetByIdAsync(typeof(Category), Id)).CategoryName;
            _products = await Repository.GetByCategory(Id);
            foreach (Product prod in _products)
            {
                prod.AmountInStock = await Repository.GetAmountInStockValue(prod.Id);
            }
        }

        public async Task Filter(ChangeEventArgs e)
        {
            string filterString = e.Value.ToString().Trim().ToLower();
            int[] productIds = _products.Select(p => p.Id).ToArray();
            int[] filteredIds = _products.Where(p => !p.Description.ToLower().Contains(filterString)).Select(p => p.Id).ToArray();
            await JSRuntime.InvokeVoidAsync("JsFunctions.filterProducts", filteredIds, productIds);
        }

        protected void GetItems(int id)
        {
            NavigationManager.NavigateTo("/itemlijst/" + id);
        }
    }
}
