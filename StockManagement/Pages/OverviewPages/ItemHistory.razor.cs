using Azure.Storage.Blobs.Models;
using Blazored.Modal;
using Blazored.Modal.Services;
using Blazored.Toast.Services;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Components;
using StockManagement.Domain;
using StockManagement.Domain.IRepositories;
using StockManagement.Domain.IServices;
using StockManagement.Pages.ModalComponents;
using StockManagement.Pages.ReuseableComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StockManagement.Pages.OverviewPages
{
    public class ItemHistoryBase : ComponentBase
    {
        [Parameter]
        public int Id { get; set; }

        [Parameter]
        public string? UserId { get; set; }

        [Inject]
        public IItemRepository Repository { get; set; }

        [Inject]
        public IBlobService BlobService { get; set; }

        [Inject]
        public IToastService ToastService { get; set; }

        [Inject]
        public IModalService ModalService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public TelemetryClient Telemetry { get; set; }

        protected FileUploadComponent _fileUpload;
        protected BlobsComponent _blobsComponent;
        protected Item _item;
        protected IList<ItemUser> _itemusers;
        protected bool imageUploadOpen;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                _item = await Repository.GetItemWithUser(Id);
                _itemusers = (await Repository.GetItemUsersByItem(Id))?.OrderByDescending(i => i.ToDate).ToList();
            } catch (Exception ex)
            {
                Telemetry.TrackException(ex);
                ToastService.ShowWarning("Fout bij het inladen van de data, herlaad de pagina.");
            }

            
        }

        protected void ToggleImageUpload()
        {
            imageUploadOpen = !imageUploadOpen;
        }

        protected async Task Clear()
        {
            await _fileUpload.ClearFile();
        }

        protected async Task Upload()
        {
            await _fileUpload.Upload("item"+ Id + DateTime.Now.ToString("ddMMyyyyHHmmss"));
            await Clear();
            Telemetry.TrackEvent("ItemImageUpload");
            NavigationManager.NavigateTo("/itemhistoriek/"+Id, true);
        }

        protected void RowExpand(ItemUser iu)
        {
            var parameters = new ModalParameters();
            parameters.Add("ItemUser", iu);
            var opts = new ModalOptions()
            {
                HideHeader = true
            };

            ModalService.Show<ItemHistoryComponent>("History", parameters, opts);
        }

        private void DeleteItem(Item item)
        {
            try
            {
                Repository.Delete(item);
                item.Supplier.Items.Remove(item);
                item.Product.Items.Remove(item);
                Telemetry.TrackEvent("ItemDelete");
                NavigationManager.NavigateTo("/itemlijst/" + item.Product.Id);
            }
            catch (Exception ex)
            {
                Telemetry.TrackException(ex);
                ToastService.ShowError("Kon item niet verwijderen.");
            }
        }

        protected async Task ShowConfirmation(Item item)
        {
            var modal = ModalService.Show<Confirmation>("Delete Confirmation");
            var res = await modal.Result;

            if (!res.Cancelled)
            {
                DeleteItem(item);
            }
        }

    }
}
