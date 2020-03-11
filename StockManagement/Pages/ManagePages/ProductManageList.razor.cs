using Microsoft.AspNetCore.Components;
using StockManagement.Domain;
using StockManagement.Domain.IRepositories;
using StockManagement.Domain.IServices;
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

        protected Category _category;
        protected Product _selectedProduct;

        protected bool _deleteFailProduct;
        protected bool _hasItems;

        protected override void OnInitialized()
        {
            _category = (Category) Repository.GetById(typeof(Category),Id);
            foreach (Product prod in _category.Products)
            {
                prod.AmountInStock = Repository.GetAmountInStockValue(prod.Id);
            }
        }

        protected async Task<IEnumerable<Product>> SearchProduct(string searchString)
        {
            return await Task.FromResult(_category.Products.Where(u => u.Description.ToLower().Contains(searchString.ToLower())));
        }

        protected void NavigateToProductDetail()
        {
            if (_selectedProduct != null)
            {
                NavigationManager.NavigateTo("/beheer/itemlijst/" + _selectedProduct.Id);
            }
        }

        protected void DeleteProduct(int id)
        {
            Product product = _category.Products.FirstOrDefault(p => p.Id == id);
            if (product.Items.Count == 0)
            {
                try
                {
                    Repository.Delete(product);
                    _category.Products.Remove(product);
                }
                catch (Exception ex)
                {
                    _deleteFailProduct = true;
                }
            } else
            {
                _hasItems = true;
            }
            

        }
    }
}
