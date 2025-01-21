using System;

namespace Coroutines
{
    public class CoroutineExecutionException : Exception
    {
        public CoroutineExecutionException(string message) : base(message) { }

        public CoroutineExecutionException(string message, Exception innerException) : base(message, innerException) { }
    }
}
