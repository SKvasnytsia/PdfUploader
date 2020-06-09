using Microsoft.Azure.Documents;
using PDFUploader.Domain.Entities;
using PDFUploader.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace PDFUploader.Data.Repositories
{
    public class PdfMetadataRepository: CosmosDbRepository<PdfMetadata>, IPdfMetadataRepository
    {
        public PdfMetadataRepository(ICosmosDbClientFactory factory) : base(factory) { }

        public override string CollectionName { get; } = "pdfMetadatas";
        public override string GenerateId(PdfMetadata entity) => $"pdfs:{Guid.NewGuid()}";
        public override PartitionKey ResolvePartitionKey(string entityId) => new PartitionKey(entityId.Split(':')[0]);
    }
}
