using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;

namespace StorageServices.Factories
{
    public class BlobContainerFactory : IBlobContainerFactory
    {
        private readonly string _connectionString;

        public BlobContainerFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public BlockBlobClient GetBlockBlobClient(string blobName, string containerNamePdf)
        {
            return new BlockBlobClient(_connectionString, containerNamePdf, blobName);
        }

        public BlobContainerClient GetBlobContainerClient(string containerNamePdf)
        {
            return new BlobContainerClient(_connectionString, containerNamePdf);
        }

    }
}
