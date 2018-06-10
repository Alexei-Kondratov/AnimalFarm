using System.Collections.Generic;

namespace AnimalFarm.Data.Seed
{
    public interface ISeedData
    {
        IEnumerable<SeedCollection> Collections { get; }
    }
}