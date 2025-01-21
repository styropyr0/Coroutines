using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Coroutines.CoroutineContext;

namespace Coroutines
{ 
    public abstract class Dispatcher
    {
        private readonly List<Task> _tasks = new List<Task>();

        public abstract Task ExecuteAsync(Func<Task> task, CancellationToken cancellationToken);

        public List<Task> GetTasks(CancellationToken cancellationToken)
        {
            return _tasks.Where(t => !t.IsCompleted).ToList();
        }

        public static Dispatcher Main { get; } = new MainContext();
        public static Dispatcher Default { get; } = new DefaultContext();
        public static Dispatcher IO { get; } = new IOContext();
        public static Dispatcher Unconfined { get; } = new UnconfinedContext();
    }
}
