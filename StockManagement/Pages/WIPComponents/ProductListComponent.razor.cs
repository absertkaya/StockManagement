using Microsoft.AspNetCore.Components;
using StockManagement.Domain;
using StockManagement.Domain.IRepositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockManagement.Pages.WIPComponents
{
    public class ProductListComponentBase : ComponentBase
    {
        [Inject]
        public IItemRepository Repository { get; set; }

        protected IList<Product> _products;
        protected override async Task OnInitializedAsync()
        {
            _products = await Repository.GetAllAsync<Product>();
            foreach (Product prod in _products)
            {
                prod.AmountInStock = await Repository.GetAmountInStockValueAsync(prod.Id);
            }
        }
    }
}
