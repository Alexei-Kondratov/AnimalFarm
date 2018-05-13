using System;
using System.Threading.Tasks;

namespace AnimalFarm.Data
{
    public interface ITransaction : IDisposable
    {
        Task CommitAsync();
    }
}
