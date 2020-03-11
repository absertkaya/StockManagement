using Microsoft.AspNetCore.Components;
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
    public class ItemFormInBase : ComponentBase
    {
        [Inject]
        public IItemRepository Repository { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Parameter]
        public string SerialNr { get; set; }

        [Parameter]
        public int? Id { get; set; }

        protected Item _item;

        protected IList<Category> _categories;
        protected IList<Product> _descriptions;
        protected IList<Supplier> _suppliers;
        protected IList<ADUser> _users;

        protected FileUploadComponent _fileUpload;

        protected int? _selectedCategory;
        protected int? _selectedDescription;
        protected int? _selectedSupplier;
        protected bool _isDefective;
        protected string _comment;
        protected DateTime _deliveryDate = DateTime.Today;
        protected DateTime _invoiceDate = DateTime.Today;


        protected bool _submitFail;
        protected bool _duplicateItem;
        protected string? _errorMessage; 

        protected void CheckDefective()
        {
            _isDefective = !_isDefective;
        }

        protected override async Task OnInitializedAsync()
        {

            _categories = await Repository.GetAllAsync<Category>();
            _suppliers = await Repository.GetAllAsync<Supplier>();
            _users = await Repository.GetAllAsync<ADUser>();
            if (SerialNr != null)
            {
                _item = Repository.GetBySerialNr(SerialNr);
            }
            if (Id != null)
            {
                _item = (Item)await Repository.GetByIdAsync(typeof(Item), Id);
                SerialNr = _item.SerialNumber;
            }

            if (_item == null)
            {
                _item = new Item();
            }
            else
            {
                _selectedCategory = _item.Product.Category.Id;
                await FetchDescriptions();
                _selectedSupplier = _item.Supplier?.Id;
                _isDefective = _item.IsDefective;
                _comment = _item.Comment;
                _deliveryDate = _item.DeliveryDate;
                _invoiceDate = _item.InvoiceDate;
            }

        }

        public async Task FireChange(ChangeEventArgs e)
        {
            _selectedCategory = int.Parse(e.Value.ToString());
            await FetchDescriptions();
        }

        public async Task FetchDescriptions()
        {
            if (_selectedCategory != null)
            {
                _descriptions = await Repository.GetByCategoryAsync((int)_selectedCategory);
                _selectedDescription = _item.Product?.Id;
            }
        }

        protected async Task Submit()
        {
            _errorMessage = null;
            if (_item.ADUser != null)
            {
                ItemUser lastUse = await Repository.GetLastUse(_item.ADUser.Id, _item.Id);
                lastUse.Close();
                Repository.Save(lastUse);
            }

            if (SerialNr == null)
            {
                _errorMessage = "Serienummer is verplicht.";
                return;
            }
            _item.SerialNumber = Regex.Replace(SerialNr, @"\s+", "");

            if (_selectedDescription == null)
            {
                _errorMessage = "Product is verplicht.";
                return;
            }
                _item.Product = _descriptions.FirstOrDefault(i => _selectedDescription == i.Id);
            if (_selectedSupplier == null)
            {
                _errorMessage = "Leverancier is verplicht.";
                return;
            }
            _item.Supplier = _suppliers.FirstOrDefault(i => _selectedSupplier == i.Id);
            _item.ADUser = null;
            _item.IsDefective = _isDefective;
            _item.Comment = _comment;
            _item.InStock = true;
            _item.DeliveryDate = _deliveryDate;
            _item.InvoiceDate = _invoiceDate;

            if (_item.Product != null && !Repository.ItemDuplicateExists(_item.Id, _item.SerialNumber, _item.Product.Id))
            {
                try
                {
                    Repository.Save(_item);
                    if (!await _fileUpload.IsEmpty())
                    {
                        try
                        {
                            await Upload(_item);
                        }
                        catch (Exception ex)
                        {
                            //Log
                        }
                    }
                    NavigationManager.NavigateTo("updatesucces/in/" + _selectedDescription, true);
                }
                catch (Exception ex)
                {
                    _submitFail = true;
                }
            } else
            {
                _submitFail = true;
            }
        }
        protected async Task Clear()
        {
            await _fileUpload.ClearFile();
        }

        private async Task Upload(Item item)
        {
            _fileUpload.Container = "item" + item.Id;
            await _fileUpload.Upload("item" + item.Id + DateTime.Now.ToString("ddMMyyyyHHmmss"));
            await Clear();
        }
    }
}
