using Microsoft.Azure.Documents.Client;
using System;

namespace AnimalFarm.Data.Transactions
{
    public class DocumentDbTransactionContext : TransactionContext
    {
        private readonly Lazy<DocumentClient> _client;

        public DocumentClient Client => _client.Value;

        public DocumentDbTransactionContext(Func<DocumentClient> clientInitializer)
        {
            _client = new Lazy<DocumentClient>(clientInitializer, true);
        }

        public override void Dispose()
        {
            base.Dispose();

            if (_client.IsValueCreated)
                _client.Value.Dispose();
        }
    }
}
