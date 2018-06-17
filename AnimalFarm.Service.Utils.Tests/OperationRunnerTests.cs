using AnimalFarm.Data;
using AnimalFarm.Service.Utils.Operations;
using AnimalFarm.Service.Utils.Tracing;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AnimalFarm.Service.Utils.Tests
{
    public class OperationRunnerTests
    {
        private Mock<ILogger> _logger;
        private Mock<ITransaction> _transactionMock;
        private Mock<ITransactionManager> _transactionManagerMock;

        public OperationRunnerTests()
        {
            _transactionMock = new Mock<ITransaction>();
            _transactionManagerMock = new Mock<ITransactionManager>();
            _logger = new Mock<ILogger>();
            _transactionManagerMock.Setup(_ => _.CreateTransaction()).Returns(_transactionMock.Object);
        }

        [Fact]
        public void Cancelled_if_the_provided_token_is_cancelled()
        {
            // Arrange
            var target = new OperationRunner(_logger.Object, new Mock<IServiceProvider>().Object, _transactionManagerMock.Object);
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
            var target = new OperationRunner(_logger.Object, new Mock<IServiceProvider>().Object, _transactionManagerMock.Object);
            var task = target.RunAsync((context) => Task.Delay(int.MaxValue, context.CancellationToken), timeout: 1);

            // Act
            await Task.WhenAny(task, Task.Delay(1000000));

            // Assert
            Assert.True(task.IsCanceled);
        }

        [Fact]
        public async Task RunAsync_executes_the_task_succesfully_only_once()
        {
            // Arrange
            int count = 0;
            var target = new OperationRunner(_logger.Object, new Mock<IServiceProvider>().Object, _transactionManagerMock.Object);

            // Act
            await target.RunAsync(async (context) => { count++; } );

            // Assert
            Assert.Equal(1, count);
        }

        [Fact]
        public async Task RunAsync_repeatedly_executes_failed_task()
        {
            // Arrange
            int count = 0;
            var target = new OperationRunner(_logger.Object, new Mock<IServiceProvider>().Object, _transactionManagerMock.Object);

            // Act
            await target.RunAsync(async (context) => { count++; if (count < 2) throw new Exception(); });

            // Assert
            Assert.Equal(2, count);
        }
    }
}
