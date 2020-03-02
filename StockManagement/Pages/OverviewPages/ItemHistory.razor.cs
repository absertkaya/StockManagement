using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Components;
using StockManagement.Domain;
using StockManagement.Domain.IRepositories;
using StockManagement.Domain.IServices;
using StockManagement.Pages.ReuseableComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Pages.OverviewPages
{
    public class ItemHistoryBase : ComponentBase
    {
        [Parameter]
        public int Id { get; set; }

        [Inject]
        public IItemRepository Repository { get; set; }
        [Inject]
        public IBlobService BlobService { get; set; }

        protected FileUploadComponent _fileUpload;
        protected BlobsComponent _blobsComponent;

        public int MyProperty { get; set; }

        protected IList<ItemUser> _itemusers;

        protected override async Task OnInitializedAsync()
        {
            _itemusers = (await Repository.GetItemUsersByItem(Id)).OrderByDescending(i => i.ToDate).ToList();
        }

        protected async Task Clear()
        {
            await _fileUpload.ClearFile();
        }

        protected async Task Upload()
        {
            await _fileUpload.Upload("item"+ Id + DateTime.Now.ToString("ddMMyyyyHHmmss"));
            await Clear();
            await _blobsComponent.RefreshBlobs();
        }


    }
}
