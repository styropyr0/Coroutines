using System;
using System.Threading;
using System.Threading.Tasks;
using Coroutines;

namespace Coroutines.CoroutineContext
{
    public class UnconfinedContext : Dispatcher
    {
        public override async Task ExecuteAsync(Func<Task> task, CancellationToken cancellationToken)
        {
            try
            {
                await task();
            }
            catch (Exception ex)
            {
                throw new CoroutineExecutionException("Error in unconfined execution context.", ex);
            }
        }

        public override async Task<T> ExecuteAsync<T>(Func<Task<T>> task, CancellationToken cancellationToken)
        {
            try
            {
                return await task();
            }
            catch (Exception ex)
            {
                throw new CoroutineExecutionException("Error in unconfined execution context.", ex);
            }
        }
    }
}
