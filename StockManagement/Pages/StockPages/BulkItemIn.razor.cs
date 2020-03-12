using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
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
    public class BulkItemInBase : ComponentBase
    {
        [Inject]
        public IItemRepository Repository { get; set; }

        protected int? _selectedCategory;
        protected IList<Category> _categories;
        protected int? _selectedSupplier;
        protected IList<Supplier> _suppliers;
        protected int? _selectedProduct;
        protected IList<Product> _products;

        protected EditContext _editContext;
        protected Item _item = new Item();
        protected QuaggaScanner _scanner;
        protected string _comment;

        protected bool _duplicateExists;
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

        protected void Submit()
        {
            _duplicateExists = false;
            Product product = null;
            Supplier supplier = null;
            string serialnr = null;
            if (_selectedProduct != null)
                product = _products.FirstOrDefault(p => p.Id == _selectedProduct);
            if (_selectedSupplier != null)
                supplier = _suppliers.FirstOrDefault(p => p.Id == _selectedSupplier);
            if (_scanner.GetResult() != null)
                serialnr = Regex.Replace(_scanner.GetResult(), @"\s+", "");
            _item.Product = product;
            _item.Supplier = supplier;
            _item.SerialNumber = serialnr;
            if (_editContext.Validate())
            {
                if (!Repository.ItemDuplicateExists(_item.Id, _item.SerialNumber, _item.Product.Id))
                {
                    Repository.Save(_item);
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
                    _duplicateExists = true;
                }
                    
            }
        }
    }
}
