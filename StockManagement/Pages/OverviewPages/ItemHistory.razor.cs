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
        public int? SupplierId { get; set; }
        [Parameter]
        public string UserId { get; set; }
        [Parameter]
        public string Beheer { get; set; }

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
                _item = await Repository.GetItemDetails(Id);
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

        protected async Task ShowConfirmationDisassociate(Item item)
        {
            var modal = ModalService.Show<Confirmation>("Item uitsluiten");
            var res = await modal.Result;

            if (!res.Cancelled)
            {
                DisassociateItem(item);
            }
        }

        protected void DisassociateItem(Item item)
        {
            _item.RemoveItem(item);
            Repository.Save(_item);
        }

        protected async Task AddIncludedItem()
        {
            var parameters = new ModalParameters();
            parameters.Add("ParentItem", _item);

            var modal = ModalService.Show<AddIncludedItem>("Inbegrepen item", parameters);
            var res = await modal.Result;

            if (!res.Cancelled)
            {
                _item.AddItem((Item)res.Data);
            }
        }

        protected async Task Clear()
        {
            await _fileUpload.ClearFile();
        }

        protected async Task Upload()
        {
            if (await _fileUpload.Upload("item"+ Id + DateTime.Now.ToString("ddMMyyyyHHmmss")))
            {
                await Clear();
                Telemetry.TrackEvent("ItemImageUpload");
                await _blobsComponent.RefreshBlobs();
            }
        }

        protected void NavigateToUser(string id)
        {
            NavigationManager.NavigateTo($"/gebruiker/{id}/{_item.Id}");
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
                item.Supplier?.Items.Remove(item);
                item.Product?.Items.Remove(item);
                Telemetry.TrackEvent("ItemDelete");
                NavigationManager.NavigateTo($"{Beheer}/itemlijst/" + item.Product?.Id);
            }
            catch (Exception ex)
            {
                Telemetry.TrackException(ex);
                ToastService.ShowError("Kon item niet verwijderen.");
            }
        }

        protected async Task ShowConfirmation(Item item)
        {
            var modal = ModalService.Show<Confirmation>("Verwijder item");
            var res = await modal.Result;

            if (!res.Cancelled)
            {
                DeleteItem(item);
            }
        }

    }
}
