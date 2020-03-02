using Microsoft.AspNetCore.Components;
using StockManagement.Domain;
using StockManagement.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
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

        protected int? _selectedCategory;
        protected int? _selectedDescription;
        protected int? _selectedSupplier;
        protected bool _isDefective;
        protected string _comment;

        protected bool _submitFail;

        protected void CheckDefective()
        {
            _isDefective = !_isDefective;
        }

        protected override async Task OnInitializedAsync()
        {

            _categories = await Repository.GetAll<Category>();
            _suppliers = await Repository.GetAll<Supplier>();
            _users = await Repository.GetAll<ADUser>();
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
                _descriptions = await Repository.GetByCategory((int)_selectedCategory);
                _selectedDescription = _item.Product?.Id;
            }
        }

        protected async Task Submit()
        {
            if (_item.ADUser != null)
            {
                ItemUser lastUse = await Repository.GetLastUse(_item.ADUser.Id, _item.Id);
                lastUse.Close();
                Repository.Save(lastUse);
            }

            try
            {
                _item.SerialNumber = SerialNr;
                _item.Product = _descriptions.FirstOrDefault(i => _selectedDescription == i.Id);
                _item.Supplier = _suppliers.FirstOrDefault(i => _selectedSupplier == i.Id);
                _item.ADUser = null;
                _item.IsDefective = _isDefective;
                _item.Comment = _comment;
                _item.InStock = true;
                Repository.Save(_item);
                NavigationManager.NavigateTo("updatesucces/in/" + _selectedDescription, true);
            }
            catch (Exception ex)
            {
                _submitFail = true;
            }


        }
    }
}
