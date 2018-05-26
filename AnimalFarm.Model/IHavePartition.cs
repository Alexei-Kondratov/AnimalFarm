using System;
using System.Collections.Generic;
using System.Text;

namespace AnimalFarm.Model
{
    /// <summary>
    /// Describes an entity that supports partitioning for data storage and processing.
    /// </summary>
    public interface IHavePartition<TPartitionId, TId> : IHaveId<TId>
    {
        /// <summary>
        /// Gets the entity's key used for partitioning.
        /// </summary>
        string PartitionKey { get; }
    }
}
