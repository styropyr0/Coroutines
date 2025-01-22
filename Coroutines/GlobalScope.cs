using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Coroutines
{
    /// <summary>
    /// Provides global access to coroutine scopes and utilities for launching, combining, and managing coroutines.
    /// </summary>
    public static class GlobalScope
    {
        private static readonly ConcurrentDictionary<Dispatcher, CoroutineScope> Scopes = new();

        /// <summary>
        /// Retrieves or creates a coroutine scope for the given dispatcher.
        /// </summary>
        /// <param name="dispatcher">The dispatcher context for the scope. If null, uses the default dispatcher.</param>
        /// <returns>The coroutine scope associated with the dispatcher.</returns>
        private static CoroutineScope GetOrCreateScope(Dispatcher dispatcher)
        {
            return Scopes.GetOrAdd(dispatcher ?? Dispatcher.Default, d => new CoroutineScope(d));
        }

        /// <summary>
        /// Launches a coroutine that takes no parameters and returns no value.
        /// </summary>
        /// <param name="coroutine">The coroutine to launch.</param>
        /// <param name="dispatcher">The dispatcher where the coroutine will run. If null, the default dispatcher is used.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the coroutine.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the coroutine is null.</exception>
        public static async Task Launch(Action coroutine, Dispatcher dispatcher = null, CancellationToken cancellationToken = default)
        {
            if (coroutine == null) throw new ArgumentNullException(nameof(coroutine));

            var scope = GetOrCreateScope(dispatcher);
            try
            {
                await scope.Launch(coroutine, dispatcher);
            }
            catch (Exception ex)
            {
                CoroutineExceptionHandler.Handle(ex);
            }
        }

        /// <summary>
        /// Launches a coroutine that takes a value and returns that value.
        /// </summary>
        /// <param name="coroutine">The coroutine to launch.</param>
        /// <param name="dispatcher">The dispatcher where the coroutine will run. If null, the default dispatcher is used.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the coroutine.</param>
        /// <returns>A task representing the asynchronous operation that will return the result of the coroutine.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the coroutine is null.</exception>
        public static async Task Launch<T>(Func<T> coroutine, Dispatcher dispatcher = null, CancellationToken cancellationToken = default)
        {
            if (coroutine == null) throw new ArgumentNullException(nameof(coroutine));

            var scope = GetOrCreateScope(dispatcher);
            try
            {
                await scope.Launch(coroutine, dispatcher);
            }
            catch (Exception ex)
            {
                CoroutineExceptionHandler.Handle(ex);
            }
        }

        /// <summary>
        /// Launches a coroutine that returns a <see cref="Task"/> and waits for its completion.
        /// </summary>
        /// <param name="coroutine">The coroutine to launch.</param>
        /// <param name="dispatcher">The dispatcher where the coroutine will run. If null, the default dispatcher is used.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the coroutine.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the coroutine is null.</exception>
        public static async Task Launch(Func<Task> coroutine, Dispatcher dispatcher = null, CancellationToken cancellationToken = default)
        {
            if (coroutine == null) throw new ArgumentNullException(nameof(coroutine));

            var scope = GetOrCreateScope(dispatcher);
            try
            {
                await scope.Launch(coroutine, dispatcher);
            }
            catch (Exception ex)
            {
                CoroutineExceptionHandler.Handle(ex);
            }
        }

        /// <summary>
        /// Combines multiple coroutines that take no parameters and runs them in parallel.
        /// </summary>
        /// <param name="coroutines">The collection of coroutines to execute.</param>
        /// <param name="dispatcher">The dispatcher where the coroutines will run. If null, the default dispatcher is used.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the coroutines.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the coroutines collection is null.</exception>
        public static async Task Combine(IEnumerable<Action> coroutines, Dispatcher dispatcher = null, CancellationToken cancellationToken = default)
        {
            if (coroutines == null) throw new ArgumentNullException(nameof(coroutines));

            var scope = GetOrCreateScope(dispatcher);
            try
            {
                await scope.Combine(coroutines, dispatcher);
            }
            catch (Exception ex)
            {
                CoroutineExceptionHandler.Handle(ex);
            }
        }

        /// <summary>
        /// Combines multiple coroutines that take a value and runs them in parallel.
        /// </summary>
        /// <param name="coroutines">The collection of coroutines to execute.</param>
        /// <param name="dispatcher">The dispatcher where the coroutines will run. If null, the default dispatcher is used.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the coroutines.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the coroutines collection is null.</exception>
        public static async Task Combine<T>(IEnumerable<Func<T>> coroutines, Dispatcher dispatcher = null, CancellationToken cancellationToken = default)
        {
            if (coroutines == null) throw new ArgumentNullException(nameof(coroutines));

            var scope = GetOrCreateScope(dispatcher);
            try
            {
                await scope.Combine(coroutines, dispatcher);
            }
            catch (Exception ex)
            {
                CoroutineExceptionHandler.Handle(ex);
            }
        }

        /// <summary>
        /// Combines multiple coroutines that return a <see cref="Task"/> and runs them in parallel.
        /// </summary>
        /// <param name="coroutines">The collection of coroutines to execute.</param>
        /// <param name="dispatcher">The dispatcher where the coroutines will run. If null, the default dispatcher is used.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the coroutines.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the coroutines collection is null.</exception>
        public static async Task Combine(IEnumerable<Func<Task>> coroutines, Dispatcher dispatcher = null, CancellationToken cancellationToken = default)
        {
            if (coroutines == null) throw new ArgumentNullException(nameof(coroutines));

            var scope = GetOrCreateScope(dispatcher);
            try
            {
                await scope.Combine(coroutines, dispatcher);
            }
            catch (Exception ex)
            {
                CoroutineExceptionHandler.Handle(ex);
            }
        }

        /// <summary>
        /// Combines multiple coroutines and executes the first one to complete.
        /// </summary>
        /// <param name="coroutines">The collection of coroutines to execute.</param>
        /// <param name="dispatcher">The dispatcher where the coroutines will run. If null, the default dispatcher is used.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the coroutines.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the coroutines collection is null.</exception>
        public static async Task CombineFirst(IEnumerable<Action> coroutines, Dispatcher dispatcher = null, CancellationToken cancellationToken = default)
        {
            if (coroutines == null) throw new ArgumentNullException(nameof(coroutines));

            var scope = GetOrCreateScope(dispatcher);
            try
            {
                await scope.CombineFirst(coroutines, dispatcher);
            }
            catch (Exception ex)
            {
                CoroutineExceptionHandler.Handle(ex);
            }
        }

        /// <summary>
        /// Combines multiple coroutines that return a <see cref="Task"/> and executes the first one to complete.
        /// </summary>
        /// <param name="coroutines">The collection of coroutines to execute.</param>
        /// <param name="dispatcher">The dispatcher where the coroutines will run. If null, the default dispatcher is used.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the coroutines.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the coroutines collection is null.</exception>
        public static async Task CombineFirst(IEnumerable<Func<Task>> coroutines, Dispatcher dispatcher = null, CancellationToken cancellationToken = default)
        {
            if (coroutines == null) throw new ArgumentNullException(nameof(coroutines));

            var scope = GetOrCreateScope(dispatcher);
            try
            {
                await scope.CombineFirst(coroutines, dispatcher);
            }
            catch (Exception ex)
            {
                CoroutineExceptionHandler.Handle(ex);
            }
        }

        /// <summary>
        /// Waits for all coroutines to complete in the global scope.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to cancel the waiting process.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task WaitAllAsync(CancellationToken cancellationToken = default)
        {
            var tasks = Scopes.Values.SelectMany(scope => scope.GetTasks(cancellationToken)).ToList();
            if (tasks.Any())
            {
                try
                {
                    await Task.WhenAll(tasks);
                }
                catch (Exception ex)
                {
                    CoroutineExceptionHandler.Handle(ex);
                }
            }
        }

        /// <summary>
        /// Cancels all coroutines in the global scope.
        /// </summary>
        public static void CancelAll()
        {
            foreach (var scope in Scopes.Values)
            {
                scope.Cancel();
            }
        }

        /// <summary>
        /// Disposes of all coroutine scopes in the global scope asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous disposal operation.</returns>
        public static async Task DisposeAllAsync()
        {
            foreach (var scope in Scopes.Values)
            {
                await scope.DisposeAsync();
            }
        }
    }
}

