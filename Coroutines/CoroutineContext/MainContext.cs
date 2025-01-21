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
                _syncContext.Post(async _ =>
                {
                    try
                    {
                        await task();
                        taskCompletionSource.SetResult(true);
                    }
                    catch (Exception ex)
                    {
                        taskCompletionSource.SetException(new CoroutineExecutionException("Error in main thread context.", ex));
                    }
                }, null);
            }
            else
            {
                await Task.Run(task, cancellationToken);
                taskCompletionSource.SetResult(true);
            }

            await taskCompletionSource.Task;
        }

        public override async Task<T> ExecuteAsync<T>(Func<Task<T>> task, CancellationToken cancellationToken)
        {
            var taskCompletionSource = new TaskCompletionSource<T>();

            if (_syncContext != null)
            {
                _syncContext.Post(async _ =>
                {
                    try
                    {
                        var result = await task();
                        taskCompletionSource.SetResult(result);
                    }
                    catch (Exception ex)
                    {
                        taskCompletionSource.SetException(new CoroutineExecutionException("Error in main thread context.", ex));
                    }
                }, null);
            }
            else
            {
                var result = await task();
                taskCompletionSource.SetResult(result);
            }

            return await taskCompletionSource.Task;
        }
    }
}
