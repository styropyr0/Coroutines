using System;
using System.Threading;
using System.Threading.Tasks;

namespace Coroutines
{
    public static class CoroutineBuilder
    {
        public static async Task Launch(Func<Task> block, Dispatcher dispatcher = null)
        {
            dispatcher = dispatcher ?? Dispatcher.Default;
            var scope = new CoroutineScope(dispatcher);
            await scope.Launch(block);
        }

        public static async Task<T> Async<T>(Func<Task<T>> block, Dispatcher dispatcher = null)
        {
            dispatcher = dispatcher ?? Dispatcher.Default;
            var scope = new CoroutineScope(dispatcher);
            var result = await block();
            return result;
        }
    }
}
