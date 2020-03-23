using Blazored.Modal.Services;
using Blazored.Toast.Services;
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
    public class ProductManageListBase : ComponentBase, IDisposable
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
        protected Category _category;
        protected IList<Product> _products;
        protected Product _selectedProduct;

        protected bool _deleteFailProduct;
        protected bool _hasItems;

        protected override async Task OnInitializedAsync()
        {
            _category = (Category) await Repository.GetByIdAsync(typeof(Category),Id);
            _products = await Repository.GetByCategoryAsync(Id);
            if (_products != null)
            {
                foreach (Product prod in _products)
                {
                    prod.AmountInStock = await Repository.GetAmountInStockValueAsync(prod.Id);
                }
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

        private void DeleteProduct(Product product)
        {
            if (product.Items.Count == 0)
            {
                try
                {
                    Repository.Delete(product);
                    _category.Products.Remove(product);
                }
                catch (Exception ex)
                {
                    ToastService.ShowError("Kon product niet verwijderen.");
                }
            } else
            {
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

        public void Dispose()
        {
            Repository.Dispose();
        }
    }
}
