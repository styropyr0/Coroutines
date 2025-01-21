using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Coroutines
{
    public class CoroutineScope : IAsyncDisposable
    {
        private readonly Dispatcher _context;
        private List<Task> _tasks = new List<Task>();
        private CancellationTokenSource _scopeCancellationTokenSource;

        public CoroutineScope(Dispatcher context)
        {
            _context = context;
            _scopeCancellationTokenSource = new CancellationTokenSource();
        }

        public virtual async Task Launch(Func<Task> coroutine, Dispatcher dispatcher = null)
        {
            try
            {
                dispatcher = dispatcher ?? _context;

                var cancellationToken = _scopeCancellationTokenSource.Token;
                var task = dispatcher.ExecuteAsync(coroutine, cancellationToken);
                _tasks.Add(task);
                await task;
            }
            catch (Exception ex)
            {
                throw new CoroutineExecutionException("An error occurred while executing the coroutine.", ex);
            }
        }

        public async Task Combine(IEnumerable<Func<Task>> coroutines)
        {
            var tasks = coroutines.Select(coroutine => Launch(coroutine));
            await Task.WhenAll(tasks);
        }

        public async Task CombineFirst(Func<Task>[] coroutines)
        {
            var tasks = coroutines.Select(coroutine => Launch(coroutine));
            await Task.WhenAny(tasks);
        }

        public async Task WaitAllAsync()
        {
            try
            {
                await Task.WhenAll(_tasks);
            }
            catch (Exception ex)
            {
                throw new CoroutineExecutionException("An error occurred while waiting for coroutines to complete.", ex);
            }
        }

        public void Cancel()
        {
            _scopeCancellationTokenSource.Cancel();
        }

        public async ValueTask DisposeAsync()
        {
            Cancel();
            await WaitAllAsync();
            _scopeCancellationTokenSource.Dispose();
        }

        public IEnumerable<Task> GetTasks(CancellationToken cancellationToken)
        {
            return _tasks.Where(task => !task.IsCompleted && !cancellationToken.IsCancellationRequested);
        }
    }
}
