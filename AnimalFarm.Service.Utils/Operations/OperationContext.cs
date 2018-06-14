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
        private CancellationTokenSource _cancellationTokenSource;

        public ITransaction Transaction { get; }
        public ServiceEventSource EventSource { get; }
        public CancellationToken CancellationToken { get; }

        internal OperationContext(OperationRunner runner, ServiceEventSource eventSource, ITransaction transaction, CancellationToken? cancellationToken = null)
        {
            _runner = runner;
            EventSource = eventSource;
            Transaction = transaction;

            _cancellationTokenSource = cancellationToken.HasValue ?
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken.Value)
                : new CancellationTokenSource();
            CancellationToken = _cancellationTokenSource.Token;
        }

        public void Cancel()
        {
            _cancellationTokenSource.Cancel();
        }

        public Task RunSuboperationAync(Func<OperationContext, Task> subOperation)
        {
            return _runner.RunAsync(subOperation, this);
        }
    }
}
