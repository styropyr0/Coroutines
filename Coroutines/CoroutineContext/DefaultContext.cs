using System;
using System.Threading;
using System.Threading.Tasks;
using Coroutines;

namespace Coroutines.CoroutineContext
{
    /// <summary>
    /// Represents the default dispatcher that runs tasks without any special context or optimization.
    /// </summary>
    public class DefaultContext : Dispatcher
    {
        /// <summary>
        /// Executes the specified asynchronous task using the default dispatcher.
        /// </summary>
        /// <param name="task">The asynchronous task to execute.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the task execution.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="CoroutineExecutionException">Thrown if an error occurs during execution.</exception>
        public override Task ExecuteAsync(Func<Task> task, CancellationToken cancellationToken)
        {
            return Task.Run(task, cancellationToken);
        }

        /// <summary>
        /// Executes the specified asynchronous task that returns a result using the default dispatcher.
        /// </summary>
        /// <typeparam name="T">The result type of the task.</typeparam>
        /// <param name="task">The asynchronous task to execute.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the task execution.</param>
        /// <returns>A task representing the asynchronous operation, with a result.</returns>
        /// <exception cref="CoroutineExecutionException">Thrown if an error occurs during execution.</exception>
        public override Task<T> ExecuteAsync<T>(Func<Task<T>> task, CancellationToken cancellationToken)
        {
            return Task.Run(task, cancellationToken);
        }
    }
}
