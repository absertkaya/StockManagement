using Microsoft.AspNetCore.Components;
using StockManagement.Domain;
using StockManagement.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Pages.ManagePages
{
    public class ManageBase : ComponentBase
    {
        protected IList<Product> _products;
        protected IList<Category> _categories;

        [Inject] public IItemRepository Repository { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }

        protected bool _deleteFailProduct;
        protected bool _deleteFailCategory;
        protected override async Task OnInitializedAsync()
        {
            _categories = await Repository.GetAll<Category>();
            _products = await Repository.GetAll<Product>();
            foreach (Product prod in _products)
            {
                prod.AmountInStock = await Repository.GetAmountInStockValue(prod.Id);
            }
        }

        protected int GetAmountOfProducts(Category cat)
        {
            return _products.Where(p => p.Category == cat).Count();
        }

        protected void AddProduct()
        {
            NavigationManager.NavigateTo("/productscanner");
        }

        protected void AddCategory()
        {
            NavigationManager.NavigateTo("/categorie");
        }

        protected void AddItem(int id)
        {
            NavigationManager.NavigateTo("/scanner/in");
        }

        protected void RemoveItem(int id)
        {
            NavigationManager.NavigateTo("/scanner/out");
        }

        protected void EditProduct(int id)
        {
            NavigationManager.NavigateTo("/productform/id/" + id);
        }

        protected void EditCategory(int id)
        {
            NavigationManager.NavigateTo("/categorie/" + id);
        }

        protected void DeleteProduct(int id)
        {
            try
            {
                Repository.Delete(_products.FirstOrDefault(p => p.Id == id));
                _products.Remove(_products.FirstOrDefault(p => p.Id == id));
            }
            catch (Exception ex)
            {
                _deleteFailProduct = true;
            }

        }

        protected void DeleteCategory(int id)
        {
            try
            {
                Repository.Delete(_categories.FirstOrDefault(p => p.Id == id));
                _categories.Remove(_categories.FirstOrDefault(p => p.Id == id));
                _products = _products.Where(p => p.Category.Id != id).ToList();
            }
            catch (Exception ex)
            {
                _deleteFailCategory = true;
            }
        }

        protected void GetItems(int id)
        {
            NavigationManager.NavigateTo("/itemlijst/" + id);
        }
    }
}
