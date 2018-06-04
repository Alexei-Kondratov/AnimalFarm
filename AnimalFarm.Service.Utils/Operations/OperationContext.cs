using AnimalFarm.Data;
using AnimalFarm.Service.Utils.Tracing;

namespace AnimalFarm.Service.Utils.Operations
{
    public class OperationContext
    {
        public ITransaction Transaction { get; }
        public ServiceEventSource EventSource { get; }

        public OperationContext(ServiceEventSource eventSource, ITransaction transaction)
        {
            EventSource = eventSource;
            Transaction = transaction;
        }
    }
}
