using Microsoft.ApplicationInsights;
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
        [Inject]
        public TelemetryClient Telemetry { get; set; }
        protected List<string> _uris = new List<string>();

        protected override async Task OnInitializedAsync()
        {
            await BlobService.SetContainerNoCreate(Container);
            await RefreshBlobs();
        }

        public async Task AddUri(string uri)
        {
            _uris.Add(uri);
            await RefreshBlobs();
        }

        public async Task RefreshBlobs()
        {
            try
            {
                List<string> uris = await BlobService.GetBlobs();
                if (uris != null)
                {
                    _uris = uris;
                }
            } catch (Exception ex)
            {
                Telemetry.TrackException(ex);
            }
            
        }

        protected async Task Delete(string uri)
        {
            try
            {
                await BlobService.DeleteBlob(uri);
                _uris.Remove(uri);

            } catch (Exception ex)
            {
                Telemetry.TrackException(ex);
            }

        }
    }
}
