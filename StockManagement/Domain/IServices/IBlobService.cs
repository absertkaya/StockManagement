using Azure.Storage.Blobs.Models;
using Blazor.FileReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Domain.IServices
{
    public interface IBlobService
    {
        Task SetContainer(string containerName);
        Task<List<string>> GetBlobs();
        Task UploadBlobToContainer(IFileReference fileName, string blobName);
        Task DeleteContainer();
        Task DeleteBlob(string blobName);
        Task SetContainerNoCreate(string containerName);
    }
}
