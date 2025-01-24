# Coroutines For CSharp - Detailed Documentation

## Overview

This coroutine library offers an intuitive and flexible approach to asynchronous programming in C#, inspired by Kotlin's coroutine system. It simplifies concurrency by abstracting away the complexity of managing threads and execution contexts. This library provides a powerful model for suspending, resuming, and managing coroutines while ensuring clean and maintainable code. With advanced dispatchers, coroutine scopes, timeouts, and more, it is a comprehensive solution for handling asynchronous workflows in your C# applications.

---

## Features

- **Dispatcher Abstraction**: Choose specific execution contexts, such as `DefaultContext`, `IOContext`, `MainContext`, and `UnconfinedContext`.
- **Coroutine Scopes**: Manage the lifecycle of coroutines with `CoroutineScope` and `CoroutineScopeWithCancellation`.
- **Global Coroutines**: Use `GlobalScope` for long-lived coroutines that are managed application-wide.
- **Timeout Management**: Control execution time of coroutines with `CoroutineTimeout`.
- **Parallel Execution**: Launch multiple coroutines concurrently with `CoroutineBuilder`.
- **Suspension Functions**: Utilize cooperative multitasking with `Suspend` functions to pause execution and resume later.
- **Exception Handling**: Robust handling of exceptions within coroutines (to be covered later).

---

## Installation

### Prerequisites
- .NET Core 3.1 or later
- .NET Framework 4.7.2 or later

### Adding the Library to Your Project

1. Clone or download the repository:
   ```bash
   git clone https://github.com/styropyr0/Coroutines
   ```
2. Add the source files directly to your project, or compile the library into a `.dll` and reference it in your project.

Alternatively, if you prefer to install via .NET CLI or Package Manager:
```bash
dotnet add package CoroutinesForCS --version <version>
```

```bash
NuGet\Install-Package CoroutinesForCS -Version <version>
```
---

## Library Components

## Core classes and methods

### 1. Dispatchers

Dispatchers are central to the coroutine library as they define the execution context for coroutines. Each dispatcher determines where and how coroutines are executed, whether on the default thread pool, a dedicated I/O thread, the main thread, or on any available thread.

#### **`DefaultContext`**
- This dispatcher uses the default thread pool to execute tasks.
- Suitable for CPU-bound tasks and general-purpose work.
- **Methods**:
  - `Task ExecuteAsync(Func<Task> task, CancellationToken cancellationToken)`: Executes a coroutine asynchronously.
  - `Task<T> ExecuteAsync<T>(Func<Task<T>> task, CancellationToken cancellationToken)`: Executes a coroutine asynchronously with a result.
- **Returns**: A new instance of `DefaultContext`.

#### **`IOContext`**
- Designed for I/O-bound operations (such as file access, network requests, etc.).
- Executes tasks on a dedicated thread pool to prevent blocking the main thread.
- **Methods**:
  - `Task ExecuteAsync(Func<Task> task, CancellationToken cancellationToken)`: Executes an I/O-bound task asynchronously.
  - `Task<T> ExecuteAsync<T>(Func<Task<T>> task, CancellationToken cancellationToken)`: Executes an I/O-bound task asynchronously with a result.
- **Returns**: A new instance of `IOContext`.

#### **`MainContext`**
- Executes coroutines on the main thread using a `SynchronizationContext`. This is useful for UI-related tasks that need to interact with the UI thread.
- **Methods**:
  - `Task ExecuteAsync(Func<Task> task, CancellationToken cancellationToken)`: Executes a coroutine on the main thread asynchronously.
  - `Task<T> ExecuteAsync<T>(Func<Task<T>> task, CancellationToken cancellationToken)`: Executes a coroutine on the main thread asynchronously with a result.
- **Returns**: A new instance of `MainContext`.

#### **`UnconfinedContext`**
- Executes coroutines without binding to a specific thread. The coroutine will be resumed on whichever thread is available.
- This context is ideal for operations that do not require a fixed execution thread.
- **Methods**:
  - `Task ExecuteAsync(Func<Task> task, CancellationToken cancellationToken)`: Executes a coroutine without a fixed execution thread.
  - `Task<T> ExecuteAsync<T>(Func<Task<T>> task, CancellationToken cancellationToken)`: Executes a coroutine without a fixed execution thread, returning a result.
- **Returns**: A new instance of `UnconfinedContext`.

---

### Example of Dispatcher Usage:

```csharp
// Creating and using Dispatchers

// DefaultContext
var defaultDispatcher = Dispatcher.Default;
await defaultDispatcher.ExecuteAsync(async () =>
{
    Console.WriteLine("Running on the default context");
});

// IOContext
var ioDispatcher = Dispatcher.IO;
await ioDispatcher.ExecuteAsync(async () =>
{
    Console.WriteLine("Running I/O bound task");
});

// MainContext
var mainDispatcher = Dispatcher.Main;
await mainDispatcher.ExecuteAsync(async () =>
{
    Console.WriteLine("Running on the main thread for UI operations");
});

// UnconfinedContext
var unconfinedDispatcher = Dispatcher.Unconfined;
await unconfinedDispatcher.ExecuteAsync(async () =>
{
    Console.WriteLine("Running in an unconfined context");
});
```


### 2. CoroutineScope

The `CoroutineScope` class is designed to manage and coordinate the execution of coroutines within a specific context or dispatcher. It allows you to launch multiple coroutines, handle cancellations, and wait for their completion in an organized manner.

#### **Key Features:**
- **Manage Coroutine Execution**: Provides methods to launch coroutines that can run in parallel or sequentially.
- **Scope-based Cancellation**: Supports cancellation of all coroutines within the scope.
- **Task Coordination**: Wait for all launched tasks to complete or cancel them when needed.
- **Combine Coroutines**: Allows combining multiple coroutines into one operation, waiting for all or the first one to complete.

#### **Constructor:**
- `CoroutineScope(Dispatcher context)`  
  Initializes a new instance of the `CoroutineScope` class with a specified dispatcher. The dispatcher defines where the coroutines will run (e.g., on the main thread, I/O thread, etc.).

#### **Methods:**

- **Launch (Action)**  
  Launches a coroutine that takes no parameters and returns no value.  
  ```csharp
  public async Task Launch(Action coroutine, Dispatcher dispatcher = null)
  ```

- **Launch (Func<T>)**  
  Launches a coroutine that takes a value and returns that value.  
  ```csharp
  public async Task Launch<T>(Func<T> coroutine, Dispatcher dispatcher = null)
  ```

- **Launch (Func<Task>)**  
  Launches a coroutine that returns a `Task` and waits for its completion.  
  ```csharp
  public async Task Launch(Func<Task> coroutine, Dispatcher dispatcher = null)
  ```

- **Combine (Action)**  
  Combines multiple coroutines that take no parameters and run them in parallel.  
  ```csharp
  public async Task Combine(IEnumerable<Action> coroutines, Dispatcher dispatcher = null)
  ```

- **Combine (Func<T>)**  
  Combines multiple coroutines that take a value and run them in parallel.  
  ```csharp
  public async Task Combine<T>(IEnumerable<Func<T>> coroutines, Dispatcher dispatcher = null)
  ```

- **Combine (Func<Task>)**  
  Combines multiple coroutines that return a `Task` and runs them in parallel.  
  ```csharp
  public async Task Combine(IEnumerable<Func<Task>> coroutines, Dispatcher dispatcher = null)
  ```

- **CombineFirst (Action)**  
  Combines multiple coroutines and executes the first one to complete.  
  ```csharp
  public async Task CombineFirst(IEnumerable<Action> coroutines, Dispatcher dispatcher = null)
  ```

- **CombineFirst (Func<Task>)**  
  Combines multiple coroutines that return a `Task` and executes the first one to complete.  
  ```csharp
  public async Task CombineFirst(IEnumerable<Func<Task>> coroutines, Dispatcher dispatcher = null)
  ```

- **WaitAllAsync**  
  Waits for all coroutines to complete in the current scope.  
  ```csharp
  public async Task WaitAllAsync()
  ```

- **Cancel**  
  Cancels all coroutines in the current scope.  
  ```csharp
  public void Cancel()
  ```

- **DisposeAsync**  
  Disposes of the `CoroutineScope`, cancelling any ongoing coroutines and waiting for them to finish.  
  ```csharp
  public async ValueTask DisposeAsync()
  ```

- **GetTasks**  
  Gets the collection of tasks that are still running and not cancelled.  
  ```csharp
  public IEnumerable<Task> GetTasks(CancellationToken cancellationToken)
  ```

---

#### Example Usage of `CoroutineScope`:

```csharp
// Initialize the CoroutineScope with a specific dispatcher
var scope = new CoroutineScope(Dispatcher.IO);

// Launch a coroutine that takes no parameters
await scope.Launch(() =>
{
    Console.WriteLine("Running a simple coroutine.");
});

// Launch a coroutine that returns a result
await scope.Launch(() =>
{
    return 42; // Example returning a result
});

// Combine multiple coroutines to run in parallel
await scope.Combine(new List<Action>
{
    () => Console.WriteLine("First parallel task"),
    () => Console.WriteLine("Second parallel task")
});

// Wait for all tasks in the scope to complete
await scope.WaitAllAsync();

// Cancel all running coroutines
scope.Cancel();

// Dispose of the scope when done
await scope.DisposeAsync();
```


### 3. GlobalScope

`GlobalScope` provides global access to coroutine scopes and utilities for launching, combining, and managing coroutines. It acts as a container for coroutines, handling their execution across different dispatcher contexts. This section explains how to use `GlobalScope` to manage coroutine tasks globally.

### Methods:

#### 3.1 Launching Coroutines

- **Launch No Parameters**:
  ```csharp
  public static async Task Launch(Action coroutine, Dispatcher dispatcher = null, CancellationToken cancellationToken = default)
  ```
  Launches a coroutine with no parameters (an `Action`) in the given `dispatcher` context. If no dispatcher is provided, the default dispatcher is used.

- **Launch With Return Value**:
  ```csharp
  public static async Task Launch<T>(Func<T> coroutine, Dispatcher dispatcher = null, CancellationToken cancellationToken = default)
  ```
  Launches a coroutine that returns a value of type `T`.

- **Launch Task Coroutine**:
  ```csharp
  public static async Task Launch(Func<Task> coroutine, Dispatcher dispatcher = null, CancellationToken cancellationToken = default)
  ```
  Launches a coroutine that returns a `Task` and waits for its completion.

#### 3.2 Combining Coroutines

- **Combine No Parameters**:
  ```csharp
  public static async Task Combine(IEnumerable<Action> coroutines, Dispatcher dispatcher = null, CancellationToken cancellationToken = default)
  ```
  Combines multiple `Action` coroutines and runs them in parallel.

- **Combine With Return Values**:
  ```csharp
  public static async Task Combine<T>(IEnumerable<Func<T>> coroutines, Dispatcher dispatcher = null, CancellationToken cancellationToken = default)
  ```
  Combines multiple coroutines that return values of type `T` and runs them in parallel.

- **Combine Task Coroutines**:
  ```csharp
  public static async Task Combine(IEnumerable<Func<Task>> coroutines, Dispatcher dispatcher = null, CancellationToken cancellationToken = default)
  ```
  Combines multiple `Task`-returning coroutines and runs them in parallel.

- **Combine First to Complete**:
  ```csharp
  public static async Task CombineFirst(IEnumerable<Action> coroutines, Dispatcher dispatcher = null, CancellationToken cancellationToken = default)
  ```
  Combines multiple `Action` coroutines and executes the first one to complete.

- **Combine Task First to Complete**:
  ```csharp
  public static async Task CombineFirst(IEnumerable<Func<Task>> coroutines, Dispatcher dispatcher = null, CancellationToken cancellationToken = default)
  ```
  Combines multiple `Task`-returning coroutines and executes the first one to complete.

#### 3.3 Managing Coroutine Execution

- **Wait for All Coroutines**:
  ```csharp
  public static async Task WaitAllAsync(CancellationToken cancellationToken = default)
  ```
  Waits for all coroutines in the global scope to complete.

- **Cancel All Coroutines**:
  ```csharp
  public static void CancelAll()
  ```
  Cancels all running coroutines in the global scope.

- **Dispose All Scopes**:
  ```csharp
  public static async Task DisposeAllAsync()
  ```
  Disposes of all coroutine scopes in the global scope asynchronously.


### 4. CoroutineBuilder

`CoroutineBuilder` provides utility methods for launching and managing coroutines with dispatchers. It simplifies launching coroutine blocks and executing multiple coroutines in parallel, handling their execution on specific dispatchers. This section explains how to use `CoroutineBuilder` for coroutine management.

### Methods:

#### 4.1 Launch a Coroutine Block

```csharp
public static async Task Launch(Func<Task> block, Dispatcher dispatcher = null)
```
Launches a single coroutine block using the specified `dispatcher`. If no dispatcher is provided, the default dispatcher is used.

- **Parameters**:
  - `block`: The coroutine block to execute.
  - `dispatcher`: The dispatcher context to execute the coroutine block. If `null`, the default dispatcher is used.
  
- **Returns**:
  - A `Task` representing the asynchronous operation.

#### 4.2 Launch Multiple Coroutines in Parallel (Asynchronous)

```csharp
public static async Task LaunchAll(IEnumerable<Func<Task>> blocks, Dispatcher dispatcher = null)
```
Executes multiple asynchronous coroutines in parallel and waits for all to complete.

- **Parameters**:
  - `blocks`: The collection of coroutine blocks (of type `Func<Task>`) to execute in parallel.
  - `dispatcher`: The dispatcher context for executing the coroutines. If `null`, the default dispatcher is used.
  
- **Returns**:
  - A `Task` representing the asynchronous operation that completes once all coroutines have finished.

#### 4.3 Launch Multiple Coroutines in Parallel (Synchronous)

```csharp
public static async Task LaunchAll(IEnumerable<Action> blocks, Dispatcher dispatcher = null)
```
Executes multiple synchronous coroutines (functions of type `Action`) in parallel and waits for all to complete.

- **Parameters**:
  - `blocks`: The collection of synchronous functions (of type `Action`) to execute in parallel.
  - `dispatcher`: The dispatcher context for executing the functions. If `null`, the default dispatcher is used.
  
- **Returns**:
  - A `Task` representing the asynchronous operation that completes once all functions have finished.


### 5. CoroutineTimeout

`CoroutineTimeout` provides utility methods for running coroutines with an applied timeout. If a coroutine does not complete within the specified time limit, a `TimeoutException` is thrown. This section explains how to use `CoroutineTimeout` to execute coroutines with a timeout.

### Methods:

#### 5.1 Run Coroutine with a Timeout (Returning a Value)

```csharp
public static async Task<T> Run<T>(Func<T> coroutine, TimeSpan timeout, Dispatcher dispatcher = null)
```
Runs a coroutine that returns a value and applies a timeout. If the coroutine doesn't finish within the specified timeout, a `TimeoutException` is thrown.

- **Parameters**:
  - `coroutine`: The coroutine to execute, which returns a value of type `T`.
  - `timeout`: The time span after which the coroutine will time out.
  - `dispatcher`: The dispatcher context to execute the coroutine. If `null`, the default dispatcher is used.
  
- **Returns**:
  - The result of the coroutine if it completes within the timeout period.

- **Exceptions**:
  - Throws a `TimeoutException` if the coroutine exceeds the specified timeout.

#### 5.2 Run Coroutine with a Timeout (Returning No Value)

```csharp
public static async Task Run(Action coroutine, TimeSpan timeout, Dispatcher dispatcher = null)
```
Runs a coroutine that takes no parameters and returns no value, with an applied timeout. If the coroutine doesn't finish within the specified timeout, a `TimeoutException` is thrown.

- **Parameters**:
  - `coroutine`: The coroutine to execute, which is a function of type `Action`.
  - `timeout`: The time span after which the coroutine will time out.
  - `dispatcher`: The dispatcher context to execute the coroutine. If `null`, the default dispatcher is used.
  
- **Returns**:
  - A `Task` representing the asynchronous operation.

- **Exceptions**:
  - Throws a `TimeoutException` if the coroutine exceeds the specified timeout.

#### 5.3 Run Coroutine with a Timeout (Returning a Task)

```csharp
public static async Task<T> Run<T>(Func<Task<T>> coroutine, TimeSpan timeout, Dispatcher dispatcher = null)
```
Runs a coroutine that returns a `Task<T>` and applies a timeout. If the coroutine doesn't finish within the specified timeout, a `TimeoutException` is thrown.

- **Parameters**:
  - `coroutine`: The coroutine to execute, which returns a `Task<T>`.
  - `timeout`: The time span after which the coroutine will time out.
  - `dispatcher`: The dispatcher context to execute the coroutine. If `null`, the default dispatcher is used.
  
- **Returns**:
  - The result of the coroutine if it completes within the timeout period.

- **Exceptions**:
  - Throws a `TimeoutException` if the coroutine exceeds the specified timeout.


### 6. CoroutineScopeWithCancellation

`CoroutineScopeWithCancellation` represents a coroutine scope with cancellation support for all coroutines within the scope. This class allows launching coroutines, waiting for them to complete, and canceling them when needed. If any coroutine in the scope is canceled, all others are also canceled.

### Constructor:

#### 6.1 CoroutineScopeWithCancellation(Dispatcher context)

```csharp
public CoroutineScopeWithCancellation(Dispatcher context)
```
- **Parameters**:
  - `context`: The `Dispatcher` to use for the coroutines within the scope.
  
- **Description**:
  Initializes a new instance of the `CoroutineScopeWithCancellation` class with the specified dispatcher.

### Methods:

#### 6.2 Launch Coroutine with Cancellation Support

```csharp
public override async Task Launch(Func<Task> coroutine, Dispatcher dispatcher = null)
```
- **Parameters**:
  - `coroutine`: The coroutine function to run. This should be an `async` function returning `Task`.
  - `dispatcher`: The `Dispatcher` on which the coroutine should run. If `null`, the context dispatcher is used by default.
  
- **Description**:
  Launches a coroutine within the scope. If a cancellation request is made for the scope, the coroutine will be canceled before execution completes.

- **Exceptions**:
  - Throws `OperationCanceledException` if the coroutine is canceled.
  - Throws a generic `Exception` if any error occurs while executing the coroutine.

#### 6.3 Launch Coroutine without Specified Dispatcher

```csharp
public override async Task Launch(Func<Task> coroutine)
```
- **Parameters**:
  - `coroutine`: The coroutine function to run.
  
- **Description**:
  Launches a coroutine within the scope with cancellation support, using the default dispatcher. If a cancellation request is made for the scope, the coroutine will be canceled before execution completes.

- **Exceptions**:
  - Throws `OperationCanceledException` if the coroutine is canceled.
  - Throws a generic `Exception` if any error occurs while executing the coroutine.

#### 6.4 Cancel All Coroutines in the Scope

```csharp
public void CancelAll()
```
- **Description**:
  Cancels all coroutines currently running within the scope. This method can be called when you want to stop all ongoing coroutines in the scope.

#### 6.5 Wait for All Coroutines to Complete with Cancellation Support

```csharp
public new async Task WaitAllAsync()
```
- **Description**:
  Waits for all coroutines within the scope to complete, with support for cancellation. If the scope is canceled, coroutines will stop running.

- **Exceptions**:
  - Throws `OperationCanceledException` if the coroutines are canceled.
  - Throws a `CoroutineExecutionException` if an error occurs while waiting for coroutines to complete.


### 7. Suspend

The `Suspend` class provides methods for suspending a coroutine for a specified duration or until a given condition is met. It offers various ways to delay execution, including waiting for a specific amount of time, waiting for a condition, or suspending for a set number of milliseconds or seconds.

### Methods:

#### 7.1 Suspend For a Duration

```csharp
public static async Task For(TimeSpan duration)
```
- **Parameters**:
  - `duration`: The amount of time to suspend the coroutine.
  
- **Description**:
  Suspends the coroutine for the specified duration.

- **Exceptions**:
  - Throws `ArgumentOutOfRangeException` if the duration is negative.

#### 7.2 Suspend for a Specific Number of Milliseconds

```csharp
public static async Task ForMilliseconds(int milliseconds)
```
- **Parameters**:
  - `milliseconds`: The number of milliseconds to suspend the coroutine.
  
- **Description**:
  Suspends the coroutine for the specified number of milliseconds.

- **Exceptions**:
  - Throws `ArgumentOutOfRangeException` if the milliseconds value is negative.

#### 7.3 Suspend for a Specific Number of Seconds

```csharp
public static async Task ForSeconds(int seconds)
```
- **Parameters**:
  - `seconds`: The number of seconds to suspend the coroutine.
  
- **Description**:
  Suspends the coroutine for the specified number of seconds.

- **Exceptions**:
  - Throws `ArgumentOutOfRangeException` if the seconds value is negative.

#### 7.4 Suspend Until a Condition is Met

```csharp
public static async Task Until(Func<bool> condition, int checkIntervalMilliseconds = 100, int timeoutMilliseconds = -1)
```
- **Parameters**:
  - `condition`: A function that returns a boolean indicating whether the condition is met.
  - `checkIntervalMilliseconds`: The interval (in milliseconds) to check the condition. The default is 100 ms.
  - `timeoutMilliseconds`: The maximum time to wait for the condition to be met, in milliseconds. A value of -1 means no timeout. The default is -1.

- **Description**:
  Suspends the coroutine until the given condition is met. The condition is checked periodically at the specified interval.

- **Exceptions**:
  - Throws `ArgumentNullException` if the condition is `null`.
  - Throws `TimeoutException` if the condition is not met within the specified timeout.

---

## Examples

### Example 1: **Combining GlobalScope, CoroutineBuilder, and Suspend**

This example demonstrates launching multiple coroutines using `GlobalScope` and `CoroutineBuilder`, suspending between operations using `Suspend`, and handling timeouts.

```csharp
using System;
using System.Threading.Tasks;
using Coroutines;

class Program
{
    static async Task Main()
    {
        Console.WriteLine("Combining GlobalScope, CoroutineBuilder, and Suspend example started");

        // Launch multiple coroutines in parallel with CoroutineBuilder
        await CoroutineBuilder.LaunchAll(new Func<Task>[]
        {
            async () =>
            {
                await GlobalScope.Launch(async () =>
                {
                    Console.WriteLine("Coroutine 1 started");
                    await Suspend.ForSeconds(2);
                    Console.WriteLine("Coroutine 1 completed after 2 seconds");
                });
            },
            async () =>
            {
                await GlobalScope.Launch(async () =>
                {
                    Console.WriteLine("Coroutine 2 started");
                    await Suspend.ForSeconds(1);
                    Console.WriteLine("Coroutine 2 completed after 1 second");
                });
            }
        });

        // Adding a timeout example with CoroutineTimeout
        try
        {
            await CoroutineTimeout.Run(async () =>
            {
                await Task.Delay(5000); // Simulate a long-running task
                Console.WriteLine("Long task completed");
            }, TimeSpan.FromSeconds(3)); // Timeout after 3 seconds
        }
        catch (TimeoutException)
        {
            Console.WriteLine("The long task timed out.");
        }

        Console.WriteLine("Combining example completed");
    }
}
```

**Output:**
```
Combining GlobalScope, CoroutineBuilder, and Suspend example started
Coroutine 1 started
Coroutine 2 started
Coroutine 2 completed after 1 second
Coroutine 1 completed after 2 seconds
The long task timed out.
Combining example completed
```

### Example Explanation:
1. **GlobalScope** is used to launch individual coroutines.
2. **CoroutineBuilder** runs multiple coroutines in parallel.
3. **Suspend** is used to introduce delays between tasks.
4. **CoroutineTimeout** is used to handle tasks that might take too long.

---

### Example 2: **Combining CoroutineScopeWithCancellation and CoroutineBuilder**

This example shows how to launch tasks in a `CoroutineScopeWithCancellation`, cancel them, and handle errors.

```csharp
using System;
using System.Threading.Tasks;
using Coroutines;

class Program
{
    static async Task Main()
    {
        Console.WriteLine("Combining CoroutineScopeWithCancellation and CoroutineBuilder example started");

        // Create a new CoroutineScopeWithCancellation
        var scope = new CoroutineScopeWithCancellation(Dispatcher.Default);

        // Launch multiple coroutines inside the scope
        var coroutine1 = scope.Launch(async () =>
        {
            Console.WriteLine("Coroutine 1 started");
            await Suspend.ForSeconds(3); // Simulate long-running task
            Console.WriteLine("Coroutine 1 completed");
        });

        var coroutine2 = scope.Launch(async () =>
        {
            Console.WriteLine("Coroutine 2 started");
            await Suspend.ForSeconds(1);
            Console.WriteLine("Coroutine 2 completed");
        });

        // Cancel all coroutines in the scope after 2 seconds
        await Task.Delay(2000); 
        scope.CancelAll(); // This will cancel coroutine1

        // Wait for the coroutines to complete
        await scope.WaitAllAsync(); 

        Console.WriteLine("Combining example completed");
    }
}
```

**Output:**
```
Combining CoroutineScopeWithCancellation and CoroutineBuilder example started
Coroutine 1 started
Coroutine 2 started
Coroutine 2 completed
Coroutine was canceled.
Combining example completed
```

### Example Explanation:
1. **CoroutineScopeWithCancellation** is used to group multiple coroutines and handle cancellation.
2. **CoroutineBuilder** launches coroutines inside the scope.
3. **Suspend** is used for delays.
4. The scope is canceled after 2 seconds, interrupting the long-running task.

---

### Example 3: **Combining CoroutineTimeout and Suspend with Error Handling**

This example demonstrates combining timeout handling with suspension.

```csharp
using System;
using System.Threading.Tasks;
using Coroutines;

class Program
{
    static async Task Main()
    {
        Console.WriteLine("Combining CoroutineTimeout and Suspend with Error Handling example started");

        // Example with CoroutineTimeout
        try
        {
            await CoroutineTimeout.Run(async () =>
            {
                Console.WriteLine("Starting long task...");
                await Suspend.ForSeconds(5); // Simulate long task
                Console.WriteLine("Long task completed");
            }, TimeSpan.FromSeconds(3)); // Timeout after 3 seconds
        }
        catch (TimeoutException)
        {
            Console.WriteLine("The long task timed out.");
        }

        // Another example with CoroutineTimeout and no timeout
        try
        {
            await CoroutineTimeout.Run(async () =>
            {
                Console.WriteLine("Starting short task...");
                await Suspend.ForSeconds(1); // Short task
                Console.WriteLine("Short task completed");
            }, TimeSpan.FromSeconds(3)); // No timeout
        }
        catch (TimeoutException)
        {
            Console.WriteLine("The short task timed out.");
        }

        Console.WriteLine("Combining example completed");
    }
}
```

**Output:**
```
Combining CoroutineTimeout and Suspend with Error Handling example started
Starting long task...
The long task timed out.
Starting short task...
Short task completed
Combining example completed
```

### Example Explanation:
1. **CoroutineTimeout** is used to limit how long a task can run.
2. **Suspend** is used to simulate long and short tasks.
3. **TimeoutException** is handled to deal with tasks that take too long.

---

### Example 4: **Combining GlobalScope, Suspend, and CoroutineScopeWithCancellation for a Health Monitoring System**

In a more complex scenario, such as monitoring environmental factors (for instance, air quality), we can combine all concepts into one solution.

```csharp
using System;
using System.Threading.Tasks;
using Coroutines;

class HealthMonitor
{
    public async Task StartMonitoring()
    {
        Console.WriteLine("Health Monitor started");

        // Create a CoroutineScopeWithCancellation for the entire health monitoring session
        var monitorScope = new CoroutineScopeWithCancellation(Dispatcher.Default);

        // Launch air quality monitoring coroutine
        var airQualityMonitor = monitorScope.Launch(async () =>
        {
            Console.WriteLine("Monitoring air quality...");
            await Suspend.ForSeconds(2); // Simulate time taken for measuring air quality
            Console.WriteLine("Air quality is good.");
        });

        // Launch temperature monitoring coroutine
        var temperatureMonitor = monitorScope.Launch(async () =>
        {
            Console.WriteLine("Monitoring temperature...");
            await Suspend.ForSeconds(1);
            Console.WriteLine("Temperature is normal.");
        });

        // Simulate a condition for scope cancellation
        await Task.Delay(1500);
        monitorScope.CancelAll(); // Cancel all coroutines

        // Wait for all coroutines to complete or handle cancellation
        await monitorScope.WaitAllAsync();

        Console.WriteLine("Health Monitor session completed");
    }
}

class Program
{
    static async Task Main()
    {
        var monitor = new HealthMonitor();
        await monitor.StartMonitoring();
    }
}
```

**Output:**
```
Health Monitor started
Monitoring air quality...
Monitoring temperature...
Temperature is normal.
Coroutine was canceled.
Health Monitor session completed
```

### Example Explanation:
1. **GlobalScope** is not needed in this scenario, as **CoroutineScopeWithCancellation** is used to group monitoring tasks.
2. **Suspend** is used to simulate the delays in monitoring different parameters.
3. After a delay, **CancelAll** cancels the air quality monitoring coroutine, demonstrating how to manage coroutines within a scope.

---

### Conclusion

This coroutine library provides a robust and flexible framework for managing asynchronous tasks in C#. With support for multiple dispatchers, cancellation, error handling, and scope management, it is a powerful tool for any application requiring efficient asynchronous task management.

For advanced use cases or to contribute, please check the [source code](https://github.com/yourusername/coroutines).
