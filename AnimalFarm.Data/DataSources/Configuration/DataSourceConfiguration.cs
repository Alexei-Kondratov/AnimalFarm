﻿using AnimalFarm.Utils.Configuration;
using System;
using System.Collections.Generic;

namespace AnimalFarm.Data.DataSources.Configuration
{
    public abstract class DataSourceConfiguration : IComponentConfiguration
    {
        public string Key { get; set; }
        public IEnumerable<Type> Decorators { get; set; }
    }

    public class ReliableStateDataSourceConfiguration : DataSourceConfiguration
    {
    }

    public class DocumentDbDataSourceConfiguration : DataSourceConfiguration
    {
        public string ConnectionInfoName { get; set; }
    }
}
