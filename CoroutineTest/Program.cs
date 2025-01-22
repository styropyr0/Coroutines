using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace CoroutineApiDemo
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            // Initialize the cancellation token
            var cancellationToken = new CancellationToken();

            // Launch multiple coroutines (both sync and async API calls)
            await GlobalScope.Launch(
                async () =>
                {
                    // Launch synchronous API call
                    Console.WriteLine("Starting synchronous API task...");
                    await GlobalScope.Launch(SyncApiCall, cancellationToken);

                    // Launch asynchronous API calls
                    Console.WriteLine("Starting asynchronous API tasks...");
                    var asyncTasks = new List<Task>
                    {
                        GlobalScope.Launch(() => AsyncApiCall("https://jsonplaceholder.typicode.com/posts/1", cancellationToken)),
                        GlobalScope.Launch(() => AsyncApiCall("https://jsonplaceholder.typicode.com/posts/2", cancellationToken))
                    };

                    await Task.WhenAll(asyncTasks);
                }, cancellationToken
            );
        }

        /// <summary>
        /// Makes a synchronous API call that blocks the thread.
        /// </summary>
        public static void SyncApiCall()
        {
            try
            {
                using var client = new HttpClient();
                var response = client.GetStringAsync("https://jsonplaceholder.typicode.com/posts/1").Result; // Blocking call
                Console.WriteLine($"Synchronous API response: {response.Substring(0, 100)}...");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in synchronous API call: {ex.Message}");
            }
        }

        /// <summary>
        /// Makes an asynchronous API call without blocking the thread.
        /// </summary>
        /// <param name="url">The URL for the API call.</param>
        /// <param name="cancellationToken">The cancellation token for cancellation control.</param>
        public static async Task AsyncApiCall(string url, CancellationToken cancellationToken)
        {
            try
            {
                using var client = new HttpClient();
                var response = await client.GetStringAsync(url, cancellationToken); // Non-blocking call
                Console.WriteLine($"Asynchronous API response from {url}: {response.Substring(0, 100)}...");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in asynchronous API call: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Provides global access to coroutine scopes and utilities for launching, combining, and managing coroutines.
    /// </summary>
    public static class GlobalScope
    {
        /// <summary>
        /// Launches a coroutine that handles asynchronous operations.
        /// </summary>
        /// <param name="coroutine">The coroutine to launch.</param>
        /// <param name="cancellationToken">The cancellation token for cancellation control.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task Launch(Func<Task> coroutine, CancellationToken cancellationToken = default)
        {
            if (coroutine == null) throw new ArgumentNullException(nameof(coroutine));

            try
            {
                await coroutine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Coroutine error: {ex.Message}");
            }
        }

        /// <summary>
        /// Launches a coroutine that takes no parameters and returns no value (synchronous).
        /// </summary>
        /// <param name="coroutine">The synchronous coroutine to launch.</param>
        /// <returns>A task representing the operation.</returns>
        public static async Task Launch(Action coroutine, CancellationToken cancellationToken = default)
        {
            if (coroutine == null) throw new ArgumentNullException(nameof(coroutine));

            try
            {
                coroutine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in synchronous coroutine: {ex.Message}");
            }
        }
    }
}
