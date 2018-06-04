using System;
using System.Threading;
using System.Threading.Tasks;
using AnimalFarm.Data;
using AnimalFarm.Service.Utils.Tracing;
using Microsoft.Extensions.DependencyInjection;

namespace AnimalFarm.Service.Utils.Operations
{
    public class OperationRunner
    {
        private readonly ServiceEventSource _eventSource;
        private readonly IServiceProvider _serviceProvider;
        private readonly ITransactionManager _transactionManager;

        public OperationRunner(ServiceEventSource eventSource, IServiceProvider serviceProvider, ITransactionManager transactionManager)
        {
            _eventSource = eventSource;
            _serviceProvider = serviceProvider;
            _transactionManager = transactionManager;
        }

        private OperationContext GetNewContext()
        {
            return new OperationContext(_eventSource, _transactionManager.CreateTransaction());
        }

        public async Task RunAsync(Operation operation, CancellationToken cancellationToken)
        {
            int retriesCount = 0;

            string operationRunId = Guid.NewGuid().ToString();
            operation.Context.EventSource.Message("Starting operation {0}", operationRunId);

            while (true && !cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await operation.Delegate(operation.Context);
                    operation.Context.EventSource.Message("Ending operation {0}", operationRunId);
                    return;
                }
                catch (Exception ex)
                {
                    operation.Context.EventSource.Message("Operation {0} failed: {1}", operationRunId, ex.Message);

                    retriesCount++;
                    if (retriesCount >= operation.Retries)
                    {
                        if (operation.IsCritical)
                            throw;
                        else
                            return;
                    }

                    operation.Context.EventSource.Message("Retrying operation {0}", operationRunId);
                }
            }
        }

        public async Task RunAsync<TType1, TType2>(Func<OperationContext, TType1, TType2, Task> operationMethod, CancellationToken cancellationToken)
        {
            var operation = new Operation
            {
                Delegate = (context) => operationMethod.Invoke(context, _serviceProvider.GetService<TType1>(), _serviceProvider.GetService<TType2>()),
                Context = GetNewContext(),
                Retries = 4,
                IsCritical = false
            };

            await RunAsync(operation, cancellationToken);
        }
    }
}
