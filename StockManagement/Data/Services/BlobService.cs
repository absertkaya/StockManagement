using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Blazor.FileReader;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using StockManagement.Domain.IServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Data.Services
{
    public class BlobService : IBlobService
    {
        [Inject]
        public IConfiguration Configuration { get; set; }
        public StorageCredentials StorageCredentials { get; set; }
        public CloudStorageAccount StorageAccount { get; set; }
        public CloudBlobClient BlobClient { get; set; }
        public CloudBlobContainer BlobContainer { get; set; }

        public BlobService()
        {
            StorageCredentials = new StorageCredentials("vgdstockmanagement", "0AuSjGeH2gUSYWJ3Pzfsfu4qqqtPN9/T0IGnczh4zdycVLUSlZWn2qoIqiFpfyyDJqB5Cd6aeIFt2jXEjQ5ajg==");
            StorageAccount = new CloudStorageAccount(StorageCredentials, true);
            BlobClient = StorageAccount.CreateCloudBlobClient();
        }

        public async Task SetContainer(string containerName)
        {          
            BlobContainer = BlobClient.GetContainerReference(containerName);
            if (! await BlobContainer.ExistsAsync())
            {
                await BlobContainer.CreateAsync();
                BlobContainerPermissions permissions = await BlobContainer.GetPermissionsAsync();
                permissions.PublicAccess = BlobContainerPublicAccessType.Blob;
                await BlobContainer.SetPermissionsAsync(permissions);
            }
        }


        public async Task UploadBlobToContainer(IFileReference file, string blobName)
        {
            CloudBlockBlob blob = BlobContainer.GetBlockBlobReference(blobName);
            using (Stream uploadFileStream =  await file.OpenReadAsync())
            {
                await blob.UploadFromStreamAsync(uploadFileStream);
            }
        }

        public async Task<List<string>> GetBlobs()
        {
            List<string> URIs = new List<string>();
            BlobResultSegment resultSegment = await BlobContainer.ListBlobsSegmentedAsync("", true, BlobListingDetails.All, null, null, null, null);
            foreach (var blobitem in resultSegment.Results)
            {
                URIs.Add(blobitem.StorageUri.PrimaryUri.ToString());
            }
            return URIs;
        }

        public async Task DeleteBlob(string uri)
        {
            string blobName = uri.Substring(uri.LastIndexOf("/") + 1);
            CloudBlockBlob blob = BlobContainer.GetBlockBlobReference(blobName);
            await blob.DeleteIfExistsAsync();

        }
    }
}
