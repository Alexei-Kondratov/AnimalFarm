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
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ITransactionManager _transactionManager;

        public OperationRunner(ILogger logger, IServiceProvider serviceProvider, ITransactionManager transactionManager)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _transactionManager = transactionManager;
        }

        private OperationContext GetNewContext(CancellationToken cancellationToken)
        {
            return new OperationContext(this, _logger, _transactionManager.CreateTransaction(), cancellationToken);
        }

        private async Task RunAsync(Operation operation)
        {
            int retriesCount = 0;
            var context = operation.Context;

            string operationRunId = Guid.NewGuid().ToString();
            string operationName = $"{operation.Delegate.Method.DeclaringType.Name}.{operation.Delegate.Method.Name}";
            _logger.LogOperationStart(operationRunId, operationName);

            while (true)
            {
                context.CancellationToken.ThrowIfCancellationRequested();

                try
                {
                    var operationTask = operation.Delegate(operation.Context);
                    var timeoutTask = Task.Delay(TimeSpan.FromMilliseconds(operation.Timeout), operation.Context.CancellationToken);
                    await Task.WhenAny(timeoutTask, operationTask);

                    if (timeoutTask.IsCompleted)
                    {
                        _logger.LogOperationTimeout(operationRunId);
                        context.Cancel();
                        continue;
                    }
                    else
                    {
                        await operationTask;
                    }

                    _logger.LogOperationStop(operationRunId);
                    await context.Transaction.CommitAsync();
                    return;
                }
                catch (Exception ex)
                {
                    if (ex is OperationCanceledException)
                        throw;

                    _logger.LogOperationException(operationRunId, ex);

                    retriesCount++;
                    if (retriesCount < operation.Retries)
                    {
                        _logger.LogOperationRetry(operationRunId, retriesCount, operation.Retries);
                        continue;
                    }

                    _logger.LogOperationStop(operationRunId);
                    if (operation.IsCritical)
                        throw;
                    else
                        return;
                }
            }
        }

        public Task RunAsync<TType1>(Func<OperationContext, TType1, Task> operationMethod,
            CancellationToken cancellationToken = default(CancellationToken), int? timeout = null, int? retries = null)
        {
            return RunAsync((context) => operationMethod.Invoke(context, _serviceProvider.GetService<TType1>()),
                cancellationToken, timeout, retries);
        }

        public Task RunAsync<TType1, TType2>(Func<OperationContext, TType1, TType2, Task> operationMethod,
            CancellationToken cancellationToken = default(CancellationToken), int? timeout = null, int? retries = null)
        {
            return RunAsync((context) => operationMethod.Invoke(context, _serviceProvider.GetService<TType1>(), _serviceProvider.GetService<TType2>()),
                cancellationToken, timeout, retries);
        }

        public Task RunAsync<TType1, TType2, TType3>(Func<OperationContext, TType1, TType2, TType3, Task> operationMethod,
            CancellationToken cancellationToken = default(CancellationToken), int? timeout = null, int? retries = null)
        {
            return RunAsync((context) => operationMethod.Invoke(context,
                _serviceProvider.GetService<TType1>(), _serviceProvider.GetService<TType2>(), _serviceProvider.GetService<TType3>()),
                cancellationToken, timeout, retries);
        }

        public Task RunAsync(Func<OperationContext, Task> operationMethod,
            CancellationToken cancellationToken = default(CancellationToken), int? timeout = null, int? retries = null)
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
