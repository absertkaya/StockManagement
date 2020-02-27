using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Data.Services
{
    public class BlobService
    {
        [Inject]
        public IConfiguration Configuration { get; set; }

        public BlobServiceClient ServiceClient { get; set; }
        public BlobContainerClient ContainerClient { get; set; }

        public BlobService()
        {
            ServiceClient = new BlobServiceClient(Configuration.GetConnectionString("AzureBlobStorage"));
        }

        public async Task CreateBlobContainerClient(string containerName)
        {
            ContainerClient = await ServiceClient.CreateBlobContainerAsync(containerName);
        }

        public async Task UploadBlobToContainer(string filename)
        {
            BlobClient blobClient = ContainerClient.GetBlobClient(filename);
        }

        public async Task DownloadBlobFromContainer(Stream fileStream)
        {
            throw new NotImplementedException();
        }
    }
}
