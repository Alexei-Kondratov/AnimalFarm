using System;
using System.Threading.Tasks;

namespace AnimalFarm.Service.Utils.Operations
{
    public class Operation
    {
        public int Retries { get; set; }
        public bool IsCritical { get; set; }
        public OperationContext Context { get; set; }
        public Func<OperationContext, Task> Delegate { get; set; }
    }
}
