using Blazor.FileReader;
using Blazored.Modal.Services;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
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

        protected IList<Category> _categories;
        protected IList<Supplier> _suppliers;

        [Inject] 
        public IItemRepository Repository { get; set; }
        [Inject] 
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public IFileReaderService FileReaderService { get; set; }
        [Inject]
        public IToastService ToastService { get; set; }

        protected ElementReference inputElement;
        protected string _value;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                _categories = await Repository.GetAllAsync<Category>();
                _suppliers = await Repository.GetAllAsync<Supplier>();
            } catch (Exception e)
            {
                ToastService.ShowWarning("Probleem bij het inladen van data, herlaad de pagina.");
            }
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
                } catch (Exception ex)
                {
                    ToastService.ShowError("Kon leverancier niet verwijderen.");
                }
            } else
            {
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
                }
                catch (Exception ex)
                {
                    ToastService.ShowError("Kon categorie niet verwijderen.");
                }
            } else
            {
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
