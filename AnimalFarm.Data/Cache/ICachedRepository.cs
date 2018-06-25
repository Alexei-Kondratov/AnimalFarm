using System.Threading.Tasks;

namespace AnimalFarm.Data.Cache
{
    /// <summary>
    /// Represents a repository with a cache.
    /// </summary>
    public interface ICachedRepository
    {
        /// <summary>
        /// Clears the repository's cache.
        /// </summary>
        Task ClearCacheAsync();
    }
}
