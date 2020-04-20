using Blazor.FileReader;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using ZXing;

namespace StockManagement.Pages.ReuseableComponents
{
    public class ZXingScannerBase : ComponentBase
    {
        public string Result { get; set; }

        [Inject]
        public IFileReaderService FileReader { get; set; }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }
        public ElementReference input { get; set; }

        public string res { get; set; }



        public async Task Decode()
        {
            var files = (await FileReader.CreateReference(input).EnumerateFilesAsync()).ToList();
            foreach (var file in files)
            {
                int bufferSize = 4096;
                int receivedBytes = 0;
                using (var stream = await file.OpenReadAsync())
                {
                    var client = new HttpClient();
                    var queryString = HttpUtility.ParseQueryString(string.Empty);
                    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "{66327faf73744d00b20f9cf2d504726c}");

                    queryString["language"] = "unk";
                    queryString["detectOrientation"] = "true";
                    var uri = "https://stockmanagementvisualsearch.cognitiveservices.azure.com/bing/v7.0" + queryString;

                    using (MemoryStream ms = new MemoryStream())
                    {
                        byte[] buffer = new byte[bufferSize];
                        int read = 0;
                        while ((read = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            ms.Write(buffer, 0, read);

                            receivedBytes += read;
                        }
                        
                        using (var image = Image.Load(ms.ToArray()))
                        {
                            var reader = new ZXing.ImageSharp.BarcodeReader<SixLabors.ImageSharp.PixelFormats.Rgba32>();

                            res = reader.Decode(image)?.Text;
                        }

                        if (res == null)
                        {

                        }
                    }
                    


                }
            }
        }
    }
}
