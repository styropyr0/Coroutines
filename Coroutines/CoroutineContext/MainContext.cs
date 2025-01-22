using System;
using System.Threading;
using System.Threading.Tasks;
using Coroutines;

namespace Coroutines.CoroutineContext
{
    /// <summary>
    /// Represents a dispatcher that executes tasks on the main thread context using <see cref="SynchronizationContext"/>.
    /// </summary>
    public class MainContext : Dispatcher
    {
        private readonly SynchronizationContext _syncContext = SynchronizationContext.Current;

        /// <summary>
        /// Executes the specified asynchronous task on the main thread context.
        /// </summary>
        /// <param name="task">The asynchronous task to execute.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the task execution.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="CoroutineExecutionException">Thrown if an error occurs during execution.</exception>
        public override async Task ExecuteAsync(Func<Task> task, CancellationToken cancellationToken)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            if (_syncContext != null)
            {
                _syncContext.Post(async _ =>
                {
                    try
                    {
                        await task();
                        taskCompletionSource.SetResult(true);
                    }
                    catch (Exception ex)
                    {
                        taskCompletionSource.SetException(new CoroutineExecutionException("Error in main thread context.", ex));
                    }
                }, null);
            }
            else
            {
                await Task.Run(task, cancellationToken);
                taskCompletionSource.SetResult(true);
            }

            await taskCompletionSource.Task;
        }

        /// <summary>
        /// Executes the specified asynchronous task that returns a result on the main thread context.
        /// </summary>
        /// <typeparam name="T">The result type of the task.</typeparam>
        /// <param name="task">The asynchronous task to execute.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the task execution.</param>
        /// <returns>A task representing the asynchronous operation, with a result.</returns>
        /// <exception cref="CoroutineExecutionException">Thrown if an error occurs during execution.</exception>
        public override async Task<T> ExecuteAsync<T>(Func<Task<T>> task, CancellationToken cancellationToken)
        {
            var taskCompletionSource = new TaskCompletionSource<T>();

            if (_syncContext != null)
            {
                _syncContext.Post(async _ =>
                {
                    try
                    {
                        var result = await task();
                        taskCompletionSource.SetResult(result);
                    }
                    catch (Exception ex)
                    {
                        taskCompletionSource.SetException(new CoroutineExecutionException("Error in main thread context.", ex));
                    }
                }, null);
            }
            else
            {
                var result = await task();
                taskCompletionSource.SetResult(result);
            }

            return await taskCompletionSource.Task;
        }
    }
}
