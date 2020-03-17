using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using StockManagement.Domain;
using StockManagement.Domain.IComponents;
using StockManagement.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StockManagement.Pages.ManagePages
{
    public class ProductFormBase : ComponentBase
    {
        [Inject] 
        public IItemRepository Repository { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public IToastService ToastService { get; set; }

        [Parameter]
        public int? Id { get; set; }

        protected IList<Category> _categories;
        protected int? _selectedCategory;
        protected Product _product = new Product();
        protected EditContext _editContext;
        protected IScannerComponent _scanner;

        protected override void OnInitialized()
        {
            if (Id != null)
            { 
                _product = (Product) Repository.GetById(typeof(Product), Id);
                if (_product == null)
                {
                    NavigationManager.NavigateTo("/error");
                }
            }
            _editContext = new EditContext(_product);
            
        }
        protected override async Task OnInitializedAsync()
        {
            _categories = await Repository.GetAllAsync<Category>();
        }

        protected void SelectCategory(ChangeEventArgs e)
        {
            int id = int.Parse(e.Value.ToString());
            _product.Category = _categories.FirstOrDefault(c => c.Id == id);
        }

        protected void Submit()
        {
            if (_editContext.Validate())
            {
                _product.ProductNumber = Regex.Replace(_product.ProductNumber, @"\s+", "");
                if (!Repository.ProductDuplicateExists(_product.Id, _product.ProductNumber))
                {
                    try
                    {
                        Repository.Save(_product);
                        _product.Category.Products.Add(_product);
                        ToastService.ShowSuccess("Product: " + _product.Description + " werd toegevoegd in categorie: " + _product.Category.CategoryName);
                        NavigationManager.NavigateTo("/beheer");
                    }
                    catch (Exception ex)
                    {
                        ToastService.ShowError("Kon product niet opslaan.");
                    }
                } else
                {
                    ToastService.ShowError("Product met identiek productnummer bestaat al.");
                }

            }

        }
    }
}
