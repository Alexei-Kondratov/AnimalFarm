using AnimalFarm.Data.DataSources.Configuration;
using AnimalFarm.Data.Transactions;
using AnimalFarm.Model;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Net;
using System.Threading.Tasks;

namespace AnimalFarm.Data.DataSources
{
    public class DocumentDbDataSource : IDataSource
    {
        private readonly string _databaseId;
        private readonly string _key;
        private readonly Uri _uri;

        public string Name { get; }

        public DocumentDbDataSource(string name, DocumentDbConnectionInfo connectionInfo)
        {
            Name = name;
            _databaseId = connectionInfo.DatabaseName;
            _key = connectionInfo.Key;
            _uri = connectionInfo.Uri;
        }

        private DocumentClient CreateClient()
        {
            return new DocumentClient(_uri, _key, new ConnectionPolicy { EnableEndpointDiscovery = false });
        }

        public TransactionContext CreateTransactionContext()
        {
            return new DocumentDbTransactionContext(CreateClient);
        }

        public async Task<TEntity> ByIdAsync<TEntity>(ITransaction transaction, string storeName, string partitionKey, string entityId)
        {
            var typedContext = (DocumentDbTransactionContext)transaction.GetContext(this);
            Uri documentUri = GetDocumentUri(storeName, entityId);
            var partitionKeyObj = new PartitionKey(partitionKey);

            try
            {
                var result = await typedContext.Client.ReadDocumentAsync<TEntity>(documentUri, new RequestOptions { PartitionKey = partitionKeyObj });
                return result.Document;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == HttpStatusCode.NotFound)
                {
                    return default(TEntity);
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task ApplyOperationAsync(DocumentClient client, DataOperation operation)
        {
            IHavePartition<string, string> entity = operation.Entity;
            var partitionKey = new PartitionKey(entity.PartitionKey);
            var requestOptions = new RequestOptions { PartitionKey = partitionKey };

            switch (operation.Type)
            {
                case DataOperationType.Upsert:
                    Uri collectionUri = GetCollectionUri(operation.StoreName);
                    await client.UpsertDocumentAsync(collectionUri, operation.Entity, requestOptions);
                    break;
                case DataOperationType.Remove:
                    Uri documentUri = GetDocumentUri(operation.StoreName, entity.Id);
                    await client.DeleteDocumentAsync(documentUri, requestOptions);
                    break;
            }
        }

        public async Task AddOperationAsync<TEntity>(ITransaction transaction, DataOperationType operationType, string storeName, TEntity entity)
            where TEntity : IHavePartition<string, string>
        {
            transaction.GetContext(this).AddOperation(operationType, storeName, entity);
        }

        public async Task ComitAsync(ITransaction transaction)
        {
            var typedContext = (DocumentDbTransactionContext)transaction.GetContext(this);
            DocumentClient client = ((DocumentDbTransactionContext)typedContext).Client;
            foreach (DataOperation operation in typedContext.Operations)
                await ApplyOperationAsync(client, operation);
        }

        private Uri GetCollectionUri(string storeName)
        {
            return UriFactory.CreateDocumentCollectionUri(_databaseId, storeName);
        }

        private Uri GetDocumentUri(string storeName, string documentId)
        {
            return UriFactory.CreateDocumentUri(_databaseId, storeName, documentId);
        }
    }
}
