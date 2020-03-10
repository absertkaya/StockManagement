using Microsoft.AspNetCore.Components;
using StockManagement.Domain;
using StockManagement.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Pages.StockPages
{
    public class BulkItemInBase : ComponentBase
    {
        [Inject]
        public IItemRepository Repository { get; set; }

        protected int? _selectedCategory;
        protected IList<Category> _categories;
        protected int? _selectedProduct;
        protected IList<Product> _products;

        protected override void OnInitialized()
        {
            _categories = Repository.GetAll<Category>();
        }

        protected void FetchProducts(ChangeEventArgs e)
        {
            _selectedCategory = int.Parse(e.Value.ToString());
            _products = Repository.GetByCategory((int)_selectedCategory);
        }
    }
}
