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

        protected IList<Product> _products;

        protected bool _deleteFailProduct;
        protected bool _hasItems;

        protected override void OnInitialized()
        {
            _products = Repository.GetByCategory(Id);
            foreach (Product prod in _products)
            {
                prod.AmountInStock = Repository.GetAmountInStockValue(prod.Id);
            }
        }

        protected void DeleteProduct(int id)
        {
            Product product = _products.FirstOrDefault(p => p.Id == id);
            if (product.Items.Count == 0)
            {
                try
                {
                    Repository.Delete(product);
                    _products.Remove(product);
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
