# Coroutine Library

A C# coroutine library that simplifies asynchronous programming by enabling the management, coordination, and execution of multiple coroutines in parallel with structured exception handling, cancellation support, and flexible task management. Inspired by Kotlin coroutines, this library abstracts away complex task control flows, making it easier to work with concurrent tasks.

## Features

- Launch and manage coroutines of various types: `Action`, `Func<Task>`, `Func<T>`, `Action<T>`, and `Action<object>`.
- Combine multiple coroutines and execute them in parallel.
- Centralized exception handling for all coroutines.
- Global cancellation control for all coroutines.
- Support for both synchronous and asynchronous task execution.
- A simple and clear API for managing complex workflows.

## Installation

To use the Coroutine library in your project, you can either download the source code directly from GitHub or install it via NuGet (if available).

```bash
Install-Package CoroutineLibrary
```

Or you can add the library as a submodule in your project.

## Usage

### Launch a Simple Coroutine

You can launch coroutines using the `GlobalScope.Launch` method. It supports several types of tasks.

#### Launch an `Action` (No parameters, no return value)

```csharp
public static async Task Main(string[] args)
{
    await GlobalScope.Launch(() => 
    {
        Console.WriteLine("Action coroutine executed");
    });
}
```

#### Launch a `Func<Task>` (Asynchronous task)

```csharp
public static async Task Main(string[] args)
{
    await GlobalScope.Launch(async () => 
    {
        await Task.Delay(1000);
        Console.WriteLine("Async coroutine executed");
    });
}
```

#### Launch a `Func<T>` (Returns a value)

```csharp
public static async Task Main(string[] args)
{
    var result = await GlobalScope.Launch(() => 
    {
        return "Coroutine result";
    });

    Console.WriteLine(result);
}
```

#### Launch a Coroutine with a Parameter (`Action<T>`)

```csharp
public static async Task Main(string[] args)
{
    await GlobalScope.Launch((int value) => 
    {
        Console.WriteLine($"Coroutine with parameter: {value}");
    }, 42);
}
```

#### Launch a Coroutine with an Object Parameter (`Action<object>`)

```csharp
public static async Task Main(string[] args)
{
    await GlobalScope.Launch((object obj) => 
    {
        Console.WriteLine($"Coroutine with object parameter: {obj}");
    }, "Hello World");
}
```

### Combine Multiple Coroutines

You can also combine multiple coroutines and run them in parallel.

```csharp
public static async Task Main(string[] args)
{
    await GlobalScope.Combine(new List<Action>
    {
        () => Console.WriteLine("Coroutine 1"),
        () => Console.WriteLine("Coroutine 2"),
        () => Console.WriteLine("Coroutine 3")
    });
}
```

### Cancellation

You can cancel all coroutines globally.

```csharp
public static async Task Main(string[] args)
{
    var cancellationToken = new CancellationTokenSource();

    // Launch some coroutines
    await GlobalScope.Launch(async () => 
    {
        await Task.Delay(2000);
        Console.WriteLine("Coroutine 1 completed");
    }, cancellationToken.Token);

    await GlobalScope.Launch(async () => 
    {
        await Task.Delay(3000);
        Console.WriteLine("Coroutine 2 completed");
    }, cancellationToken.Token);

    // Cancel all coroutines
    cancellationToken.Cancel();

    // Wait for all tasks to complete
    await GlobalScope.WaitAllAsync(cancellationToken.Token);
}
```

### Error Handling

Coroutines support global error handling. All uncaught exceptions will be forwarded to the `CoroutineExceptionHandler`.

```csharp
public static async Task Main(string[] args)
{
    await GlobalScope.Launch(async () => 
    {
        throw new Exception("An error occurred");
    });
}
```

You can implement custom error handling by creating your own `CoroutineExceptionHandler`.

```csharp
public class CustomCoroutineExceptionHandler : ICoroutineExceptionHandler
{
    public void Handle(Exception ex)
    {
        Console.WriteLine($"Custom exception handler: {ex.Message}");
    }
}
```

## API

### GlobalScope

The `GlobalScope` provides the main interface for launching and managing coroutines.

- `Launch(Func<Task> coroutine, Dispatcher dispatcher = null, CancellationToken cancellationToken = default)`
- `Launch(Action coroutine, Dispatcher dispatcher = null, CancellationToken cancellationToken = default)`
- `Launch(Func<T> coroutine, Dispatcher dispatcher = null, CancellationToken cancellationToken = default)`
- `Combine(IEnumerable<Action> coroutines, Dispatcher dispatcher = null, CancellationToken cancellationToken = default)`
- `Combine(IEnumerable<Func<T>> coroutines, Dispatcher dispatcher = null, CancellationToken cancellationToken = default)`
- `WaitAllAsync(CancellationToken cancellationToken = default)`
- `CancelAll()`
- `DisposeAllAsync()`

### CoroutineScope

Manages individual coroutines and provides the ability to execute them with a specific dispatcher.

### Dispatcher

A dispatcher is responsible for executing coroutines. You can create custom dispatchers or use the default dispatcher.

### CoroutineExceptionHandler

Handles exceptions that occur during coroutine execution.

## Advanced Topics

### Combining Coroutines in Parallel

You can combine multiple coroutines and execute them concurrently, making it easier to handle multiple asynchronous tasks at once.

### Coroutine Context

You can pass a specific dispatcher for executing coroutines, allowing you to control the thread or execution context in which the coroutine runs.

## Conclusion

The Coroutine Library provides a simple yet powerful way to manage coroutines in C#. It improves concurrency management, handles exceptions centrally, and provides a clean API for launching, combining, and synchronizing tasks in your applications.
