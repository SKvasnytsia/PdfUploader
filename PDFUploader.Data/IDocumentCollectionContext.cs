using Microsoft.Azure.Documents;
using PDFUploader.Domain.Entities;
using System;

namespace PDFUploader.Data
{
    public interface IDocumentCollectionContext<in T> where T : Entity
    {
        string CollectionName { get; }

        string GenerateId(T entity);

        PartitionKey ResolvePartitionKey(string entityId);
    }
}