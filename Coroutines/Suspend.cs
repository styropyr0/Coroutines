using System;
using System.Threading.Tasks;

namespace Coroutines
{
    public static class Suspend
    {
        /// <summary>
        /// Suspends the coroutine for a specified duration.
        /// </summary>
        /// <param name="duration">The amount of time to suspend the coroutine.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="duration"/> is negative.</exception>
        public static async Task For(TimeSpan duration)
        {
            if (duration < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(duration), "Duration cannot be negative.");

            await Task.Delay(duration);
        }

        /// <summary>
        /// Suspends the coroutine for a specified number of milliseconds.
        /// </summary>
        /// <param name="milliseconds">The number of milliseconds to suspend the coroutine.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="milliseconds"/> is negative.</exception>
        public static async Task ForMilliseconds(int milliseconds)
        {
            if (milliseconds < 0)
                throw new ArgumentOutOfRangeException(nameof(milliseconds), "Milliseconds cannot be negative.");

            await Task.Delay(milliseconds);
        }

        /// <summary>
        /// Suspends the coroutine for a specified number of seconds.
        /// </summary>
        /// <param name="seconds">The number of seconds to suspend the coroutine.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="seconds"/> is negative.</exception>
        public static async Task ForSeconds(int seconds)
        {
            if (seconds < 0)
                throw new ArgumentOutOfRangeException(nameof(seconds), "Seconds cannot be negative.");

            await Task.Delay(TimeSpan.FromSeconds(seconds));
        }

        /// <summary>
        /// Suspends the coroutine until a given condition is met.
        /// </summary>
        /// <param name="condition">A function that returns a boolean indicating whether the condition is met.</param>
        /// <param name="checkIntervalMilliseconds">The interval (in milliseconds) to check the condition. Default is 100 ms.</param>
        /// <param name="timeoutMilliseconds">The maximum time to wait for the condition to be met, in milliseconds. 
        /// A value of -1 means no timeout. Default is -1.</param>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="condition"/> is null.</exception>
        /// <exception cref="TimeoutException">Thrown if the condition is not met within the specified timeout.</exception>
        public static async Task Until(Func<bool> condition, int checkIntervalMilliseconds = 100, int timeoutMilliseconds = -1)
        {
            if (condition == null)
                throw new ArgumentNullException(nameof(condition));

            var startTime = DateTime.UtcNow;
            while (!condition())
            {
                if (timeoutMilliseconds > 0 && (DateTime.UtcNow - startTime).TotalMilliseconds > timeoutMilliseconds)
                    throw new TimeoutException("The condition was not met within the specified timeout.");

                await Task.Delay(checkIntervalMilliseconds);
            }
        }
    }
}
