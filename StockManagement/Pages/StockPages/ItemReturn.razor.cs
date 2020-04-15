using Blazored.Toast.Services;
using Microsoft.ApplicationInsights;
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
        [Inject]
        public TelemetryClient Telemetry { get; set; }

        protected int? _selectedCategory;
        protected IList<Category> _categories;

        protected string _productNumber;
        protected IList<Product> _products;

        protected string _serialNumber;
        protected IEnumerable<Item> _items;

        protected bool _hideSettings;
        protected bool _serialScan;
        protected bool _isDefective;

        protected Item _item;
        protected string _comment;

        protected FileUploadComponent _fileUpload;
        protected QuaggaScanner _scanner;
        protected UserSearchBox userSearch;

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
            if (Method == "out")
            {
                _items = _products.First(p => p.ProductNumber == _productNumber)
                   .Items.Where(i => i.ItemStatus == ItemStatus.INSTOCK);
            } else
            {
                _items = _products.First(p => p.ProductNumber == _productNumber)
                   .Items.Where(i => i.ItemStatus == ItemStatus.OUTSTOCK);
            }
        }

        protected void SelectItem(ChangeEventArgs e)
        {
            _serialNumber = e.Value.ToString();
            _item = _items.First(i => i.SerialNumber == _serialNumber);
            _comment = _item.Comment;
        }

        protected void ToggleSettings()
        {
            _hideSettings = !_hideSettings;
        }

        private bool HandleOut(ADUser stockUser)
        {
            if (_item.ItemStatus != ItemStatus.INSTOCK)
            {
                ToastService.ShowWarning("Item kan niet uit stock gehaald worden met status: " + _item.ItemStatus.ToString());
                return false;
            }
            GraphUser graphUser = userSearch.GetSelectedUser();
            if (graphUser == null)
            {
                ToastService.ShowWarning("Selecteer een gebruiker.");
                return false;
            }
            ADUser aduser = UserRepository.GetByEmail(graphUser.Mail);
            if (aduser == null)
            {
                aduser = new ADUser(graphUser);
                try
                {
                    Repository.Save(aduser);
                } catch (Exception ex)
                {
                    Telemetry.TrackException(ex);
                    return false;
                }
            }
            _item.RemoveFromStock(aduser, stockUser);
            try
            {
                Repository.Save(_item);
                Telemetry.TrackEvent("ItemOut");
                ToastService.ShowSuccess("Item uit stock gehaald, er zijn nog " + _item.Product.Items.Where(i => i.ItemStatus == ItemStatus.INSTOCK).Count() + " items van dit product.");
            } catch (Exception ex)
            {
                Telemetry.TrackException(ex);
                ToastService.ShowError("Kon item niet uit stock halen.");
                return false;
            }
            return true;
        }

        private async Task<bool> HandleReturn(ADUser stockUser)
        {
            if (_item.ItemStatus != ItemStatus.OUTSTOCK)
            {
                Telemetry.TrackEvent("InvalidIn");
                ToastService.ShowWarning("Item kan niet geretourneerd worden met status: " + _item.ItemStatus.ToString());
                return false;
            }

            _item.ReturnToStock(stockUser);
            if (_isDefective)
            {
                _item.ItemStatus = ItemStatus.DEFECTIVE;
            }
            _item.Comment = _comment;
            try
            {
                Repository.Save(_item);
                _comment = null;
                ToastService.ShowSuccess("Item in stock geplaatst, er zijn " + _item.Product.Items.Where(i => i.ItemStatus == ItemStatus.INSTOCK).Count() + " items van dit product.");
                if (!await _fileUpload.IsEmpty())
                {
                    try
                    {
                        await _fileUpload.Upload("item" + _item?.Id,"item" + _item.Id + DateTime.Now.ToString("ddMMyyyyHHmmss"));
                    }
                    catch (Exception ex)
                    {
                        
                        Telemetry.TrackException(ex);
                        ToastService.ShowWarning("Kon bestand niet uploaden.");
                        return false;
                    }
                }
            } catch (Exception ex)
            {
                Telemetry.TrackException(ex);
                ToastService.ShowError("Kon item niet in stock plaatsen.");
                return false;
            }
            return true;
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


                _item = await Repository.GetItemByProductAndSerialNumberAsync(_productNumber, _serialNumber);            
                if (_item == null)
                {
                    ToastService.ShowWarning("Item bestaat niet.");
                    return;
                }
            }

            if (Method == "out")
            {
                var res = HandleOut(stockUser);
                if (!res)
                {
                    return;
                }

            } else
            {
               var res = await HandleReturn(stockUser);
                if (!res)
                {
                    return;
                }
            }

            _item = null;
            _serialNumber = null;
        }
    }
}
