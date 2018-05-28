using AnimalFarm.Data;
using AnimalFarm.Data.DataSources;
using AnimalFarm.Model;
using System;
using System.Threading.Tasks;

namespace AnimalFarm.Logic.RulesetManagement
{
    public class RulesetUnpackingDecorator : DataSourceDecoratorBase
    {
        private Lazy<RulesetUnpacker> _unpacker;

        public RulesetUnpackingDecorator(IDataSource internalImplementation, Lazy<RulesetUnpacker> unpacker)
            : base (internalImplementation)
        {
            _unpacker = unpacker;
        }

        public override async Task<TEntity> ByIdAsync<TEntity>(ITransaction transaction, string storeName, string partitionKey, string entityId)
        {
            TEntity result = await base.ByIdAsync<TEntity>(transaction, storeName, partitionKey, entityId);
            if (result is Ruleset)
                result = (TEntity)(object)(await _unpacker.Value.UnpackAsync(transaction, result as Ruleset));

            return result;
        }
    }
}
