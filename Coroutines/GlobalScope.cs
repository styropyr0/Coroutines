using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coroutines.CoroutineContext;

namespace Coroutines
{
    internal class GlobalScope
    {
        private static readonly CoroutineScope _globalScope = new CoroutineScope(Dispatcher.Default);

        public static void Launch(Func<Task> coroutine)
        {
            _globalScope.Launch(coroutine);
        }

        public static void Launch(Dispatcher dispatcher, Func<Task> coroutine)
        {
            var scope = new CoroutineScope(dispatcher);
            scope.Launch(coroutine);
        }
    }
}
