using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Coroutines
{
    public static class GlobalScope
    {
        private static readonly ConcurrentDictionary<Dispatcher, CoroutineScope> Scopes = new();

        public static async Task Launch(Dispatcher dispatcher = null, Func<Task> coroutine = null, CancellationToken cancellationToken = default)
        {
            dispatcher = dispatcher ?? Dispatcher.Default;
            if (coroutine == null)
            {
                throw new ArgumentNullException(nameof(coroutine));
            }
            else
            {
                var scope = Scopes.GetOrAdd(dispatcher, new CoroutineScope(dispatcher));
                try
                {
                    await scope.Launch(coroutine);
                }
                catch (Exception ex)
                {
                    CoroutineExceptionHandler.Handle(ex);
                }
            }
        }

        public static async Task Combine(Dispatcher dispatcher = null, IEnumerable<Func<Task>> coroutines = null, CancellationToken cancellationToken = default)
        {
            dispatcher = dispatcher ?? Dispatcher.Default;
            if (coroutines == null)
            {
                throw new ArgumentNullException(nameof(coroutines));
            }
            else
            {
                var scope = Scopes.GetOrAdd(dispatcher, new CoroutineScope(dispatcher));
                try
                {
                    await scope.Combine(coroutines);
                }
                catch (Exception ex)
                {
                    CoroutineExceptionHandler.Handle(ex);
                }
            }
        }

        public static async Task WaitAllAsync(CancellationToken cancellationToken = default)
        {
            var tasks = Scopes.Values.SelectMany(scope => scope.GetTasks(cancellationToken)).ToList();
            if (tasks.Any())
            {
                try
                {
                    await Task.WhenAll(tasks);
                }
                catch (Exception ex)
                {
                    CoroutineExceptionHandler.Handle(ex);
                }
            }
        }

        public static async Task CombineFirstAsync(Dispatcher dispatcher = null, IEnumerable<Func<Task>> coroutines = null, CancellationToken cancellationToken = default)
        {
            dispatcher = dispatcher ?? Dispatcher.Default;
            if (coroutines == null)
            {
                throw new ArgumentNullException(nameof(coroutines));
            }
            else
            {
                var scope = Scopes.GetOrAdd(dispatcher, new CoroutineScope(dispatcher));
                try
                {
                    await scope.CombineFirst(coroutines.ToArray());
                }
                catch (Exception ex)
                {
                    CoroutineExceptionHandler.Handle(ex);
                }
            }
        }

        public static async Task<IEnumerable<Task>> GetTasks(CancellationToken cancellationToken)
        {
            var tasks = Scopes.Values.SelectMany(scope => scope.GetTasks(cancellationToken)).ToList();
            return tasks;
        }
    }
}
