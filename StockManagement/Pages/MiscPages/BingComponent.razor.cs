using Blazor.FileReader;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace StockManagement.Pages.MiscPages
{
    public class BingComponentBase : ComponentBase
    {
        [Inject]
        public IFileReaderService FileReaderService { get; set; }
        protected ElementReference inputElement;
        [Inject]
        public IJSRuntime JSRuntime { get; set; }
        const string CRLF = "\r\n";
        static string BoundaryTemplate = "batch_{0}";
        static string StartBoundaryTemplate = "--{0}";
        static string EndBoundaryTemplate = "--{0}--";
        const string accessKey = "66327faf73744d00b20f9cf2d504726c";
        const string CONTENT_TYPE_HEADER_PARAMS = "multipart/form-data; boundary={0}";
        const string POST_BODY_DISPOSITION_HEADER = "Content-Disposition: form-data; name=\"image\"; filename=\"{0}\"" + CRLF + CRLF;
        const string uriBase = "https://api.cognitive.microsoft.com/bing/v7.0/images/visualsearch";
        protected string json;

        public async Task<bool> Upload()
        {
            var files = (await FileReaderService.CreateReference(inputElement).EnumerateFilesAsync()).ToList();
            foreach (var file in files)
            {
                try
                {
                    var inf = await file.ReadFileInfoAsync();
                    var filename = inf.Name;
                    var imageBinary = (await file.CreateMemoryStreamAsync()).ToArray();
                    var boundary = string.Format(BoundaryTemplate, Guid.NewGuid());
                    var startFormData = BuildFormDataStart(boundary, filename);
                    var endFormData = BuildFormDataEnd(boundary);
                    var contentTypeHdrValue = string.Format(CONTENT_TYPE_HEADER_PARAMS, boundary);
                    json = BingImageSearch(startFormData, endFormData, imageBinary, contentTypeHdrValue);
                    await JSRuntime.InvokeVoidAsync("console.log", json);
                }
                catch (Exception ex)
                {
                    
                }
            }

            return true;
        }

        string BuildFormDataStart(string boundary, string filename)
        {
            var startBoundary = string.Format(StartBoundaryTemplate, boundary);

            var requestBody = startBoundary + CRLF;
            requestBody += string.Format(POST_BODY_DISPOSITION_HEADER, filename);

            return requestBody;
        }

        string BuildFormDataEnd(string boundary)
        {
            return CRLF + CRLF + string.Format(EndBoundaryTemplate, boundary) + CRLF;
        }

        string BingImageSearch(string startFormData, string endFormData, byte[] image, string contentTypeValue)
        {
            WebRequest request = WebRequest.Create(uriBase);
            request.ContentType = contentTypeValue;
            request.Headers["Ocp-Apim-Subscription-Key"] = accessKey;
            request.Method = "POST";

            // Writes the boundary and Content-Disposition header, then writes
            // the image binary, and finishes by writing the closing boundary.
            using (Stream requestStream = request.GetRequestStream())
            {
                StreamWriter writer = new StreamWriter(requestStream);
                writer.Write(startFormData);
                writer.Flush();
                requestStream.Write(image, 0, image.Length);
                writer.Write(endFormData);
                writer.Flush();
                writer.Close();
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponseAsync().Result;
            string json = new StreamReader(response.GetResponseStream()).ReadToEnd();

            return json.Split("##TextRecognition")[2].Split("lines")[1].Split("boundingBox")[0].Substring(13);
        }
    }
}
