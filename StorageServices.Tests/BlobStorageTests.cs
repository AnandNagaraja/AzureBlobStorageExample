using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StorageServices;
using StorageServices.Factories;

namespace StorageServices.Tests
{
    [TestClass]
    public class BlobStorageTests
    {
        private IBlobStorage _sut;

        private Mock<IBlobContainerFactory> _mockBlobContainerFactory;
        private Mock<BlockBlobClient> _mockBlockBlobClient;
        private new Mock<BlobContainerClient> _mockBlobContainerClient;
        [TestInitialize]
        public void Initialize()
        {
            _mockBlobContainerClient = new Mock<BlobContainerClient>();
            _mockBlockBlobClient = new Mock<BlockBlobClient>(MockBehavior.Loose, new Uri("http://127.0.0.1:10000/devstoreaccount1/pdffile"), (BlobClientOptions)null);
            _mockBlobContainerFactory = new Mock<IBlobContainerFactory>();



            _mockBlockBlobClient
                //.Setup(c => c.UploadAsync(It.IsAny<Stream>(), It.IsAny<BlobHttpHeaders>(), It.IsAny<IDictionary<string, string>>(), It.IsAny<BlobRequestConditions>(), It.IsAny<AccessTier>(), It.IsAny<IProgress<long>>(), cancellationToken))
                .Setup(c => c.UploadAsync(null, null, null, null, null, null, CancellationToken.None))
                .Verifiable();

            _mockBlobContainerClient.Setup(c => c.CreateIfNotExistsAsync(PublicAccessType.None, It.IsAny<IDictionary<string, string>>(), null, CancellationToken.None)).Verifiable();
            _mockBlobContainerClient.Setup(c => c.GetBlobsAsync(BlobTraits.None, BlobStates.None, null, CancellationToken.None)).Verifiable();
            _mockBlobContainerFactory.Setup(b => b.GetBlockBlobClient(It.IsAny<string>(), It.IsNotNull<string>())).Returns(_mockBlockBlobClient.Object);
            _mockBlobContainerFactory.Setup(b => b.GetBlobContainerClient(It.IsNotNull<string>())).Returns(_mockBlobContainerClient.Object);
        }

        [TestMethod]
        public void UploadFileAsync_ShouldUploadFile()
        {
            _sut = new BlobStorage(_mockBlobContainerFactory.Object);
            _sut.UploadFileAsync(It.IsNotNull<Stream>(), It.IsAny<string>());

            _mockBlockBlobClient.Verify(v => v.UploadAsync(null, null, null, null, null, null, CancellationToken.None), Times.Once);
            _mockBlobContainerFactory.Verify(v => v.GetBlockBlobClient(It.IsAny<string>(), It.IsNotNull<string>()), Times.Once);
            _mockBlobContainerFactory.Verify(v => v.GetBlobContainerClient(It.IsAny<string>()), Times.Once);
            _mockBlobContainerClient.Verify(v => v.CreateIfNotExistsAsync(PublicAccessType.None, It.IsAny<IDictionary<string, string>>(), null, CancellationToken.None), Times.Once);
        }

        [TestMethod]
        public void ListFilesAsync_ShouldReturn_ListOfBlobItem()
        {
            //Todo: not able to Mock GetBlobsAsync

            //_sut = new BlobStorage(_mockBlobContainerFactory.Object);
            //_sut.ListFilesAsync();

            //_mockBlockBlobClient.Verify(v => v.UploadAsync(null, null, null, null, null, null, CancellationToken.None), Times.Once);
            //_mockBlobContainerFactory.Verify(v => v.GetBlockBlobClient(It.IsAny<string>(), It.IsNotNull<string>()), Times.Once);
        }

        [TestMethod]
        public void GetBlobContainerUri_ShouldReturnUri()
        {
            _sut = new BlobStorage(_mockBlobContainerFactory.Object);
            _sut.GetBlobContainerUri();

            _mockBlobContainerFactory.Verify(v => v.GetBlobContainerClient(It.IsAny<string>()), Times.Once);
            _mockBlobContainerClient.Verify(V => V.CreateIfNotExistsAsync(PublicAccessType.None, It.IsAny<IDictionary<string, string>>(), null, CancellationToken.None), Times.Once);
        }


        [TestMethod]
        public void DownloadFileAsync_ShouldReturBlobItem_ForDownload()
        {
            _sut = new BlobStorage(_mockBlobContainerFactory.Object);
            _sut.DownloadFileAsync(It.IsAny<string>());

            _mockBlobContainerFactory.Verify(v => v.GetBlockBlobClient(It.IsAny<string>(), It.IsNotNull<string>()), Times.Once);
        }

    }
}
