using Coroutines.CoroutineContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Coroutines
{
    /// <summary>
    /// Represents a coroutine scope with cancellation support for all coroutines within the scope.
    /// </summary>
    public class CoroutineScopeWithCancellation : CoroutineScope
    {
        private CancellationTokenSource _scopeCancellationTokenSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoroutineScopeWithCancellation"/> class.
        /// </summary>
        /// <param name="context">The <see cref="Dispatcher"/> to use for the coroutines.</param>
        public CoroutineScopeWithCancellation(Dispatcher context) : base(context)
        {
            _scopeCancellationTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// Launches a coroutine in the scope with cancellation support.
        /// </summary>
        /// <param name="coroutine">The coroutine function to run.</param>
        /// <param name="dispatcher">The <see cref="Dispatcher"/> on which the coroutine should run. Defaults to the context dispatcher.</param>
        /// <exception cref="OperationCanceledException">Thrown if the coroutine is canceled.</exception>
        /// <exception cref="Exception">Thrown if an error occurs while executing the coroutine.</exception>
        public override async Task Launch(Func<Task> coroutine, Dispatcher dispatcher = null)
        {
            if (coroutine == null) throw new ArgumentNullException(nameof(coroutine));

            dispatcher ??= _context;
            var cancellationToken = _scopeCancellationTokenSource.Token;

            try
            {
                await base.Launch(async () =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await coroutine();
                }, dispatcher);
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

        /// <summary>
        /// Launches a coroutine in the scope with cancellation support.
        /// </summary>
        /// <param name="coroutine">The coroutine function to run.</param>
        /// <exception cref="OperationCanceledException">Thrown if the coroutine is canceled.</exception>
        /// <exception cref="Exception">Thrown if an error occurs while executing the coroutine.</exception>
        public override async Task Launch(Func<Task> coroutine)
        {
            if (coroutine == null) throw new ArgumentNullException(nameof(coroutine));

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

        /// <summary>
        /// Cancels all coroutines within the scope.
        /// </summary>
        public void CancelAll()
        {
            _scopeCancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Waits for all coroutines in the scope to complete, with cancellation support.
        /// </summary>
        /// <exception cref="OperationCanceledException">Thrown if the coroutines are canceled.</exception>
        /// <exception cref="CoroutineExecutionException">Thrown if an error occurs while waiting for coroutines to complete.</exception>
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
