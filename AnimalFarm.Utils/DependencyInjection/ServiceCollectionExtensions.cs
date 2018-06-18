using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace AnimalFarm.Utils.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection Replace<TService, TImplementation>(
        this IServiceCollection services,
        ServiceLifetime lifetime)
        where TService : class
        where TImplementation : class, TService
        {
            var descriptorToRemove = services.FirstOrDefault(d => d.ServiceType == typeof(TService));
            services.Remove(descriptorToRemove);
            var descriptorToAdd = new ServiceDescriptor(typeof(TService), typeof(TImplementation), lifetime);
            services.Add(descriptorToAdd);
            return services;
        }

        public static IServiceCollection ReplaceSingleton<TService>(
        this IServiceCollection services,
        TService singleton)
        where TService : class
        {
            var descriptorToRemove = services.FirstOrDefault(d => d.ServiceType == typeof(TService));
            services.Remove(descriptorToRemove);
            var descriptorToAdd = new ServiceDescriptor(typeof(TService), singleton);
            services.Add(descriptorToAdd);
            return services;
        }
    }
}
