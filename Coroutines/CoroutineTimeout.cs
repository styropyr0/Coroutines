using System;
using System.Threading;
using System.Threading.Tasks;

namespace Coroutines
{
    public static class CoroutineTimeout
    {
        public static async Task<T> WithTimeout<T>(this Task<T> task, int millisecondsTimeout, CancellationToken cancellationToken = default)
        {
            if (await Task.WhenAny(task, Task.Delay(millisecondsTimeout, cancellationToken)) == task)
            {
                return await task;
            }
            else
            {
                throw new TimeoutException("The operation has timed out.");
            }
        }

        public static async Task WithTimeout(this Task task, int millisecondsTimeout, CancellationToken cancellationToken = default)
        {
            if (await Task.WhenAny(task, Task.Delay(millisecondsTimeout, cancellationToken)) == task)
            {
                await task;
            }
            else
            {
                throw new TimeoutException("The operation has timed out.");
            }
        }
    }
}
