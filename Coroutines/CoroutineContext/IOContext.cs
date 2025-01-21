using System;
using System.Threading;
using System.Threading.Tasks;
using Coroutines;

namespace Coroutines.CoroutineContext
{
    public class IOContext : Dispatcher
    {
        public override async Task ExecuteAsync(Func<Task> task, CancellationToken cancellationToken)
        {
            await Task.Factory.StartNew(
                async () =>
                {
                    try
                    {
                        await task();
                    }
                    catch (Exception ex)
                    {
                        throw new CoroutineExecutionException("Error in IO-optimized context.", ex);
                    }
                },
                cancellationToken,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default
            ).Unwrap();
        }

        public override async Task<T> ExecuteAsync<T>(Func<Task<T>> task, CancellationToken cancellationToken)
        {
            try
            {
                return await task();
            }
            catch (Exception ex)
            {
                throw new CoroutineExecutionException("Error in IO-optimized context.", ex);
            }
        }
    }
}
