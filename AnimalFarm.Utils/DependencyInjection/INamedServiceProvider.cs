using System.Threading.Tasks;

namespace AnimalFarm.Utils.DependencyInjection
{
    public interface INamedServiceProvider
    {
        Task<TService> GetServiceAsync<TService>(string name);
    }
}
