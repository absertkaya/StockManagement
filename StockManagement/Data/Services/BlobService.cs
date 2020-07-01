using Blazor.FileReader;
using StockManagement.Domain.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Data.Services
{
    public class BlobService : IBlobService
    {
        public Task DeleteBlob(string blobName)
        {
            throw new NotImplementedException();
        }

        public Task DeleteContainer()
        {
            throw new NotImplementedException();
        }

        public Task<List<string>> GetBlobs()
        {
            throw new NotImplementedException();
        }

        public Task SetContainer(string containerName)
        {
            throw new NotImplementedException();
        }

        public Task SetContainerNoCreate(string containerName)
        {
            throw new NotImplementedException();
        }

        public Task UploadBlobToContainer(IFileReference fileName, string blobName)
        {
            throw new NotImplementedException();
        }
    }
}
