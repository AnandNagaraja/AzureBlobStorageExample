using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;

namespace StorageServices
{
    public interface IBlobStorage
    {
        Task<Uri> UploadFileAsync(Stream fileStream, string blobName);
        Task<List<BlobItem>> ListFilesAsync();

        Task<Uri> GetBlobContainerUri();

        Task<byte[]> DownloadFileAsync(string blobName);
    }
}