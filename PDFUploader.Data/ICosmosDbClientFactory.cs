using System;
using System.Collections.Generic;
using System.Text;

namespace PDFUploader.Data
{
    public interface ICosmosDbClientFactory
    {
        ICosmosDbClient GetClient(string collectionName);
    }
}
