using AnimalFarm.Data;
using AnimalFarm.Service.Utils.Tracing;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AnimalFarm.Service.Utils.Operations
{
    public class OperationContext
    {
        private OperationRunner _runner; 

        public ITransaction Transaction { get; }
        public ServiceEventSource EventSource { get; }
        public CancellationToken CancellationToken { get; }

        internal OperationContext(OperationRunner runner, ServiceEventSource eventSource, ITransaction transaction, CancellationToken cancellationToken)
        {
            _runner = runner;
            EventSource = eventSource;
            Transaction = transaction;
            CancellationToken = cancellationToken;
        }

        public async Task RunSuboperationAync(Func<OperationContext, Task> subOperation)
        {
            _runner.RunAsync(subOperation, this);
        }
    }
}
