using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using StorageServices.Factories;

namespace StorageServices
{
    public class BlobStorage : IBlobStorage
    {
        private readonly IBlobContainerFactory _blobContainerFactory;
        private const string ContainerNamePdf = "pdffiles";

        public BlobStorage(IBlobContainerFactory blobContainerFactory)
        {
            _blobContainerFactory = blobContainerFactory;
        }


        public async Task<Uri> UploadFileAsync(Stream fileStream, string blobName)
        {
            try
            {

                await GetContainerAsync();
                var blockBlobClient = _blobContainerFactory.GetBlockBlobClient(blobName, ContainerNamePdf);
                await blockBlobClient.UploadAsync(fileStream);
                return blockBlobClient.Uri;
            }
            catch (Exception e)
            {
                //Log Error 
                throw;
            }

        }


        public async Task<List<BlobItem>> ListFilesAsync()
        {
            try
            {
                var blobs = new List<BlobItem>();
                var blobContainer = await GetContainerAsync();

                var results = blobContainer.GetBlobsAsync().AsPages();

                await foreach (var blobPage in results)
                {
                    blobs.AddRange(blobPage.Values);
                }

                return blobs;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<byte[]> DownloadFileAsync(string blobName)
        {
            try
            {
                var blockBlob = _blobContainerFactory.GetBlockBlobClient(blobName, ContainerNamePdf);
                using (var memoryStream = new MemoryStream())
                {
                    await blockBlob.DownloadToAsync(memoryStream);
                    return memoryStream.ToArray();
                }
            }
            catch (Exception e)
            {
                // Log Error
                throw;
            }

        }

        public async Task<Uri> GetBlobContainerUri()
        {
            try
            {

                var blobContainer = await GetContainerAsync();
                return blobContainer.Uri;
            }
            catch (Exception e)
            {
                // Log Error
                throw;
            }

        }

        private async Task<BlobContainerClient> GetContainerAsync()
        {
            var blobContainerClient = _blobContainerFactory.GetBlobContainerClient(ContainerNamePdf);
            await blobContainerClient.CreateIfNotExistsAsync();

            return blobContainerClient;
        }





    }
}
