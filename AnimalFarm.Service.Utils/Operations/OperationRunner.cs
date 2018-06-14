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

        private OperationContext GetNewContext(CancellationToken? cancellationToken)
        {
            return new OperationContext(this, _eventSource, _transactionManager.CreateTransaction(), cancellationToken);
        }

        private async Task RunAsync(Operation operation)
        {
            int retriesCount = 0;
            var context = operation.Context;

            string operationRunId = Guid.NewGuid().ToString();
            context.EventSource.Message("Starting operation {0}", operationRunId);

            while (true)
            {
                context.CancellationToken.ThrowIfCancellationRequested();

                try
                {
                    var timeoutTask = Task.Delay(TimeSpan.FromMilliseconds(operation.Timeout), operation.Context.CancellationToken);
                    await Task.WhenAny(timeoutTask, operation.Delegate(operation.Context));
                    if (timeoutTask.IsCompleted)
                    {
                        context.EventSource.Message("Operation {0} timed out", operationRunId);
                        context.Cancel();
                        continue;
                    }

                    context.EventSource.Message("Ending operation {0}", operationRunId);
                    await context.Transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    if (ex is OperationCanceledException)
                        throw;

                    context.EventSource.Message("Operation {0} failed: {1}", operationRunId, ex.Message);

                    retriesCount++;
                    if (retriesCount < operation.Retries)
                    {
                        operation.Context.EventSource.Message("Retrying operation {0}", operationRunId);
                        continue;
                    }

                    context.EventSource.Message("Ending operation {0}", operationRunId);
                    if (operation.IsCritical)
                        throw;
                    else
                        return;
                }
            }
        }

        public Task RunAsync<TType1>(Func<OperationContext, TType1, Task> operationMethod,
            CancellationToken? cancellationToken = null, int? timeout = null, int? retries = null)
        {
            return RunAsync((context) => operationMethod.Invoke(context, _serviceProvider.GetService<TType1>()),
                cancellationToken);
        }

        public Task RunAsync<TType1, TType2>(Func<OperationContext, TType1, TType2, Task> operationMethod,
            CancellationToken? cancellationToken = null, int? timeout = null, int? retries = null)
        {
            return RunAsync((context) => operationMethod.Invoke(context, _serviceProvider.GetService<TType1>(), _serviceProvider.GetService<TType2>()),
                cancellationToken);
        }

        public Task RunAsync<TType1, TType2, TType3>(Func<OperationContext, TType1, TType2, TType3, Task> operationMethod,
            CancellationToken? cancellationToken = null, int? timeout = null, int? retries = null)
        {
            return RunAsync((context) => operationMethod.Invoke(context,
                _serviceProvider.GetService<TType1>(), _serviceProvider.GetService<TType2>(), _serviceProvider.GetService<TType3>()),
                cancellationToken);
        }

        public Task RunAsync(Func<OperationContext, Task> operationMethod,
            CancellationToken? cancellationToken = null, int? timeout = null, int? retries = null)
        {
            return RunAsync(operationMethod, GetNewContext(cancellationToken), timeout, retries);
        }

        internal Task RunAsync(Func<OperationContext, Task> operationMethod, OperationContext context,
            int? timeout = null, int? retries = null)
        {
            var operation = new Operation
            {
                Delegate = operationMethod,
                Context = context,
                Retries = retries ?? 4,
                Timeout = timeout ?? 240000,
                IsCritical = true
            };

            return RunAsync(operation);
        }
    }
}
