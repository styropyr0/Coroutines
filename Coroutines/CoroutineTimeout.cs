using System;
using System.Threading;
using System.Threading.Tasks;

namespace Coroutines
{
    public static class CoroutineTimeout
    {
        /// <summary>
        /// Runs a coroutine that returns a value and applies a timeout. If the coroutine doesn't finish in time, a <see cref="TimeoutException"/> is thrown.
        /// </summary>
        /// <typeparam name="T">The type of the value returned by the coroutine.</typeparam>
        /// <param name="coroutine">The coroutine to execute.</param>
        /// <param name="timeout">The time span after which the coroutine will time out.</param>
        /// <param name="dispatcher">The dispatcher where the coroutine will run. If null, the default dispatcher is used.</param>
        /// <returns>The result of the coroutine if it finishes before the timeout.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the coroutine is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the timeout is negative.</exception>
        /// <exception cref="TimeoutException">Thrown if the coroutine exceeds the timeout period.</exception>
        public static async Task<T> Run<T>(Func<T> coroutine, TimeSpan timeout, Dispatcher dispatcher = null)
        {
            if (coroutine == null)
                throw new ArgumentNullException(nameof(coroutine));
            if (timeout < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(timeout), "Timeout must be non-negative.");

            dispatcher ??= Dispatcher.Default;

            using var cts = new CancellationTokenSource();
            var task = dispatcher.ExecuteAsync(() => Task.FromResult(coroutine()), cts.Token);

            if (await Task.WhenAny(task, Task.Delay(timeout)) == task)
            {
                return await task;
            }
            else
            {
                cts.Cancel();
                throw new TimeoutException("The coroutine exceeded the timeout.");
            }
        }

        /// <summary>
        /// Runs a coroutine that takes no parameters and returns no value, with a timeout. If the coroutine doesn't finish in time, a <see cref="TimeoutException"/> is thrown.
        /// </summary>
        /// <param name="coroutine">The coroutine to execute.</param>
        /// <param name="timeout">The time span after which the coroutine will time out.</param>
        /// <param name="dispatcher">The dispatcher where the coroutine will run. If null, the default dispatcher is used.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the coroutine is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the timeout is negative.</exception>
        /// <exception cref="TimeoutException">Thrown if the coroutine exceeds the timeout period.</exception>
        public static async Task Run(Action coroutine, TimeSpan timeout, Dispatcher dispatcher = null)
        {
            if (coroutine == null)
                throw new ArgumentNullException(nameof(coroutine));
            if (timeout < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(timeout), "Timeout must be non-negative.");

            dispatcher ??= Dispatcher.Default;

            using var cts = new CancellationTokenSource();
            var task = dispatcher.ExecuteAsync(() =>
            {
                coroutine();
                return Task.CompletedTask;
            }, cts.Token);

            if (await Task.WhenAny(task, Task.Delay(timeout)) == task)
            {
                await task;
            }
            else
            {
                cts.Cancel();
                throw new TimeoutException("The coroutine exceeded the timeout.");
            }
        }

        /// <summary>
        /// Runs a coroutine that returns a <see cref="Task"/> and applies a timeout. If the coroutine doesn't finish in time, a <see cref="TimeoutException"/> is thrown.
        /// </summary>
        /// <typeparam name="T">The type of the value returned by the coroutine.</typeparam>
        /// <param name="coroutine">The coroutine to execute, which returns a <see cref="Task{T}"/>.</param>
        /// <param name="timeout">The time span after which the coroutine will time out.</param>
        /// <param name="dispatcher">The dispatcher where the coroutine will run. If null, the default dispatcher is used.</param>
        /// <returns>The result of the coroutine if it finishes before the timeout.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the coroutine is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the timeout is negative.</exception>
        /// <exception cref="TimeoutException">Thrown if the coroutine exceeds the timeout period.</exception>
        public static async Task<T> Run<T>(Func<Task<T>> coroutine, TimeSpan timeout, Dispatcher dispatcher = null)
        {
            if (coroutine == null)
                throw new ArgumentNullException(nameof(coroutine));
            if (timeout < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(timeout), "Timeout must be non-negative.");

            dispatcher ??= Dispatcher.Default;

            using var cts = new CancellationTokenSource();
            var task = dispatcher.ExecuteAsync(coroutine, cts.Token);

            if (await Task.WhenAny(task, Task.Delay(timeout)) == task)
            {
                return await task;
            }
            else
            {
                cts.Cancel();
                throw new TimeoutException("The coroutine exceeded the timeout.");
            }
        }
    }
}
