using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PDFUploader.API.CrossCutting;
using PDFUploader.Domain.Interfaces.Blobs;

namespace PDFUploader.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PdfController : ControllerBase
    {
        private const int MAX_FILE_SIZE = 5 * 1048576;
        private const string _allowedExtension = ".pdf";

        private readonly IBlobStorage _blobStorage;
        private readonly ILogger<PdfController> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blobStorage"></param>
        /// <param name="logger"></param>
        public PdfController(
            IBlobStorage blobStorage,
            ILogger<PdfController> logger)
        {
            _blobStorage = blobStorage ?? throw new ArgumentNullException(nameof(IBlobStorage));
            _logger = logger ?? throw new ArgumentNullException(nameof(ILogger<PdfController>));
        }


        /// <summary>
        /// Get pdf files collection from blob storage
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _blobStorage.GetFileCollectionAsync();
            return Ok(result);
        }
        /// <summary>
        /// Uploads non-existing pdf file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost(nameof(Upload))]
        [MaxFileSize(MAX_FILE_SIZE)]
        [AllowedExtensions(_allowedExtension)]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            _logger.LogInformation("Uploading file");

            // The FileName property should only be used for display purposes but for demo have used it as identifier
            //The maximum length of a blob name in the storage emulator is 256 characters, while the maximum length of a blob name in Azure Storage is 1024 characters.
            if (file != null && file.Length > 0)
            {
                var blobURL = await _blobStorage.UploadAsync(file.FileName, file.OpenReadStream());

                return Ok(blobURL);
            }
            else
            {
                return BadRequest("Invalid file");
            }
        }
        /// <summary>
        /// Downloads existing pdf file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpPost(nameof(Download))]
        public async Task<IActionResult> Download([FromBody]string fileName)
        {
            _logger.LogInformation($"Downloading file {fileName}");

            var result = await _blobStorage.GetDownloadLinkAsync(fileName);
            if (result == null)
            {
                return BadRequest($"File {fileName} is not found");
            }
            return Ok(result);
        }

        /// <summary>
        /// Deletes existing pdf file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody]string fileName)
        {
            _logger.LogInformation($"Removing file {fileName}");

            var isDeleted =  await _blobStorage.DeleteAsync(fileName);
            if (isDeleted)
            {
                return Ok();
            }
            else
            {
                return BadRequest($"File {fileName} is not found");
            }
        }
    }
}
