using Blazored.Toast.Services;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
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
        [CascadingParameter]
        protected Task<AuthenticationState> AuthenticationStateTask { get; set; }
        [Inject]
        private IUserRepository UserRepository { get; set; }
        [Inject]
        public TelemetryClient Telemetry { get; set; }
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
            var auth = await AuthenticationStateTask;
            var stockUser = UserRepository.GetByEmail(auth.User.Identity.Name);

            if (stockUser == null || stockUser.StockRole != StockRole.ADMIN)
            {
                NavigationManager.NavigateTo("/accessdenied");
                return;
            }

            try
            {
                _categories = await Repository.GetAllAsync<Category>();
            } catch (Exception ex)
            {
                Telemetry.TrackException(ex);
                ToastService.ShowWarning("Fout bij het inladen van de data, herlaad de pagina.");
            }
            
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
                        Telemetry.TrackEvent("NonUniqueProductNumber");
                        NavigationManager.NavigateTo("/beheer");
                    }
                    catch (Exception ex)
                    {
                        Telemetry.TrackException(ex);
                        ToastService.ShowError("Kon product niet opslaan.");
                    }
                } else
                {
                    Telemetry.TrackEvent("NonUniqueProductNumber");
                    ToastService.ShowError("Product met identiek productnummer bestaat al.");
                }

            }

        }
    }
}
