using Blazor.FileReader;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using StockManagement.Data;
using StockManagement.Domain;
using StockManagement.Domain.IRepositories;
using StockManagement.Domain.IServices;
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

        protected ElementReference inputElement;
        protected string _value;

        protected bool _loadFail;
        protected bool _deleteFailCategory;
        protected bool _hasProducts;

        protected bool _supplierHasItems;
        protected bool _deleteFailSupplier;

        protected bool _showDialog;

        [Inject]
        public IBlobService BlobService { get; set; }
        protected override void OnInitialized()
        {
            try
            {
                _categories = Repository.GetAll<Category>();
                _suppliers = Repository.GetAll<Supplier>();
            } catch (Exception e)
            {
                _loadFail = true;
            }
        }

        protected void DeleteSupplier(int id)
        {
            _supplierHasItems = false;
            _deleteFailSupplier = false;
            Supplier sup = _suppliers.FirstOrDefault(s => s.Id == id);
            if (sup.Items == null || sup.Items.Count == 0)
            {
                try
                {
                    Repository.Delete(sup);
                    _suppliers.Remove(sup);
                } catch (Exception ex)
                {
                    _deleteFailSupplier = true;
                }
            } else
            {
                _supplierHasItems = true;
            }
        }

        protected void DeleteCategory(int id)
        {
            _deleteFailCategory = false;
            _hasProducts = false;
            Category cat = _categories.FirstOrDefault(p => p.Id == id);
            if (cat.Products?.Count == 0)
            {
                try
                {
                    Repository.Delete(cat);
                    _categories.Remove(cat);
                }
                catch (Exception ex)
                {
                    _deleteFailCategory = true;
                }
            } else
            {
                _hasProducts = true;
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
