using PDFUploader.Domain.Entities;
using System.IO;
using System.Threading.Tasks;

namespace PDFUploader.Domain.Interfaces.Blobs
{
    public interface IBlobStorage
    {
        Task<string> UploadAsync(string filename, Stream content);
        Task<PdfMetadata[]> GetFileCollectionAsync();
        Task<string> GetDownloadLinkAsync(string filename);

        Task<bool> DeleteAsync(string fileName);
    }
}
