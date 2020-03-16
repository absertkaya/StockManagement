using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using StockManagement.Domain;
using StockManagement.Domain.IRepositories;
using StockManagement.Graph;
using StockManagement.Pages.ReuseableComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StockManagement.Pages.StockPages
{
    public class ItemBase : ComponentBase
    {
        [Parameter]
        public string Method { get; set; }
        [CascadingParameter]
        protected Task<AuthenticationState> AuthenticationStateTask { get; set; }

        [Inject]
        public IItemRepository Repository { get; set; }
        [Inject]
        private IUserRepository UserRepository { get; set; }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        [Inject]
        public IToastService ToastService { get; set; }

        protected int? _selectedCategory;
        protected IList<Category> _categories;

        protected string _productNumber;
        protected IList<Product> _products;

        protected string _serialNumber;
        protected IEnumerable<Item> _items;

        protected bool _hideSettings;
        protected bool _serialScan;

        protected Item _item;

        protected QuaggaScanner _scanner;
        protected UserSearchBox userSearch;

        protected override void OnInitialized()
        {
            _categories = Repository.GetAll<Category>();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
                await JSRuntime.InvokeVoidAsync("JsFunctions.quagga");
        }

        protected void FetchProducts(ChangeEventArgs e)
        {
            _selectedCategory = int.Parse(e.Value.ToString());
            _products = Repository.GetByCategory((int)_selectedCategory);
        }

        protected void FetchItems(ChangeEventArgs e)
        {
            _productNumber = e.Value.ToString();
            if (Method == "out")
            {
                _items = _products.First(p => p.ProductNumber == _productNumber)
                   .Items.Where(i => i.ItemStatus == ItemStatus.INSTOCK);
            } else
            {
                _items = _products.First(p => p.ProductNumber == _productNumber)
                   .Items.Where(i => i.ItemStatus != ItemStatus.INSTOCK);
            }
        }

        protected void SelectItem(ChangeEventArgs e)
        {
            _serialNumber = e.Value.ToString();
            _item = _items.First(i => i.SerialNumber == _serialNumber);
        }

        protected void ToggleSettings()
        {
            _hideSettings = !_hideSettings;
        }

        protected async Task Submit()
        {
            if (string.IsNullOrWhiteSpace(_productNumber))
            {
                ToastService.ShowWarning("Selecteer of scan een product.");
                return;
            }
            var auth = await AuthenticationStateTask;
            var stockUser = UserRepository.GetByEmail(auth.User.Identity.Name);
            if (_item == null)
            {
                if (string.IsNullOrWhiteSpace(_serialNumber))
                {
                    ToastService.ShowWarning("Selecteer of scan een serienummer.");
                    return;
                }
                try
                {
                    _item = _items.First(i => i.SerialNumber == _serialNumber);
                } catch (Exception ex)
                {
                    ToastService.ShowError("Item bestaat niet.");
                    return;
                }
            }

            if (Method == "out")
            {
                if (_item.ItemStatus != ItemStatus.INSTOCK)
                {
                    ToastService.ShowWarning("Item kan niet uit stock gehaald worden met status: " + _item.ItemStatus.ToString());
                    return;
                }
                GraphUser gruser = userSearch.GetSelectedUser();
                if (gruser == null)
                {
                    ToastService.ShowWarning("Selecteer een gebruiker.");
                    return;
                }
                ADUser aduser = UserRepository.GetByEmail(stockUser.Mail);
                if (aduser == null)
                {
                    aduser = new ADUser(gruser);
                    Repository.Save(aduser);
                }
                _item.RemoveFromStock(aduser, stockUser);
                Repository.Save(_item);
                ToastService.ShowSuccess("Item uit stock gehaald, er zijn nog " + _item.Product.Items.Where(i => i.ItemStatus == ItemStatus.INSTOCK).Count() + " items van dit product.");
            } else
            {
                if (_item.ItemStatus != ItemStatus.OUTSTOCK)
                {
                    ToastService.ShowWarning("Item kan niet geretourneerd worden met status: " + _item.ItemStatus.ToString());
                    return;
                }
                _item.ReturnToStock(stockUser);
                Repository.Save(_item);
                ToastService.ShowSuccess("Item in stock geplaatst, er zijn " + _item.Product.Items.Where(i => i.ItemStatus == ItemStatus.INSTOCK).Count() + " items van dit product.");
            }
            _item = null;
            _serialNumber = _items.FirstOrDefault()?.SerialNumber;

        }
    }
}
