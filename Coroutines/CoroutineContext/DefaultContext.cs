using System;
using System.Threading;
using System.Threading.Tasks;
using Coroutines;

namespace Coroutines.CoroutineContext
{
    public class DefaultContext : Dispatcher
    {
        public override Task ExecuteAsync(Func<Task> task, CancellationToken cancellationToken)
        {
            return Task.Run(async () =>
            {
                try
                {
                    await task();
                }
                catch (Exception ex)
                {
                    throw new CoroutineExecutionException("Error in default background context.", ex);
                }
            }, cancellationToken);
        }
    }
}
