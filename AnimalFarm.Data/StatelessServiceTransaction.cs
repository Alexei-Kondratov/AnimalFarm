using System.Threading.Tasks;

namespace AnimalFarm.Data
{
    /// <summary>
    /// ITransaction implementation suitable for stateless services.
    /// </summary>
    public class StatelessServiceTransaction : IAzureTableTransaction
    {
        public StatelessServiceTransaction()
        {
        }

        public async Task CommitAsync()
        {
        }

        public void Dispose()
        {
        }
    }
}
