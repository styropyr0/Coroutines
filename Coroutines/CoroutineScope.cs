using System;
using System.Threading;
using System.Threading.Tasks;

namespace Coroutines
{
    public class CoroutineScope
    {
        private readonly Dispatcher _context;

        public CoroutineScope(Dispatcher context)
        {
            _context = context;
        }

        public void Launch(Func<Task> coroutine, CancellationToken cancellationToken = default)
        {
            try
            {
                _context.ExecuteAsync(coroutine, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new CoroutineExecutionException("An error occurred while executing the coroutine.", ex);
            }
        }

        public async Task WaitAllAsync(CancellationToken cancellationToken = default)
        {
            var tasks = _context.GetTasks(cancellationToken);
            try
            {
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                throw new CoroutineExecutionException("An error occurred while waiting for coroutines to complete.", ex);
            }
        }
    }
}
