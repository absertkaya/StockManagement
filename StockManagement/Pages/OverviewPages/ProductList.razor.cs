using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using StockManagement.Domain;
using StockManagement.Domain.IRepositories;

namespace StockManagement.Pages.OverviewPages
{
    public class ProductListBase : ComponentBase
    {
        [Inject]
        public IItemRepository Repository { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Parameter]
        public int Id { get; set; }

        protected IList<Product> _products;
        protected string _category;

        protected override async Task OnInitializedAsync()
        {
            _category = ((Category)await Repository.GetById(typeof(Category), Id)).CategoryName;
            _products = await Repository.GetByCategory(Id);
            foreach (Product prod in _products)
            {
                prod.AmountInStock = await Repository.GetAmountInStockValue(prod.Id);
            }
        }

        protected void GetItems(int id)
        {
            NavigationManager.NavigateTo("/itemlijst/" + id);
        }
    }
}
