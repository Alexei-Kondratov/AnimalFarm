using AnimalFarm.Data;
using AnimalFarm.Data.Repositories;
using AnimalFarm.Model;
using System.Threading.Tasks;

namespace AnimalFarm.Logic.RulesetManagement
{
    public class RulesetUnpackingTransformation : IEntityTransformation<Ruleset>
    {
        private RulesetUnpacker _unpacker;

        public RulesetUnpackingTransformation(RulesetUnpacker unpacker)
        {
            _unpacker = unpacker;
        }

        public Task<Ruleset> TransformAsync(ITransaction transaction, Ruleset entity)
        {
            return _unpacker.UnpackAsync(transaction, entity);
        }
    }
}
