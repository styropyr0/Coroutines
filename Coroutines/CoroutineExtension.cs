using System;
using System.Threading;
using System.Threading.Tasks;

namespace Coroutines
{
    /// <summary>
    /// Provides extension methods for <see cref="Dispatcher"/> to execute coroutines within a specific context.
    /// </summary>
    public static class CoroutineContextExtensions
    {
        /// <summary>
        /// Executes the specified coroutine in the context of the provided <see cref="Dispatcher"/>.
        /// </summary>
        /// <typeparam name="T">The return type of the coroutine.</typeparam>
        /// <param name="dispatcher">The <see cref="Dispatcher"/> to execute the coroutine on.</param>
        /// <param name="block">The coroutine function to execute.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the coroutine execution. Defaults to <see cref="CancellationToken.None"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The result is the return value of the coroutine.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="block"/> is <c>null</c>.</exception>
        public static async Task<T> WithContext<T>(this Dispatcher dispatcher, Func<Task<T>> block, CancellationToken cancellationToken = default)
        {
            if (block == null)
                throw new ArgumentNullException(nameof(block));

            return await dispatcher.ExecuteAsync(block, cancellationToken);
        }

        /// <summary>
        /// Executes the specified coroutine in the context of the provided <see cref="Dispatcher"/>.
        /// </summary>
        /// <param name="dispatcher">The <see cref="Dispatcher"/> to execute the coroutine on.</param>
        /// <param name="block">The coroutine function to execute.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the coroutine execution. Defaults to <see cref="CancellationToken.None"/>.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="block"/> is <c>null</c>.</exception>
        public static async Task WithContext(this Dispatcher dispatcher, Func<Task> block, CancellationToken cancellationToken = default)
        {
            if (block == null)
                throw new ArgumentNullException(nameof(block));

            await dispatcher.ExecuteAsync(block, cancellationToken);
        }
    }
}
