using System;
using System.Threading;
using System.Threading.Tasks;

namespace AnimalFarm.Utils.Tasks
{
    /// <summary>
    /// Contains helper methods for running asynchronous tasks.
    /// </summary>
    public static class RunTask
    {
        public static async Task WithRetries(Func<Task> task, int attempts)
        {
            await WithRetries(task, attempts, CancellationToken.None);
        }

        public static async Task WithRetries(Func<Task> task, int attempts, CancellationToken cancellationToken)
        {
            int attemptsCount = 0;

            while (true && !cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await task();
                    return;
                }
                catch
                {
                    attemptsCount++;
                    if (attemptsCount >= attempts)
                        throw;
                }
            }
        }
    }
}
