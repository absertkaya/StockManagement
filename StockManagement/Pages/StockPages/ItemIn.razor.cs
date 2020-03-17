using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using StockManagement.Domain;
using StockManagement.Domain.IRepositories;
using StockManagement.Pages.ReuseableComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StockManagement.Pages.StockPages
{
    public class ItemInBase : ComponentBase
    {
        [Inject]
        public IItemRepository Repository { get; set; }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }
        [Inject]
        public IToastService ToastService { get; set; }

        protected int? _selectedCategory;
        protected IList<Category> _categories;
        protected int? _selectedSupplier;
        protected IList<Supplier> _suppliers;
        protected string _productNumber;
        protected IList<Product> _products;

        protected string _serialNumber;

        protected bool _serialScan;
        protected bool _hideSettings;

        protected EditContext _editContext;
        protected Item _item = new Item();
        protected QuaggaScanner _scanner;
        protected string _comment;

        protected override void OnInitialized()
        {
            _editContext = new EditContext(_item);
            _categories = Repository.GetAll<Category>();
            _suppliers = Repository.GetAll<Supplier>();
        }

        protected void FetchProducts(ChangeEventArgs e)
        {
            _selectedCategory = int.Parse(e.Value.ToString());
            _products = Repository.GetByCategory((int)_selectedCategory);
        }

        protected void ToggleSettings() { 
            _hideSettings = !_hideSettings;
        }

        protected void Submit()
        {
            Product product = null;
            Supplier supplier = null;
            string serialnr = null;
            if (_productNumber != null)
                product = _products.FirstOrDefault(p => p.ProductNumber == _productNumber);
            if (_selectedSupplier != null)
                supplier = _suppliers.FirstOrDefault(p => p.Id == _selectedSupplier);
            if (_serialNumber != null)
                serialnr = Regex.Replace(_serialNumber, @"\s+", "");
            _item.Product = product;
            _item.Supplier = supplier;
            _item.SerialNumber = serialnr;
            if (_editContext.Validate())
            {
                if (!Repository.ItemDuplicateExists(_item.Id, _item.SerialNumber, _item.Product.Id))
                {
                    Repository.Save(_item);
                    ToastService.ShowSuccess("Item toegevoegd");
                    product.AddItem(_item);
                    StateHasChanged();
                    _item = new Item()
                    {
                        Product = product,
                        Supplier = supplier,
                        SerialNumber = serialnr
                    };
                }
                else
                {
                    ToastService.ShowError("Duplicate item bestaat al in de databank.");
                }
                    
            }
        }
    }
}
