using System;
using System.Threading;
using System.Threading.Tasks;
using Coroutines;

namespace Coroutines.CoroutineContext
{
    /// <summary>
    /// Represents a dispatcher optimized for I/O-bound tasks, such as network or file system operations.
    /// This context ensures that long-running I/O operations are executed on dedicated threads to avoid blocking the thread pool.
    /// </summary>
    public class IOContext : Dispatcher
    {
        /// <summary>
        /// Executes the specified asynchronous task on an I/O-optimized thread.
        /// For long-running I/O operations, this will use a dedicated thread to avoid blocking the thread pool.
        /// </summary>
        /// <param name="task">The asynchronous task to execute.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the task execution.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="CoroutineExecutionException">Thrown if an error occurs during execution.</exception>
        public override async Task ExecuteAsync(Func<Task> task, CancellationToken cancellationToken)
        {
            // Check if the task is a long-running I/O operation.
            if (IsLongRunningIOOperation(task))
            {
                // Run the long-running I/O task on a separate thread to avoid blocking the thread pool.
                await Task.Factory.StartNew(
                    async () =>
                    {
                        try
                        {
                            // Execute the task asynchronously.
                            await task();
                        }
                        catch (Exception ex)
                        {
                            // Handle exceptions during task execution.
                            throw new CoroutineExecutionException("Error in IO-optimized context.", ex);
                        }
                    },
                    cancellationToken,
                    TaskCreationOptions.LongRunning, // Ensures the task runs on a dedicated thread.
                    TaskScheduler.Default // Use the default task scheduler.
                ).Unwrap();
            }
            else
            {
                // For regular I/O tasks, run them asynchronously without dedicating a thread.
                await Task.Run(task, cancellationToken);
            }
        }

        /// <summary>
        /// Executes the specified asynchronous task that returns a result on an I/O-optimized thread.
        /// For long-running I/O operations, this will use a dedicated thread to avoid blocking the thread pool.
        /// </summary>
        /// <typeparam name="T">The result type of the task.</typeparam>
        /// <param name="task">The asynchronous task to execute.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the task execution.</param>
        /// <returns>A task representing the asynchronous operation, with a result.</returns>
        /// <exception cref="CoroutineExecutionException">Thrown if an error occurs during execution.</exception>
        public override async Task<T> ExecuteAsync<T>(Func<Task<T>> task, CancellationToken cancellationToken)
        {
            // Check if the task is a long-running I/O operation.
            if (IsLongRunningIOOperation(task))
            {
                // Run the long-running I/O task on a separate thread to avoid blocking the thread pool.
                return await Task.Factory.StartNew(
                    async () =>
                    {
                        try
                        {
                            // Execute the task asynchronously and return the result.
                            return await task();
                        }
                        catch (Exception ex)
                        {
                            // Handle exceptions during task execution.
                            throw new CoroutineExecutionException("Error in IO-optimized context.", ex);
                        }
                    },
                    cancellationToken,
                    TaskCreationOptions.LongRunning, // Ensures the task runs on a dedicated thread.
                    TaskScheduler.Default // Use the default task scheduler.
                ).Unwrap();
            }
            else
            {
                // For regular I/O tasks, run them asynchronously without dedicating a thread.
                return await Task.Run(task, cancellationToken);
            }
        }

        /// <summary>
        /// Determines whether the provided task represents a long-running I/O operation.
        /// This is used to decide if the task should be executed on a dedicated thread.
        /// </summary>
        /// <param name="task">The asynchronous task to evaluate.</param>
        /// <returns><c>true</c> if the task is a long-running I/O operation; otherwise, <c>false</c>.</returns>
        private bool IsLongRunningIOOperation(Func<Task> task)
        {
            // Example: A simple check for long-running tasks.
            // You can customize this logic based on task characteristics (e.g., network, file I/O, etc.).
            return task.Method.Name.Contains("LongRunning");
        }
    }
}
