using Microsoft.AspNetCore.Components;
using StockManagement.Domain.IServices;
using System.Linq;
using System.Threading.Tasks;
using Blazor.FileReader;
using Microsoft.ApplicationInsights;
using System;

namespace StockManagement.Pages.ReuseableComponents
{
    public class FileUploadComponentBase : ComponentBase
    {
        [Parameter]
        public string Container { get; set; }

        [Inject]
        public IFileReaderService FileReaderService { get; set; }
        [Inject]
        public TelemetryClient Telemetry { get; set; }

        protected ElementReference inputElement;

        [Inject]
        private IBlobService _service { get; set; }

        public async Task ClearFile()
        {
            await FileReaderService.CreateReference(inputElement).ClearValue();
        }

        public async Task<bool> IsEmpty()
        {
            return (await FileReaderService.CreateReference(inputElement).EnumerateFilesAsync()).ToList().Count == 0;
        }

        public async Task Upload(string container, string filename)
        {
            Container = container;
            await Upload(filename);
        }

        public async Task Upload(string filename)
        {
            var files = (await FileReaderService.CreateReference(inputElement).EnumerateFilesAsync()).ToList();
            foreach (var file in files)
            {
                try
                {
                    var fi = await file.ReadFileInfoAsync();
                    if (fi.Size < 20*1024*1024) {
                        await _service.SetContainer(Container);
                        await _service.UploadBlobToContainer(file, filename);
                    }
                } catch (Exception ex)
                {
                    Telemetry.TrackException(ex);
                }
            }
            await ClearFile();
        }
    }
}
