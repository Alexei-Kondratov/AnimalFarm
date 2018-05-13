using Microsoft.ServiceFabric.Data;
using System;

namespace AnimalFarm.Data
{
    public interface IReliableStateTransaction : ITransaction
    {
        Microsoft.ServiceFabric.Data.ITransaction Object { get; }
    }
}
