using System;
using System.Threading;
using System.Threading.Tasks;

namespace Coroutines
{
    public static class CoroutineContextExtensions
    {
        public static async Task<T> WithContext<T>(this Dispatcher dispatcher, Func<Task<T>> block, CancellationToken cancellationToken = default)
        {
            return await dispatcher.ExecuteAsync(block, cancellationToken);
        }

        public static async Task WithContext(this Dispatcher dispatcher, Func<Task> block, CancellationToken cancellationToken = default)
        {
            await dispatcher.ExecuteAsync(block, cancellationToken);
        }
    }
}
