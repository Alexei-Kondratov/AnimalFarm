using AnimalFarm.Data;
using AnimalFarm.Data.Repositories;
using AnimalFarm.Model;
using Microsoft.ServiceFabric.Data;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AnimalFarm.Service.Utils
{
    /// <summary>
    /// Responsible for building entity repositories based on configuraiton.
    /// </summary>
    public class RepositoryBuilder
    {
        private readonly CloudStorageConnector _azureConnector;
        private readonly string _configurationFilePath;
        private readonly IReliableStateManager _reliableStateManager;


        public RepositoryBuilder(string configurationFilePath, IReliableStateManager reliableStateManager)
        {
            _configurationFilePath = configurationFilePath;
            _reliableStateManager = reliableStateManager;
        }

        public RepositoryBuilder(ServiceContext context, IReliableStateManager reliableStateManager, CloudStorageConnector azureConnector)
        {
            _azureConnector = azureConnector;
            _configurationFilePath = GetDefaultConfigurationFilePath(context);
            _reliableStateManager = reliableStateManager;
        }

        private string GetDefaultConfigurationFilePath(ServiceContext context)
        {
            var configPath = context.CodePackageActivationContext.GetConfigurationPackageObject("Config").Path;
            return Path.Combine(configPath, @"RepositoriesConfig.json");
        }

        private RepositoryConfiguration GetRepositoryConfiguration<TEntity>()
        {
            var configContent = File.ReadAllText(_configurationFilePath);
            var configs = JsonConvert.DeserializeObject<RepositoryConfiguration[]>(configContent, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto });
            var config = configs.FirstOrDefault(c => c.EntityType == typeof(TEntity).FullName);
            return config;
        }

        private IRepository<TEntity> BuildAzureTableRepository<TEntity>(AzureTableRepositoryConfiguration config)
            where TEntity : TableEntity
        {
            return new AzureTableRepository<TEntity>(_azureConnector, config.TableName);
        }

        private IRepository<TEntity> BuildReadOnlyProxyRepository<TEntity>(ReadOnlyProxyRepositoryConfiguration config)
        {
            if (!Enum.TryParse(config.SourceService, out ServiceType service))
                throw new KeyNotFoundException($"Uknown ServiceType {config.SourceService}");

            return new ReadOnlyProxyRepository<TEntity>(service, config.SourceEndpointPath);
        }

        private IRepository<TEntity> BuildReliableStateepository<TEntity>(RepositoryConfiguration config)
            where TEntity : class, IHaveId<string>
        {
            return new ReliableStateRepository<TEntity>(_reliableStateManager, config.LocalCacheName);
        }

        private string GetBuilderMethodNameForConfig(RepositoryConfiguration config)
        {
            if (config is AzureTableRepositoryConfiguration)
                return nameof(BuildAzureTableRepository);

            if (config is ReadOnlyProxyRepositoryConfiguration)
                return nameof(BuildReadOnlyProxyRepository);

            throw new KeyNotFoundException($"No builders are defined for {config.GetType().Name} configuration.");
        }

        private IRepository<TEntity> BuildRepository<TEntity>(string builderMethodName, RepositoryConfiguration config)
        {
            MethodInfo builderMethod
                = GetType().GetMethod(builderMethodName, BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(typeof(TEntity));

            var result = builderMethod.Invoke(this, new[] { config });
            return (IRepository<TEntity>)result;
        }

        private IRepository<TEntity> AddReliableCache<TEntity>(IRepository<TEntity> repository, RepositoryConfiguration config)
        {
            var cacheRepository = BuildRepository<TEntity>(nameof(BuildReliableStateepository), config);
            return new CachedRepository<TEntity>(cacheRepository, repository);
        }

        /// <summary>
        /// Creates a repository for the TEntity type based on the configuration.
        /// </summary>
        public IRepository<TEntity> BuildRepository<TEntity>(IEntityTransformation<TEntity> transformation = null)
        {
            RepositoryConfiguration config = GetRepositoryConfiguration<TEntity>();
            if (config == null)
                throw new KeyNotFoundException($"No configuration defined for {typeof(TEntity).FullName} repository");

            string builderMethod = GetBuilderMethodNameForConfig(config);
            IRepository<TEntity> result = BuildRepository<TEntity>(builderMethod, config);

            if (transformation != null)
                result = new TransformingRepositoryDecorator<TEntity>(result, transformation);

            if (!String.IsNullOrEmpty(config.LocalCacheName))
                result = AddReliableCache(result, config);

            return result;
        }
    }
}
