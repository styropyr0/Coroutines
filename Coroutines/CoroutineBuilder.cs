using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Coroutines
{
    /// <summary>
    /// Provides utility methods for launching and managing coroutines with dispatchers.
    /// </summary>
    public static class CoroutineBuilder
    {
        /// <summary>
        /// Launches a coroutine block using the specified dispatcher.
        /// </summary>
        /// <param name="block">The coroutine block to execute.</param>
        /// <param name="dispatcher">The dispatcher to execute the coroutine on. If null, the default dispatcher is used.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task Launch(Func<Task> block, Dispatcher dispatcher = null)
        {
            if (block == null) throw new ArgumentNullException(nameof(block));

            dispatcher ??= Dispatcher.Default;

            try
            {
                await dispatcher.ExecuteAsync(block, CancellationToken.None);
            }
            catch (Exception ex)
            {
                throw new CoroutineExecutionException("Error during coroutine execution.", ex);
            }
        }

        /// <summary>
        /// Executes multiple coroutines in parallel and waits for all to complete.
        /// </summary>
        /// <param name="blocks">The collection of coroutine blocks to execute.</param>
        /// <param name="dispatcher">The dispatcher to execute the coroutines on. If null, the default dispatcher is used.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task LaunchAll(IEnumerable<Func<Task>> blocks, Dispatcher dispatcher = null)
        {
            if (blocks == null || !blocks.Any()) throw new ArgumentNullException(nameof(blocks));

            dispatcher ??= Dispatcher.Default;

            try
            {
                var tasks = blocks.Select(block => dispatcher.ExecuteAsync(block, CancellationToken.None));
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                throw new CoroutineExecutionException("Error during parallel coroutine execution.", ex);
            }
        }

        /// <summary>
        /// Executes multiple synchronous functions in parallel and waits for all to complete.
        /// </summary>
        /// <param name="blocks">The collection of synchronous functions to execute.</param>
        /// <param name="dispatcher">The dispatcher to execute the functions on. If null, the default dispatcher is used.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task LaunchAll(IEnumerable<Action> blocks, Dispatcher dispatcher = null)
        {
            if (blocks == null || !blocks.Any()) throw new ArgumentNullException(nameof(blocks));

            dispatcher ??= Dispatcher.Default;

            try
            {
                var tasks = blocks.Select(block =>
                    dispatcher.ExecuteAsync(() =>
                    {
                        block();
                        return Task.CompletedTask;
                    }, CancellationToken.None)
                );
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                throw new CoroutineExecutionException("Error during parallel function execution.", ex);
            }
        }
    }
}
