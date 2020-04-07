using Blazor.FileReader;
using Blazored.Modal.Services;
using Blazored.Toast.Services;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using StockManagement.Data;
using StockManagement.Domain;
using StockManagement.Domain.IRepositories;
using StockManagement.Domain.IServices;
using StockManagement.Pages.ModalComponents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Pages.ManagePages
{
    public class ManageBase : ComponentBase
    {

        private IList<Category> _categories;
        private IList<Supplier> _suppliers;

        protected IEnumerable<Category> _sortedCategories;
        protected IEnumerable<Supplier> _sortedSuppliers;

        [Inject] 
        public IItemRepository Repository { get; set; }
        [Inject] 
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public IFileReaderService FileReaderService { get; set; }
        [Inject]
        public IToastService ToastService { get; set; }
        [Inject]
        public TelemetryClient Telemetry { get; set; }
        [CascadingParameter]
        protected Task<AuthenticationState> AuthenticationStateTask { get; set; }
        [Inject]
        private IUserRepository UserRepository { get; set; }

        protected ElementReference inputElement;
        protected string _value;

        private bool sortCategoryDesc;
        private bool sortSupplierDesc;

        protected override async Task OnInitializedAsync()
        {

            var auth = await AuthenticationStateTask;
            var stockUser = UserRepository.GetByEmail(auth.User.Identity.Name);

            if (stockUser == null || stockUser.StockRole != StockRole.ADMIN)
            {
                Telemetry.TrackEvent("AccessDenied");
                NavigationManager.NavigateTo("/accessdenied");
                return;
            }

            try
            {
                _categories = await Repository.GetAllAsync<Category>();
                _sortedCategories = new List<Category>(_categories);
                _suppliers = await Repository.GetAllAsync<Supplier>();
                _sortedSuppliers = new List<Supplier>(_suppliers);
            } catch (Exception e)
            {
                Telemetry.TrackException(e);
                ToastService.ShowWarning("Probleem bij het inladen van data, herlaad de pagina.");
            }
        }

        protected void NavigateToCategory(Category cat)
        {
            NavigationManager.NavigateTo("/beheer/productlijst/" + cat.Id);
        }

        protected void NavigateToSupplier(Supplier sup)
        {
            NavigationManager.NavigateTo("/leverancier/" + sup.Id);
        }

        protected void SortByCategory()
        {
            if (!sortCategoryDesc)
            {
                _sortedCategories = _sortedCategories.OrderBy(c => c.CategoryName);
            } else
            {
                _sortedCategories = _sortedCategories.OrderByDescending(c => c.CategoryName);
            }

            sortCategoryDesc = !sortCategoryDesc;
        }

        protected void SortBySupplier()
        {
            if (!sortSupplierDesc)
            {
                _sortedSuppliers = _sortedSuppliers.OrderBy(c => c.SupplierName);
            }
            else
            {
                _sortedSuppliers = _sortedSuppliers.OrderByDescending(c => c.SupplierName);
            }

            sortSupplierDesc = !sortSupplierDesc;
        }

        protected void DeleteSupplier(int id)
        {
            Supplier sup = _suppliers.FirstOrDefault(s => s.Id == id);
            if (sup.Items == null || sup.Items.Count == 0)
            {
                try
                {
                    Repository.Delete(sup);
                    _suppliers.Remove(sup);
                    Telemetry.TrackEvent("SupplierDelete");
                } catch (Exception ex)
                {
                    Telemetry.TrackException(ex);
                    ToastService.ShowError("Kon leverancier niet verwijderen.");
                }
            } else
            {
                Telemetry.TrackEvent("SupplierDeleteFail");
                ToastService.ShowError("Leverancier heeft items.");
            }
        }

        protected void DeleteCategory(int id)
        {
            Category cat = _categories.FirstOrDefault(p => p.Id == id);
            if (cat.Products == null || cat.Products.Count == 0)
            {
                try
                {
                    Repository.Delete(cat);
                    _categories.Remove(cat);
                    Telemetry.TrackEvent("CategoryDelete");
                }
                catch (Exception ex)
                {
                    Telemetry.TrackException(ex);
                    ToastService.ShowError("Kon categorie niet verwijderen.");
                }
            } else
            {
                Telemetry.TrackEvent("CategoryDeleteFail");
                ToastService.ShowError("Categorie heeft producten.");
            }
        }

        protected async Task ImportData()
        {
            var files = (await FileReaderService.CreateReference(inputElement).EnumerateFilesAsync()).ToList();
            
            foreach (var file in files)
            {
               
            }
        }
    }
}
