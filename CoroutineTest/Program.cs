using System;
using System.Collections.Generic;
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
            // Create a cancellation token source for global scope cancellation
            var globalCancellationTokenSource = new CancellationTokenSource();
            var globalCancellationToken = globalCancellationTokenSource.Token;

            // Set up a coroutine scope with cancellation (will be used throughout)
            var scopeWithCancellation = new CoroutineScopeWithCancellation(new IOContext());

            // Demonstrate simple task launching
            Console.WriteLine("Starting basic task...");
            await CoroutineBuilder.Launch(async () =>
            {
                await SimulateTask("Task 1", 2000, globalCancellationToken); // Simulate a task with 2 seconds delay
                Console.WriteLine("Task 1 completed.");
            });

            // Demonstrate task with timeout handling
            Console.WriteLine("\nStarting task with timeout...");
            var timeoutTask = CoroutineBuilder.Launch(async () =>
            {
                await Task.WhenAny(SimulateTask("Task 2 (with timeout)", 5000, globalCancellationToken), Task.Delay(3000, globalCancellationToken)); // 3-second timeout
                if (globalCancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine("Task 2 was canceled due to timeout.");
                }
                else
                {
                    Console.WriteLine("Task 2 completed successfully.");
                }
            });

            // Combining multiple coroutines to run concurrently
            Console.WriteLine("\nLaunching multiple tasks concurrently (Combine)...");

            // Launch multiple tasks, all of which will run concurrently
            await scopeWithCancellation.Combine(new Func<Task>[]
            {
                async () => await SimulateTask("Task 3", 3000, globalCancellationToken), // 3 seconds delay
                async () => await SimulateTask("Task 4", 4000, globalCancellationToken), // 4 seconds delay
                async () => await SimulateTask("Task 5", 2500, globalCancellationToken), // 2.5 seconds delay
            });

            // Task Cancellation with Timeout
            Console.WriteLine("\nLaunching task with cancellation after timeout...");
            var taskWithTimeoutCancellation = CoroutineBuilder.Launch(async () =>
            {
                var task = SimulateTask("Task 6 (with cancellation)", 6000, globalCancellationToken); // Simulate a long task
                await Task.WhenAny(task, Task.Delay(4000, globalCancellationToken)); // Timeout after 4 seconds

                if (globalCancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine("Task 6 was canceled.");
                }
                else
                {
                    Console.WriteLine("Task 6 completed.");
                }
            });

            // Combine multiple tasks and wait for first completion
            Console.WriteLine("\nLaunching tasks and waiting for first completion...");
            await scopeWithCancellation.CombineFirst(new Func<Task>[]
            {
                async () => await SimulateTask("Task 7", 3000, globalCancellationToken),
                async () => await SimulateTask("Task 8", 1000, globalCancellationToken), // This one will finish first
                async () => await SimulateTask("Task 9", 5000, globalCancellationToken),
            });

            // Cancel a scope after waiting a while
            Task.Run(async () =>
            {
                await Task.Delay(2000); // Wait for a while before cancelling the scope
                Console.WriteLine("\nCanceling all tasks in the scope...");
                scopeWithCancellation.CancelAll();
            });

            // Wait for all tasks to complete or be canceled
            Console.WriteLine("\nWaiting for all tasks to complete...");
            await scopeWithCancellation.WaitAllAsync();

            // Task with custom dispatcher context (e.g., MainContext for UI thread simulation)
            Console.WriteLine("\nLaunching task in MainContext...");
            var mainContextScope = new CoroutineScope(new MainContext());
            await mainContextScope.Launch(async () =>
            {
                Console.WriteLine("Performing work on the MainContext (simulated UI thread).");
                await Task.Delay(1000); // Simulate work
                Console.WriteLine("Work completed on the MainContext.");
            });

            // Handling async result with CoroutineBuilder (return value)
            var result = await CoroutineBuilder.Async(async () =>
            {
                Console.WriteLine("\nSimulating asynchronous task that returns a result...");
                await Task.Delay(1500); // Simulate processing
                return "Processing Completed!";
            });
            Console.WriteLine($"Result from async task: {result}");

            // Example of complex combination of tasks (with cancellation support and timeout)
            Console.WriteLine("\nDemonstrating complex task combination with cancellation and timeout...");
            var complexScope = new CoroutineScopeWithCancellation(new IOContext());
            await complexScope.Combine(new Func<Task>[]
            {
                async () => await SimulateTask("Task 10", 4000, globalCancellationToken),
                async () => await SimulateTask("Task 11", 2500, globalCancellationToken),
                async () => await SimulateTask("Task 12 (timeout test)", 7000, globalCancellationToken),
            });

            // Cleanup
            globalCancellationTokenSource.Dispose();
            Console.WriteLine("\nDemo completed.");
        }

        // Simulate a basic task with a custom message and a delay
        private static async Task SimulateTask(string taskName, int delayMilliseconds, CancellationToken cancellationToken)
        {
            Console.WriteLine($"{taskName} started...");
            await Task.Delay(delayMilliseconds, cancellationToken); // Simulating task with delay
            cancellationToken.ThrowIfCancellationRequested(); // Check for cancellation
            Console.WriteLine($"{taskName} completed after {delayMilliseconds / 1000} seconds.");
        }
    }
}
