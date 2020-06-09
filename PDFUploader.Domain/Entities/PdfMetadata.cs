namespace PDFUploader.Domain.Entities
{
    public class PdfMetadata: Entity
    {
        public string Name { get; set; }

        public string Location { get; set; }

        public string FileSize { get; set; }
    }
}
