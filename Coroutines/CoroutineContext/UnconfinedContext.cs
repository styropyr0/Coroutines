using System;
using System.Threading;
using System.Threading.Tasks;
using Coroutines;

namespace Coroutines.CoroutineContext
{
    /// <summary>
    /// Represents a dispatcher that runs tasks without a specific synchronization context, 
    /// executing the task asynchronously without any confinement to a particular thread.
    /// </summary>
    public class UnconfinedContext : Dispatcher
    {
        /// <summary>
        /// Executes the specified asynchronous task without any synchronization context.
        /// </summary>
        /// <param name="task">The asynchronous task to execute.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the task execution.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="CoroutineExecutionException">Thrown if an error occurs during execution.</exception>
        public override async Task ExecuteAsync(Func<Task> task, CancellationToken cancellationToken)
        {
            try
            {
                await task();
            }
            catch (Exception ex)
            {
                throw new CoroutineExecutionException("Error in unconfined execution context.", ex);
            }
        }

        /// <summary>
        /// Executes the specified asynchronous task that returns a result, without any synchronization context.
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
                throw new CoroutineExecutionException("Error in unconfined execution context.", ex);
            }
        }
    }
}
