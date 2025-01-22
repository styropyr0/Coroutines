using System;
using System.Threading;
using System.Threading.Tasks;
using Coroutines;

namespace Coroutines.CoroutineContext
{
    /// <summary>
    /// Represents a dispatcher optimized for IO-bound tasks, such as network or file system operations.
    /// </summary>
    public class IOContext : Dispatcher
    {
        /// <summary>
        /// Executes the specified asynchronous task on an IO-optimized thread.
        /// </summary>
        /// <param name="task">The asynchronous task to execute.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the task execution.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="CoroutineExecutionException">Thrown if an error occurs during execution.</exception>
        public override async Task ExecuteAsync(Func<Task> task, CancellationToken cancellationToken)
        {
            await Task.Factory.StartNew(
                async () =>
                {
                    try
                    {
                        await task();
                    }
                    catch (Exception ex)
                    {
                        throw new CoroutineExecutionException("Error in IO-optimized context.", ex);
                    }
                },
                cancellationToken,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default
            ).Unwrap();
        }

        /// <summary>
        /// Executes the specified asynchronous task that returns a result on an IO-optimized thread.
        /// </summary>
        /// <typeparam name="T">The result type of the task.</typeparam>
        /// <param name="task">The asynchronous task to execute.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the task execution.</param>
        /// <returns>A task representing the asynchronous operation, with a result.</returns>
        /// <exception cref="CoroutineExecutionException">Thrown if an error occurs during execution.</exception>
        public override async Task<T> ExecuteAsync<T>(Func<Task<T>> task, CancellationToken cancellationToken)
        {
            try
            {
                return await task();
            }
            catch (Exception ex)
            {
                throw new CoroutineExecutionException("Error in IO-optimized context.", ex);
            }
        }
    }
}
