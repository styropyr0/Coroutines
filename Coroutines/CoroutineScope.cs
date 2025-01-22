using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Coroutines
{
    public class CoroutineScope : IAsyncDisposable
    {
        protected readonly Dispatcher _context;
        private readonly List<Task> _tasks = new List<Task>();
        private readonly CancellationTokenSource _scopeCancellationTokenSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoroutineScope"/> class with a specified dispatcher.
        /// </summary>
        /// <param name="context">The dispatcher context for the scope.</param>
        public CoroutineScope(Dispatcher context)
        {
            _context = context;
            _scopeCancellationTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// Launches a coroutine that takes no parameters and returns no value.
        /// </summary>
        /// <param name="coroutine">The coroutine to launch.</param>
        /// <param name="dispatcher">The dispatcher where the coroutine will run. If null, the scope's dispatcher is used.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the coroutine is null.</exception>
        public virtual async Task Launch(Action coroutine, Dispatcher dispatcher = null)
        {
            if (coroutine == null) throw new ArgumentNullException(nameof(coroutine));

            dispatcher ??= _context;
            var cancellationToken = _scopeCancellationTokenSource.Token;

            try
            {
                var task = dispatcher.ExecuteAsync(() =>
                {
                    coroutine();
                    return Task.CompletedTask;
                }, cancellationToken);

                _tasks.Add(task);
                await task;
            }
            catch (Exception ex)
            {
                throw new CoroutineExecutionException("Error during coroutine execution.", ex);
            }
        }

        /// <summary>
        /// Launches a coroutine that takes a value and returns that value.
        /// </summary>
        /// <param name="coroutine">The coroutine to launch.</param>
        /// <param name="dispatcher">The dispatcher where the coroutine will run. If null, the scope's dispatcher is used.</param>
        /// <returns>A task representing the asynchronous operation that will return the result of the coroutine.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the coroutine is null.</exception>
        public virtual async Task Launch<T>(Func<T> coroutine, Dispatcher dispatcher = null)
        {
            if (coroutine == null) throw new ArgumentNullException(nameof(coroutine));

            dispatcher ??= _context;
            var cancellationToken = _scopeCancellationTokenSource.Token;

            try
            {
                var task = dispatcher.ExecuteAsync(() =>
                {
                    var result = coroutine();
                    return Task.FromResult(result);
                }, cancellationToken);

                _tasks.Add(task);
                await task;
            }
            catch (Exception ex)
            {
                throw new CoroutineExecutionException("Error during coroutine execution.", ex);
            }
        }

        /// <summary>
        /// Launches a coroutine that returns a <see cref="Task"/> and waits for its completion.
        /// </summary>
        /// <param name="coroutine">The coroutine to launch.</param>
        /// <param name="dispatcher">The dispatcher where the coroutine will run. If null, the scope's dispatcher is used.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the coroutine is null.</exception>
        public virtual async Task Launch(Func<Task> coroutine, Dispatcher dispatcher = null)
        {
            if (coroutine == null) throw new ArgumentNullException(nameof(coroutine));

            dispatcher ??= _context;
            var cancellationToken = _scopeCancellationTokenSource.Token;

            try
            {
                var task = dispatcher.ExecuteAsync(coroutine, cancellationToken);
                _tasks.Add(task);
                await task;
            }
            catch (Exception ex)
            {
                throw new CoroutineExecutionException("Error during coroutine execution.", ex);
            }
        }

        /// <summary>
        /// Launches a coroutine that returns a <see cref="Task"/> and waits for its completion.
        /// </summary>
        /// <param name="coroutine">The coroutine to launch.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the coroutine is null.</exception>
        public virtual async Task Launch(Func<Task> coroutine)
        {
            if (coroutine == null) throw new ArgumentNullException(nameof(coroutine));

            Dispatcher dispatcher = _context;
            var cancellationToken = _scopeCancellationTokenSource.Token;

            try
            {
                var task = dispatcher.ExecuteAsync(coroutine, cancellationToken);
                _tasks.Add(task);
                await task;
            }
            catch (Exception ex)
            {
                throw new CoroutineExecutionException("Error during coroutine execution.", ex);
            }
        }

        /// <summary>
        /// Combines multiple coroutines that take no parameters and run them in parallel.
        /// </summary>
        /// <param name="coroutines">The collection of coroutines to execute.</param>
        /// <param name="dispatcher">The dispatcher where the coroutines will run. If null, the scope's dispatcher is used.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task Combine(IEnumerable<Action> coroutines, Dispatcher dispatcher = null)
        {
            var tasks = coroutines.Select(coroutine => Launch(coroutine, dispatcher));
            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Combines multiple coroutines that take a value and run them in parallel.
        /// </summary>
        /// <param name="coroutines">The collection of coroutines to execute.</param>
        /// <param name="dispatcher">The dispatcher where the coroutines will run. If null, the scope's dispatcher is used.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task Combine<T>(IEnumerable<Func<T>> coroutines, Dispatcher dispatcher = null)
        {
            var tasks = coroutines.Select(coroutine => Launch(coroutine, dispatcher));
            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Combines multiple coroutines that return a <see cref="Task"/> and runs them in parallel.
        /// </summary>
        /// <param name="coroutines">The collection of coroutines to execute.</param>
        /// <param name="dispatcher">The dispatcher where the coroutines will run. If null, the scope's dispatcher is used.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task Combine(IEnumerable<Func<Task>> coroutines, Dispatcher dispatcher = null)
        {
            var tasks = coroutines.Select(coroutine => Launch(coroutine, dispatcher));
            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Combines multiple coroutines and executes the first one to complete.
        /// </summary>
        /// <param name="coroutines">The collection of coroutines to execute.</param>
        /// <param name="dispatcher">The dispatcher where the coroutines will run. If null, the scope's dispatcher is used.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task CombineFirst(IEnumerable<Action> coroutines, Dispatcher dispatcher = null)
        {
            var tasks = coroutines.Select(coroutine => Launch(coroutine, dispatcher));
            await Task.WhenAny(tasks);
        }

        /// <summary>
        /// Combines multiple coroutines that return a <see cref="Task"/> and executes the first one to complete.
        /// </summary>
        /// <param name="coroutines">The collection of coroutines to execute.</param>
        /// <param name="dispatcher">The dispatcher where the coroutines will run. If null, the scope's dispatcher is used.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task CombineFirst(IEnumerable<Func<Task>> coroutines, Dispatcher dispatcher = null)
        {
            var tasks = coroutines.Select(coroutine => Launch(coroutine, dispatcher));
            await Task.WhenAny(tasks);
        }

        /// <summary>
        /// Waits for all coroutines to complete in the current scope.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task WaitAllAsync()
        {
            try
            {
                await Task.WhenAll(_tasks);
            }
            catch (Exception ex)
            {
                throw new CoroutineExecutionException("Error while waiting for all coroutines.", ex);
            }
        }

        /// <summary>
        /// Cancels all coroutines in the current scope.
        /// </summary>
        public void Cancel()
        {
            _scopeCancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Disposes of the <see cref="CoroutineScope"/>, cancelling any ongoing coroutines and waiting for them to finish.
        /// </summary>
        /// <returns>A task that represents the asynchronous dispose operation.</returns>
        public async ValueTask DisposeAsync()
        {
            Cancel();
            await WaitAllAsync();
            _scopeCancellationTokenSource.Dispose();
        }

        /// <summary>
        /// Gets the collection of tasks that are still running and not cancelled.
        /// </summary>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>An enumerable collection of running tasks.</returns>
        public IEnumerable<Task> GetTasks(CancellationToken cancellationToken)
        {
            return _tasks.Where(task => !task.IsCompleted && !cancellationToken.IsCancellationRequested);
        }
    }
}
