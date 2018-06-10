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

        private OperationContext GetNewContext(CancellationToken cancellationToken)
        {
            return new OperationContext(this, _eventSource, _transactionManager.CreateTransaction(), cancellationToken);
        }

        public async Task RunAsync(Operation operation)
        {
            int retriesCount = 0;

            string operationRunId = Guid.NewGuid().ToString();
            operation.Context.EventSource.Message("Starting operation {0}", operationRunId);

            while (true && !operation.Context.CancellationToken.IsCancellationRequested)
            {
                try
                {
                    await operation.Delegate(operation.Context);
                    operation.Context.EventSource.Message("Ending operation {0}", operationRunId);
                    await operation.Context.Transaction.CommitAsync();
                    return;
                }
                catch (Exception ex)
                {
                    operation.Context.EventSource.Message("Operation {0} failed: {1}", operationRunId, ex.Message);

                    retriesCount++;
                    if (retriesCount >= operation.Retries)
                    {
                        operation.Context.EventSource.Message("Ending operation {0}", operationRunId);
                        if (operation.IsCritical)
                            throw;
                        else
                            return;
                    }

                    operation.Context.EventSource.Message("Retrying operation {0}", operationRunId);
                }
            }
        }

        public Task RunAsync<TType1>(Func<OperationContext, TType1, Task> operationMethod, CancellationToken cancellationToken)
        {
            return RunAsync((context) => operationMethod.Invoke(context, _serviceProvider.GetService<TType1>()),
                cancellationToken);
        }

        public Task RunAsync<TType1, TType2>(Func<OperationContext, TType1, TType2, Task> operationMethod, CancellationToken cancellationToken)
        {
            return RunAsync((context) => operationMethod.Invoke(context, _serviceProvider.GetService<TType1>(), _serviceProvider.GetService<TType2>()),
                cancellationToken);
        }

        public Task RunAsync<TType1, TType2, TType3>(Func<OperationContext, TType1, TType2, TType3, Task> operationMethod, CancellationToken cancellationToken)
        {
            return RunAsync((context) => operationMethod.Invoke(context,
                _serviceProvider.GetService<TType1>(), _serviceProvider.GetService<TType2>(), _serviceProvider.GetService<TType3>()),
                cancellationToken);
        }

        public async Task RunAsync(Func<OperationContext, Task> operationMethod, OperationContext context)
        {
            var operation = new Operation
            {
                Delegate = operationMethod,
                Context = context,
                Retries = 4,
                IsCritical = true
            };

            await RunAsync(operation);
        }

        public Task RunAsync(Func<OperationContext, Task> operationMethod, CancellationToken cancellationToken)
        {
            return RunAsync(operationMethod, GetNewContext(cancellationToken));
        }
    }
}
