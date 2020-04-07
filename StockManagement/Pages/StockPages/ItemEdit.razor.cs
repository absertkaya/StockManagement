using Blazored.Toast.Services;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
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
        private IUserRepository UserRepository { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public IToastService ToastService { get; set; }
        [Inject]
        public TelemetryClient Telemetry { get; set; }

        [Parameter]
        public int Id { get; set; }

        protected Item _item;

        protected IList<Category> _categories;
        protected IList<Product> _products;
        protected IList<Supplier> _suppliers;
        protected IList<ADUser> _users;

        protected FileUploadComponent _fileUpload;

        protected int? _selectedCategory;
        protected int? _selectedProduct;
        protected int? _selectedSupplier;
        protected string _comment;
        protected string _serialNumber;
        protected DateTime? _deliveryDate;
        protected DateTime? _invoiceDate;
        protected ItemStatus _selectedStatus;

        protected string _imei;
        protected string _vgdnumber;
        protected string _license;
        protected string _hostname;
        protected string _carepack;

        [CascadingParameter]
        protected Task<AuthenticationState> AuthenticationStateTask { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                _categories = await Repository.GetAllAsync<Category>();
                _suppliers = await Repository.GetAllAsync<Supplier>();
                _item = (Item)await Repository.GetByIdAsync(typeof(Item), Id);

                if (_item == null)
                {
                    NavigationManager.NavigateTo("error");
                }
                else
                {
                    _selectedCategory = _item.Product.Category.Id;
                    _serialNumber = _item.SerialNumber;
                    await FetchProducts();
                    _selectedSupplier = _item.Supplier?.Id;
                    _comment = _item.Comment;
                    _deliveryDate = _item.DeliveryDate;
                    _invoiceDate = _item.InvoiceDate;
                    _selectedStatus = _item.ItemStatus;
                    _imei = _item.Imei;
                    _vgdnumber = _item.Imei;
                    _license = _item.License;
                    _hostname = _item.Hostname;
                    _carepack = _item.Carepack;
                }
            } catch (Exception ex)
            {
                Telemetry.TrackException(ex);
                ToastService.ShowWarning("Fout bij het inladen van de data, herlaad de pagina.");
            }
        }
        
        public async Task FireChange(ChangeEventArgs e)
        {
            _selectedCategory = int.Parse(e.Value.ToString());
            await FetchProducts();
        }

        public async Task FetchProducts()
        {
            try
            {
                if (_selectedCategory != null)
                {
                    _products = await Repository.GetByCategoryAsync((int)_selectedCategory);
                    _selectedProduct = _item.Product?.Id;
                }
            } catch (Exception ex)
            {
                Telemetry.TrackException(ex);
                ToastService.ShowWarning("Fout bij het inladen van de producten, herlaad de pagina.");
            }

        }

        protected async Task Submit()
        {
            if (string.IsNullOrWhiteSpace(_serialNumber))
            {
                ToastService.ShowWarning("Serienummer mag niet leeg zijn.");
                return;
            }
            _item.SerialNumber = Regex.Replace(_serialNumber, @"\s+", "");

            if (_selectedProduct == null)
            {
                ToastService.ShowWarning("Selecteer een product.");
                return;
            }
            _item.Product = _products.First(i => _selectedProduct == i.Id);
            if (_selectedSupplier == null)
            {
                ToastService.ShowWarning("Selecteer een leverancier.");
                return;
            }


            _item.Supplier = _suppliers.First(i => _selectedSupplier == i.Id);
            _item.Comment = _comment;
            _item.DeliveryDate = _deliveryDate;
            _item.InvoiceDate = _invoiceDate;
            _item.ItemStatus = _selectedStatus;
            _item.Carepack = string.IsNullOrWhiteSpace(_carepack) ? null : _carepack;
            _item.Hostname = string.IsNullOrWhiteSpace(_hostname) ? null : _hostname;
            _item.Imei = string.IsNullOrWhiteSpace(_imei) ? null : _imei;
            _item.License = string.IsNullOrWhiteSpace(_license) ? null : _license;
            _item.VGDNumber = string.IsNullOrWhiteSpace(_vgdnumber) ? null : _vgdnumber;
            try
            {
                Repository.Save(_item);
            } catch (Exception ex)
            {
                Telemetry.TrackException(ex);
                ToastService.ShowError("Kon item niet opslaan.");
            }

            if (!await _fileUpload.IsEmpty())
            {
                try
                {
                    await Upload(_item);
                }
                catch (Exception ex)
                {
                    Telemetry.TrackException(ex);
                    ToastService.ShowError("Kon foto niet opslaan.");
                }
            }
            Telemetry.TrackEvent("ItemUpdate");
            ToastService.ShowSuccess("Item succesvol geëditeerd.");
            NavigationManager.NavigateTo("itemhistoriek/" + _item.Id);
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
