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
        public StorageCredentials StorageCredentials { get; set; }
        public CloudStorageAccount StorageAccount { get; set; }
        public CloudBlobClient BlobClient { get; set; }
        public CloudBlobContainer BlobContainer { get; set; }

        public BlobService(IKeyVaultService keyVaultService)
        {
            var accountKey = keyVaultService.GetSecret("BlobStorageAccountKey");
            StorageCredentials = new StorageCredentials("vgdstockmanagement", accountKey);
            StorageAccount = new CloudStorageAccount(StorageCredentials, true);
            BlobClient = StorageAccount.CreateCloudBlobClient();
        }

        public void SetContainerNoCreate(string containerName)
        {
            BlobContainer = BlobClient.GetContainerReference(containerName);
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
            if (await BlobContainer.ExistsAsync())
            {
                List<string> URIs = new List<string>();
                BlobResultSegment resultSegment = await BlobContainer.ListBlobsSegmentedAsync("", true, BlobListingDetails.All, null, null, null, null);
                foreach (var blobitem in resultSegment.Results)
                {
                    URIs.Add(blobitem.StorageUri.PrimaryUri.ToString());
                }
                return URIs;
            }
            return null;
        }

        public async Task DeleteBlob(string uri)
        {
            string blobName = uri.Substring(uri.LastIndexOf("/") + 1);
            CloudBlockBlob blob = BlobContainer.GetBlockBlobReference(blobName);
            await blob.DeleteIfExistsAsync();
        }

        public async Task DeleteContainer()
        {
            if (await BlobContainer.ExistsAsync())
            {
                await BlobContainer.DeleteAsync();
            }
        }
    }
}
