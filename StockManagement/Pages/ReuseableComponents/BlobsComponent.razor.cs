using Microsoft.AspNetCore.Components;
using StockManagement.Data.Services;
using StockManagement.Domain.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Pages.ReuseableComponents
{
    public class BlobsComponentBase : ComponentBase
    {
        [Parameter]
        public string Container { get; set; }
        [Inject]
        public IBlobService BlobService { get; set; }
        protected List<string> _uris;

        protected override async Task OnInitializedAsync()
        {
            await BlobService.SetContainer(Container);
            await RefreshBlobs();
        }

        public async Task RefreshBlobs()
        {
            _uris = await BlobService.GetBlobs();
        }

        protected async Task Delete(string uri)
        {
            try
            {
                await BlobService.DeleteBlob(uri);
                _uris.Remove(uri);
            } catch (Exception ex)
            {
                //TODDO
            }

        }
    }
}
