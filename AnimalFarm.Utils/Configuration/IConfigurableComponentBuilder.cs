using System;

namespace AnimalFarm.Utils.Configuration
{
    public interface IConfigurableComponentBuilder<TComponent, TKey>
    {
        bool CanBuild(IComponentConfiguration<TKey> configuration);

        TComponent Build(IComponentConfiguration<TKey> configuration, IServiceProvider serviceContainer);
    }
}
