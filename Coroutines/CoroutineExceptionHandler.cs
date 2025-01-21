using System;

namespace Coroutines
{
    public static class CoroutineExceptionHandler
    {
        public static Action<Exception> HandleCoroutineException = ex =>
        {
            Console.WriteLine($"Unhandled coroutine exception: {ex.Message}");
        };

        public static void Handle(Exception ex)
        {
            HandleCoroutineException?.Invoke(ex);
        }
    }
}
