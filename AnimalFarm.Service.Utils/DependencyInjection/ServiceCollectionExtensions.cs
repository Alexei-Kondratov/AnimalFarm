using AnimalFarm.Data;
using AnimalFarm.Utils.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace AnimalFarm.Services.Utils.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRepository<TEntity>(this IServiceCollection services)
        {
            services.AddTransient((provider) =>
                (IRepository<TEntity>)provider.GetRequiredService<INamedServiceProvider>().GetServiceAsync<IRepository<TEntity>>(typeof(TEntity).Name)
            );
            return services;
        }
    }
}
