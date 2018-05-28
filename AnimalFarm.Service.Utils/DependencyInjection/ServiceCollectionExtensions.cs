using AnimalFarm.Data;
using AnimalFarm.Data.Repositories.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AnimalFarm.Services.Utils.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRepository<TEntity>(this IServiceCollection services)
        {
            services.AddTransient((provider) => (IRepository<TEntity>)provider.GetRequiredService<RepositoryFactory>().Get(typeof(TEntity)));
            return services;
        }
    }
}
