using AnimalFarm.Data;
using AnimalFarm.Service.Utils.Operations;
using AnimalFarm.Service.Utils.Tracing;
using Moq;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AnimalFarm.Service.Utils.Tests
{
    public class OperationRunnerTests
    {
        [Fact]
        public void Cancelled_if_the_provided_token_is_cancelled()
        {
            // Arrange
            var target = new OperationRunner(ServiceEventSource.Current, new Mock<IServiceProvider>().Object, new Mock<ITransactionManager>().Object);
            var cancellationSource = new CancellationTokenSource();

            // Act
            var task = target.RunAsync((context) => Task.Delay(int.MaxValue, context.CancellationToken), cancellationSource.Token);
            cancellationSource.Cancel();

            // Assert
            Assert.True(task.IsCanceled);
        }

        [Fact]
        public async Task Cancelled_if_the_context_on_timeout()
        {
            // Arrange
            var target = new OperationRunner(ServiceEventSource.Current, new Mock<IServiceProvider>().Object, new Mock<ITransactionManager>().Object);
            var task = target.RunAsync((context) => Task.Delay(int.MaxValue, context.CancellationToken), timeout: 1);

            // Act
            try
            {
                await task;
            }
            catch (OperationCanceledException)
            {
            }

            // Assert
            Assert.True(task.IsCanceled);
        }
    }
}
