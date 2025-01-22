# Coroutine Library - Detailed Documentation

## Overview

This Coroutine Library is a lightweight asynchronous task management system for C# inspired by Kotlin Coroutines. It provides a flexible framework to handle asynchronous tasks in a structured manner. It includes features like context switching, cancellation support, error handling, and custom dispatchers to handle different types of tasks.

### Features:
- **Coroutine Scope**: Group and manage multiple coroutines with easy lifecycle management.
- **Multiple Dispatchers**: Choose from different dispatchers optimized for different execution contexts (e.g., IO-bound tasks, main thread tasks).
- **Cancellation Support**: Cancel all coroutines within a scope simultaneously, or cancel individual tasks.
- **Flexible Context Handling**: Switch between various contexts (e.g., main thread, IO, unconfined) to execute coroutines efficiently.
- **Error Handling**: Built-in exception handling to ensure safe execution of coroutines.
- **Task Execution Control**: Explicit control over how and where coroutines run using custom dispatchers.

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

Alternatively, if you prefer to install via NuGet (if the package is made public):
```bash
Install-Package CoroutineLibrary
```

## Library Components

### Core Classes and Components

#### `CoroutineScope`
A **CoroutineScope** is used to manage coroutines. It ensures that coroutines are executed together and can be canceled or waited for completion. The scope acts as a container that handles the lifecycle of coroutines and ensures they are executed on a specific dispatcher.

##### Constructor

```csharp
public CoroutineScope(Dispatcher dispatcher)
```

- **dispatcher**: The dispatcher to run coroutines on (e.g., `Dispatcher.Default`, `Dispatcher.IOContext`).

##### Methods

- **`Launch`**:
  Launches a coroutine within the scope.

  ```csharp
  public async Task Launch(Func<Task> coroutine)
  ```

  Example:
  ```csharp
  var scope = new CoroutineScope(Dispatcher.Default);
  await scope.Launch(async () =>
  {
      await Task.Delay(1000);
      Console.WriteLine("Coroutine completed.");
  });
  ```

- **`WaitAllAsync`**:
  Waits for all coroutines within the scope to complete.

  ```csharp
  public async Task WaitAllAsync()
  ```

- **`CancelAll`**:
  Cancels all coroutines within the scope.

  ```csharp
  public void CancelAll()
  ```

---

#### `CoroutineScopeWithCancellation`
This class extends `CoroutineScope` and adds the ability to cancel all coroutines within the scope.

##### Methods

- **`Launch`**:
  Same as in `CoroutineScope`, but with cancellation support. You can cancel all coroutines launched in this scope.

  ```csharp
  public override async Task Launch(Func<Task> coroutine)
  ```

- **`CancelAll`**:
  Cancels all coroutines within this scope.

  ```csharp
  public void CancelAll()
  ```

---

#### `Dispatcher`
The `Dispatcher` class defines how and where coroutines are executed. There are different dispatcher types optimized for different use cases. A dispatcher determines the synchronization context of a coroutine and can execute it on the main thread, a background thread, or an unconfined context.

##### Abstract Methods

- **`ExecuteAsync`**:
  Executes a coroutine asynchronously using the dispatcher context.

  ```csharp
  public abstract Task ExecuteAsync(Func<Task> task, CancellationToken cancellationToken)
  ```

- **`ExecuteAsync`** (with Result):
  Executes an asynchronous task that returns a result.

  ```csharp
  public abstract Task<T> ExecuteAsync<T>(Func<Task<T>> task, CancellationToken cancellationToken)
  ```

##### Dispatcher Types

1. **`DefaultContext`**: Executes tasks without any special synchronization context. Suitable for general-purpose tasks.
2. **`IOContext`**: Optimized for IO-bound tasks (e.g., network or file operations).
3. **`MainContext`**: Executes tasks on the main thread (typically useful for UI-based applications).
4. **`UnconfinedContext`**: Executes tasks without any synchronization context, allowing tasks to run freely on any available thread.

---

### `CoroutineContextExtensions`
This class provides extension methods to easily execute coroutines within a specified `Dispatcher` context.

#### `WithContext`

The `WithContext` method executes a coroutine within the specified dispatcher context.

```csharp
public static async Task WithContext(this Dispatcher dispatcher, Func<Task> block, CancellationToken cancellationToken = default)
```

```csharp
public static async Task<T> WithContext<T>(this Dispatcher dispatcher, Func<Task<T>> block, CancellationToken cancellationToken = default)
```

Example:

```csharp
await Dispatcher.IOContext.WithContext(async () =>
{
    await Task.Delay(1000);
    Console.WriteLine("Executed in IO context.");
});
```

---

### `CoroutineBuilder`
This is a utility class that provides simpler methods for launching coroutines.

#### Methods

- **`Launch`**:
  Launches a coroutine without worrying about the scope.

  ```csharp
  public static async Task Launch(Func<Task> block, Dispatcher dispatcher)
  ```

- **`CoroutineAsync`**:
  Executes a coroutine that returns a result.

  ```csharp
  public static async Task<T> CoroutineAsync<T>(Func<Task<T>> block, Dispatcher dispatcher)
  ```

---

## Example Usage

### Basic Coroutine Execution

You can launch a simple coroutine using the `CoroutineScope`:

```csharp
var scope = new CoroutineScope(Dispatcher.Default);

await scope.Launch(async () =>
{
    await Task.Delay(1000);  // Simulating async task
    Console.WriteLine("Coroutine finished.");
});
```

### Coroutine with Cancellation Support

Use `CoroutineScopeWithCancellation` to manage coroutines that can be canceled:

```csharp
var scope = new CoroutineScopeWithCancellation(Dispatcher.Default);

await scope.Launch(async () =>
{
    await Task.Delay(1000);  // Simulate long-running task
    Console.WriteLine("This won't print if canceled.");
});

// Cancel all coroutines after 500ms
await Task.Delay(500);
scope.CancelAll();
Console.WriteLine("Coroutines were canceled.");
```

### Custom Dispatcher Usage

You can use custom dispatchers for specific execution contexts:

```csharp
var ioContext = new IOContext();

await ioContext.ExecuteAsync(async () =>
{
    await Task.Delay(1000);  // Simulate an IO-bound task
    Console.WriteLine("Executed in IO context.");
});
```

### Error Handling in Coroutines

Any exceptions thrown within coroutines are caught and thrown as `CoroutineExecutionException`:

```csharp
await scope.Launch(async () =>
{
    throw new Exception("Something went wrong.");
}).ContinueWith(t =>
{
    if (t.Exception != null)
    {
        Console.WriteLine("Error: " + t.Exception.InnerException.Message);
    }
});
```

### Multiple Coroutines in a Scope

You can launch multiple coroutines in a single scope:

```csharp
var scope = new CoroutineScope(Dispatcher.Default);

await scope.Launch(async () =>
{
    await Task.Delay(1000);
    Console.WriteLine("First coroutine completed.");
});

await scope.Launch(async () =>
{
    await Task.Delay(2000);
    Console.WriteLine("Second coroutine completed.");
});

await scope.WaitAllAsync();  // Wait for both coroutines to finish
Console.WriteLine("All coroutines completed.");
```

---

## Error Handling

All coroutines are wrapped in try-catch blocks, so any unhandled exceptions will be captured and thrown as `CoroutineExecutionException`:

```csharp
try
{
    await someScope.Launch(async () =>
    {
        throw new InvalidOperationException("An error occurred.");
    });
}
catch (CoroutineExecutionException ex)
{
    Console.WriteLine("Coroutine failed: " + ex.Message);
}
```

---

## Advanced Features

### Combining Coroutines

You can combine multiple coroutines into one:

```csharp
await scope.Launch(async () =>
{
    await Task.WhenAll(
        Task.Delay(1000),
        Task.Delay(2000)
    );
    Console.WriteLine("All tasks finished.");
});
```

---

### Conclusion

This coroutine library provides a robust and flexible framework for managing asynchronous tasks in C#. With support for multiple dispatchers, cancellation, error handling, and scope management, it is a powerful tool for any application requiring efficient asynchronous task management.

For advanced use cases or to contribute, please check the [source code](https://github.com/yourusername/coroutines).
