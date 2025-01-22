using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Coroutines;
using Coroutines.CoroutineContext;

namespace CoroutineDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Demo Program - Coroutine Library");

            // Demonstrating basic coroutine execution
            await RunBasicCoroutine();

            // Demonstrating coroutine with cancellation token
            await RunCoroutineWithCancellationToken();

            // Demonstrating combining coroutines
            await RunCombinedCoroutines();

            // Demonstrating synchronization with coroutines
            await RunCoroutinesWithSync();

            // Demonstrating waiting for multiple coroutines
            await RunMultipleCoroutinesAndWait();

            // Demonstrating handling errors in coroutines
            await RunCoroutineWithErrorHandling();

            Console.WriteLine("\nDemo Finished!");
        }

        // Basic coroutine execution with the default dispatcher
        static async Task RunBasicCoroutine()
        {
            Console.WriteLine("\nRunning Basic Coroutine:");
            var scope = new CoroutineScope(Dispatcher.Default);
            await scope.Launch(async () =>
            {
                await Task.Delay(1000); // Simulate an async task
                Console.WriteLine("Basic Coroutine completed.");
            });
        }

        // Coroutine with cancellation token
        static async Task RunCoroutineWithCancellationToken()
        {
            Console.WriteLine("\nRunning Coroutine with Cancellation Token:");

            var cancellationTokenSource = new CancellationTokenSource();
            var scope = new CoroutineScope(Dispatcher.Default);

            var task = scope.Launch(async () =>
            {
                try
                {
                    await Task.Delay(5000, cancellationTokenSource.Token); // Simulating long-running task
                    Console.WriteLine("This message won't be printed if canceled.");
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Coroutine was canceled.");
                }
            });

            // Cancel the coroutine after 2 seconds
            await Task.Delay(2000);
            cancellationTokenSource.Cancel();
        }

        // Combining coroutines using Task.WhenAll (launching coroutines in parallel)
        static async Task RunCombinedCoroutines()
        {
            Console.WriteLine("\nRunning Combined Coroutines:");

            var scope = new CoroutineScope(Dispatcher.Default);
            var task1 = scope.Launch(async () =>
            {
                await Task.Delay(1000); // Simulate task 1
                Console.WriteLine("First coroutine completed.");
            });

            var task2 = scope.Launch(async () =>
            {
                await Task.Delay(1500); // Simulate task 2
                Console.WriteLine("Second coroutine completed.");
            });

            var task3 = scope.Launch(async () =>
            {
                await Task.Delay(2000); // Simulate task 3
                Console.WriteLine("Third coroutine completed.");
            });

            // Wait for all coroutines to complete using Task.WhenAll
            await Task.WhenAll(task1, task2, task3);
            Console.WriteLine("All coroutines completed.");
        }

        // Running coroutines with synchronization
        static async Task RunCoroutinesWithSync()
        {
            Console.WriteLine("\nRunning Coroutines with Synchronization:");

            var scope = new CoroutineScope(Dispatcher.Default);

            // First coroutine simulates a task with a delay
            var task1 = scope.Launch(async () =>
            {
                await Task.Delay(1000);
                Console.WriteLine("First coroutine synchronized.");
            });

            // Second coroutine simulates another task with a delay
            var task2 = scope.Launch(async () =>
            {
                await Task.Delay(500); // Shorter delay for testing synchronization
                Console.WriteLine("Second coroutine synchronized.");
            });

            // Wait for both coroutines to finish in sequence
            await task1;
            await task2;
            Console.WriteLine("Both coroutines finished synchronously.");
        }

        // Waiting for multiple coroutines to complete before continuing
        static async Task RunMultipleCoroutinesAndWait()
        {
            Console.WriteLine("\nRunning Multiple Coroutines and Waiting:");

            var scope = new CoroutineScope(Dispatcher.Default);

            // Launching multiple coroutines
            var task1 = scope.Launch(async () =>
            {
                await Task.Delay(1000); // Simulate async task
                Console.WriteLine("First coroutine completed.");
            });

            var task2 = scope.Launch(async () =>
            {
                await Task.Delay(1500); // Simulate async task
                Console.WriteLine("Second coroutine completed.");
            });

            var task3 = scope.Launch(async () =>
            {
                await Task.Delay(500); // Simulate async task
                Console.WriteLine("Third coroutine completed.");
            });

            // Waiting for all coroutines to finish using WaitAllAsync
            await scope.WaitAllAsync();
            Console.WriteLine("All coroutines finished waiting.");
        }

        // Coroutine with error handling
        static async Task RunCoroutineWithErrorHandling()
        {
            Console.WriteLine("\nRunning Coroutine with Error Handling:");

            var scope = new CoroutineScope(Dispatcher.Default);

            try
            {
                // Simulate a coroutine that throws an exception
                await scope.Launch(async () =>
                {
                    await Task.Delay(500);  // Simulating some work
                    throw new InvalidOperationException("Something went wrong.");
                });
            }
            catch (CoroutineExecutionException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}
