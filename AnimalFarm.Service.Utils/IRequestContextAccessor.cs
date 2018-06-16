
namespace AnimalFarm.Service.Utils
{
    public interface IRequestContextAccessor
    {
        RequestContext Context { get; }
    }
}
