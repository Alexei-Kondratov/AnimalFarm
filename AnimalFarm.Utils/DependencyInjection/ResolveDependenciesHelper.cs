using System;
using System.Linq;
using System.Reflection;

namespace AnimalFarm.Utils.DependencyInjection
{
    public static class ResolveDependenciesHelper
    {
        private static Lazy<TType> LazyInitializer<TType>(IServiceProvider services)
        {
            return new Lazy<TType>(() => (TType)services.GetService(typeof(TType)), true);
        }

        private static object FetchParameter(Type type, IServiceProvider services, object[] additionalServices)
        {
            var value = services.GetService(type);
            if (value != null)
                return value;

            foreach (object obj in additionalServices)
            {
                if (type.IsAssignableFrom(obj.GetType()))
                    return obj;
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Lazy<>))
            {
                Type valueType = type.GetGenericArguments()[0];
                return typeof(ResolveDependenciesHelper).GetMethod(nameof(LazyInitializer), BindingFlags.NonPublic | BindingFlags.Static)
                    .MakeGenericMethod(new[] { valueType })
                    .Invoke(null, new[] { services});
            }

            return null;
        }

        private static object[] FetchParameters(ConstructorInfo constructorInfo, IServiceProvider services, object[] additionalServices)
        {
            object[] result = constructorInfo.GetParameters().Select(p => FetchParameter(p.ParameterType, services, additionalServices))
                .ToArray();

            if (result.Any(obj => obj == null))
                return null;

            return result;
        }

        public static TType Instantiate<TType>(IServiceProvider services, params object[] additionalServices)
        {
            return (TType)Instantiate(typeof(TType), services, additionalServices);
        }

        public static object Instantiate(Type type, IServiceProvider services, params object[] additionalServices)
        {
            foreach (ConstructorInfo constructorInfo in type.GetConstructors())
            {
                var parameters = FetchParameters(constructorInfo, services, additionalServices);
                if (parameters != null)
                    return constructorInfo.Invoke(parameters);
            }

            throw new ArgumentException($"Cannot resolve dependencies for {type.FullName}");
        }
    }
}
