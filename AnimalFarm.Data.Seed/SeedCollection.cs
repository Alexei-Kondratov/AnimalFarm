using AnimalFarm.Model;
using System;
using System.Collections.Generic;

namespace AnimalFarm.Data.Seed
{
    public class SeedCollection
    {
        public string Name { get; set; }
        public Type EntityType { get; set; }
        public IEnumerable<IHavePartition<string, string>> Entities { get; set; }
    }
}
