using System;
using System.Threading;
using System.Threading.Tasks;
using Coroutines;

namespace Coroutines.CoroutineContext
{
    public class UnconfinedContext : Dispatcher
    {
        public override Task ExecuteAsync(Func<Task> task, CancellationToken cancellationToken)
        {
            try
            {
                return task();
            }
            catch (Exception ex)
            {
                throw new CoroutineExecutionException("Error in unconfined execution context.", ex);
            }
        }
    }
}
