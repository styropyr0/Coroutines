using System;
using System.Threading.Tasks;

namespace Coroutines
{
    public static class Suspend
    {
        public static async Task Delay(int milliseconds)
        {
            try
            {
                await Task.Delay(milliseconds);
            }
            catch (Exception ex)
            {
                throw new CoroutineExecutionException("Error during delay suspension.", ex);
            }
        }

        public static async Task<T> From<T>(Func<Task<T>> function)
        {
            try
            {
                return await function();
            }
            catch (Exception ex)
            {
                throw new CoroutineExecutionException("Error during From suspension.", ex);
            }
        }

        public static async Task SuspendWith(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                throw new CoroutineExecutionException("Error during SuspendWith action.", ex);
            }
        }
    }
}
