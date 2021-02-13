using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StorageServices;
using UploadPdfApi.Model;



namespace UploadPdfApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UploadPdfController : ControllerBase
    {

        private readonly IBlobStorage _blobStorage;

        private const int MaxFileUploadSize = 5242880;
        public UploadPdfController(IBlobStorage uploadPdfStorage)
        {
            _blobStorage = uploadPdfStorage;
        }

        [HttpGet]
        public List<BlobFileInfo> Get()
        {
            var blobContainerUri = _blobStorage.GetBlobContainerUri().Result;
            var blobs = _blobStorage.ListFilesAsync().Result;

            return blobs.Select(blob => new BlobFileInfo
            {
                Name = blob.Name,
                FileSize = blob.Properties.ContentLength,
                Location = new Uri(blobContainerUri, blob.Name).ToString()
            }).ToList();
        }



        [HttpGet("{blobName}")]
        public async Task<IActionResult> Get(string blobName)
        {
            var stream = _blobStorage.DownloadFileAsync(blobName).Result;
            if (stream == null || stream.Length == 0)
            {
                return NotFound("Blob does not exists");
            }

            return File(stream, "application/pdf");

        }


        [HttpPost()]
        public async Task<IActionResult> PostPdf(IFormFile pdfFile)
        {
            if (pdfFile.Length <= 0 || pdfFile.Length > MaxFileUploadSize)
            {
                return BadRequest("Pdf file size cannot be 0");
            }

            if (pdfFile.Length > MaxFileUploadSize)
            {
                return BadRequest("Max Pdf size can be 5MB");
            }

            if (Path.GetExtension(pdfFile.FileName) != ".pdf")
            {
                return BadRequest("Only PDF files can be uploaded");
            }

            var uri = await _blobStorage.UploadFileAsync(pdfFile.OpenReadStream(), pdfFile.FileName);
            return Ok(new { uri });
        }

    }
}
