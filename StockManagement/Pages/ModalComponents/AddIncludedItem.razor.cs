using Blazored.Modal;
using Blazored.Modal.Services;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using StockManagement.Domain;
using StockManagement.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Pages.ModalComponents
{
    public class AddIncludedItemBase : ComponentBase
    {
        [CascadingParameter]
        private BlazoredModalInstance BlazoredModal { get; set; }

        [Inject]
        public IItemRepository Repository { get; set; }
        [Inject]
        public IToastService ToastService { get; set; }

        [Parameter]
        public Item ParentItem { get; set; }

        protected int? _selectedCategory;
        protected IList<Category> _categories;

        protected string _productNumber;
        protected IList<Product> _products;

        protected string _serialNumber;
        protected IEnumerable<Item> _items;

        protected Item _item;

        protected override async Task OnInitializedAsync()
        {
            _categories = await Repository.GetAllAsync<Category>();
        }

        protected async Task FetchProducts(ChangeEventArgs e)
        {
            _selectedCategory = int.Parse(e.Value.ToString());
            _products = await Repository.GetByCategoryAsync((int)_selectedCategory);
        }

        protected void FetchItems(ChangeEventArgs e)
        {
            _productNumber = e.Value.ToString();
            _items = _products.First(p => p.ProductNumber == _productNumber).Items;
        }

        protected void SelectItem(ChangeEventArgs e)
        {
            _serialNumber = e.Value.ToString();
            _item = _items.First(i => i.SerialNumber == _serialNumber);
        }

        protected void Submit()
        {
            if (_item != null)
            {
                ParentItem.AddItem(_item);
                Repository.Save(ParentItem);
                BlazoredModal.Close(ModalResult.Ok<Item>(_item));
                ToastService.ShowSuccess("Item inbegrepen");
            } else
            {
                ToastService.ShowWarning("Selecteer een item.");
            }
        }
    }
}
