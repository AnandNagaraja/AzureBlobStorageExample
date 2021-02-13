using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;

namespace StorageServices.Factories
{
    public interface IBlobContainerFactory
    {
        BlockBlobClient GetBlockBlobClient(string blobName, string containerNamePdf);
        BlobContainerClient GetBlobContainerClient(string containerNamePdf);
    }
}