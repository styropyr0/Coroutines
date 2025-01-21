using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Coroutines
{
    public class CoroutineScopeWithCancellation : CoroutineScope
    {
        private CancellationTokenSource _scopeCancellationTokenSource;

        public CoroutineScopeWithCancellation(Dispatcher context) : base(context)
        {
            _scopeCancellationTokenSource = new CancellationTokenSource();
        }

        public override async Task Launch(Func<Task> coroutine, Dispatcher dispatcher = null)
        {
            var cancellationToken = _scopeCancellationTokenSource.Token;

            try
            {
                await base.Launch(async () =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await coroutine();
                });
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Coroutine was canceled.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing coroutine: {ex.Message}");
            }
        }

        public void CancelAll()
        {
            _scopeCancellationTokenSource.Cancel();
        }

        public new async Task WaitAllAsync()
        {
            try
            {
                await base.WaitAllAsync();
            }
            catch (Exception ex)
            {
                if (_scopeCancellationTokenSource.Token.IsCancellationRequested)
                {
                    Console.WriteLine("Scope cancellation requested.");
                }
                else
                {
                    throw new CoroutineExecutionException("An error occurred while waiting for coroutines.", ex);
                }
            }
        }
    }
}
