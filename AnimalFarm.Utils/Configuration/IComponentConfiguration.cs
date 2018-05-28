
namespace AnimalFarm.Utils.Configuration
{
    public interface IComponentConfiguration<TKey>
    {
        TKey Key { get; }
    }
}
