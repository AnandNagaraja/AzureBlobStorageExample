using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StorageServices;
using UploadPdfApi.Controllers;

namespace UplaodPdfApi.Tests
{
    [TestClass]
    public class UploadPdfControllerTests
    {
        private Mock<IBlobStorage> _blobStorage;
        private UploadPdfController _sut;


        [TestInitialize]
        public void Initialize()
        {
            _blobStorage = new Mock<IBlobStorage>();

            _blobStorage.Setup(b => b.GetBlobContainerUri()).Returns(Task.FromResult(new Uri("http://127.0.0.1:10000/devstoreaccount1/pdffile"))).Verifiable();
            _blobStorage.Setup(b => b.ListFilesAsync()).Returns(Task.FromResult(new List<BlobItem>())).Verifiable();
            _blobStorage.Setup(b => b.DownloadFileAsync(It.IsNotNull<string>())).Returns(Task.FromResult(Encoding.ASCII.GetBytes("pdf content"))).Verifiable();

        }

        [TestMethod]
        public void Get_ShouldGet_List_OfBlob_Files()
        {
            _sut = new UploadPdfController(_blobStorage.Object);
            _sut.Get();

            _blobStorage.Verify(v => v.GetBlobContainerUri(), Times.Once);
            _blobStorage.Verify(v => v.ListFilesAsync(), Times.Once);

        }

        [TestMethod]
        public void Get_WithBlobName_Should_Download_File()
        {
            var blobFile = "BlobFile";
            _sut = new UploadPdfController(_blobStorage.Object);
            var result = _sut.Get(blobFile).Result;

            _blobStorage.Verify(v => v.DownloadFileAsync(blobFile), Times.Once);
            Assert.IsNotNull(result);

        }

        [TestMethod]
        public void Get_WithBlobName_Should_Throw_NotFound_WhenSteamIs_Empty()
        {
            var blobFile = "BlobFile";
            _blobStorage.Setup(b => b.DownloadFileAsync(It.IsNotNull<string>())).Returns(Task.FromResult(Encoding.ASCII.GetBytes(""))).Verifiable();
            _sut = new UploadPdfController(_blobStorage.Object);
            var result = (NotFoundObjectResult)_sut.Get(blobFile).Result;

            _blobStorage.Verify(v => v.DownloadFileAsync(blobFile), Times.Once);
            Assert.IsNotNull(result);

            Assert.AreEqual(404, result.StatusCode);
            Assert.AreEqual("Blob does not exists", result.Value);

        }

        [TestMethod]
        public void PostPdf_ShouldUplaodFile()
        {
            var mockFile = new Mock<IFormFile>();
            const string content = "MockFile Content for Blob";
            const string fileName = "BlobUplaodPdfFile.pdf";
            var memoryStream = new MemoryStream();
            var streamWriter = new StreamWriter(memoryStream);
            streamWriter.Write(content);
            streamWriter.Flush();
            memoryStream.Position = 0;
            mockFile.Setup(_ => _.OpenReadStream()).Returns(memoryStream);
            mockFile.Setup(_ => _.FileName).Returns(fileName);
            mockFile.Setup(_ => _.Length).Returns(memoryStream.Length);

            _sut = new UploadPdfController(_blobStorage.Object);
            var result = (OkObjectResult)_sut.PostPdf(mockFile.Object).Result;

            _blobStorage.Verify(v => v.UploadFileAsync(It.IsAny<Stream>(), fileName), Times.Once);

            Assert.IsNotNull(result);

            Assert.AreEqual(200, result.StatusCode);

        }

        [TestMethod]
        public void PostPdf_Should_ThrowBadReqeust_When_FileIsNot_Pdf()
        {
            var mockFile = new Mock<IFormFile>();
            const string content = "MockFile Content for Blob";
            const string fileName = "BlobUplaodPdfFile.txt";
            var memoryStream = new MemoryStream();
            var streamWriter = new StreamWriter(memoryStream);
            streamWriter.Write(content);
            streamWriter.Flush();
            memoryStream.Position = 0;
            mockFile.Setup(_ => _.OpenReadStream()).Returns(memoryStream);
            mockFile.Setup(_ => _.FileName).Returns(fileName);
            mockFile.Setup(_ => _.Length).Returns(memoryStream.Length);

            _sut = new UploadPdfController(_blobStorage.Object);
            var result = (BadRequestObjectResult)_sut.PostPdf(mockFile.Object).Result;

            _blobStorage.Verify(v => v.UploadFileAsync(It.IsAny<Stream>(), fileName), Times.Never);

            Assert.IsNotNull(result);

            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual("Only PDF files can be uploaded", result.Value);

        }


        [TestMethod]
        public void PostPdf_Should_ThrowBadReqeust_When_File_Size_IsEmpty()
        {
            var mockFile = new Mock<IFormFile>();
            const string content = "";
            const string fileName = "BlobUplaodPdfFile.pdf";
            var memoryStream = new MemoryStream();
            var streamWriter = new StreamWriter(memoryStream);
            streamWriter.Write(content);
            streamWriter.Flush();
            memoryStream.Position = 0;
            mockFile.Setup(_ => _.OpenReadStream()).Returns(memoryStream);
            mockFile.Setup(_ => _.FileName).Returns(fileName);
            mockFile.Setup(_ => _.Length).Returns(memoryStream.Length);

            _sut = new UploadPdfController(_blobStorage.Object);
            var result = (BadRequestObjectResult)_sut.PostPdf(mockFile.Object).Result;

            _blobStorage.Verify(v => v.UploadFileAsync(It.IsAny<Stream>(), fileName), Times.Never);

            Assert.IsNotNull(result);

            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual("Pdf file size cannot be 0", result.Value);

        }

    }
}
