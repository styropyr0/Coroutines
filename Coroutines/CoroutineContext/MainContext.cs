using System;
using System.Threading;
using System.Threading.Tasks;
using Coroutines;

namespace Coroutines.CoroutineContext
{
    public class MainContext : Dispatcher
    {
        private readonly SynchronizationContext _syncContext = SynchronizationContext.Current;

        public override async Task ExecuteAsync(Func<Task> task, CancellationToken cancellationToken)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            if (_syncContext != null)
            {
                try
                {
                    _syncContext.Post(async _ =>
                    {
                        try
                        {
                            await task();
                            taskCompletionSource.SetResult(true);
                        }
                        catch (Exception ex)
                        {
                            taskCompletionSource.SetException(new CoroutineExecutionException("Error in main thread context", ex));
                        }
                    }, null);
                }
                catch (Exception ex)
                {
                    taskCompletionSource.SetException(new CoroutineExecutionException("Failed to post task to main thread context.", ex));
                }
            }
            else
            {
                try
                {
                    await task();
                    taskCompletionSource.SetResult(true);
                }
                catch (Exception ex)
                {
                    taskCompletionSource.SetException(new CoroutineExecutionException("Error in default execution context.", ex));
                }
            }

            await taskCompletionSource.Task;
        }
    }
}
