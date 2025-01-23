using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Coroutines;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Starting Matrix Modification Demo...");

        int[,] matrix = new int[10, 10];
        InitializeMatrix(matrix);

        Console.WriteLine("Original Matrix:");
        PrintMatrix(matrix);

        Stopwatch stopwatch = Stopwatch.StartNew();

        var coroutine1 = async () => { await ModifyMatrix(matrix, 0, 3, "Coroutine 1"); };
        var coroutine2 = async () => { await ModifyMatrix(matrix, 3, 6, "Coroutine 2"); };
        var coroutine3 = async () => { await ModifyMatrix(matrix, 6, 8, "Coroutine 3"); };
        var coroutine4 = async () => { await ModifyMatrix(matrix, 8, 10, "Coroutine 4"); };

        await GlobalScope.Combine(new[] { coroutine1, coroutine2, coroutine3, coroutine4 });

        stopwatch.Stop();
        Console.WriteLine($"Matrix modification completed in {stopwatch.ElapsedMilliseconds} ms.");

        Console.WriteLine("Modified Matrix:");
        PrintMatrix(matrix);
    }

    static void InitializeMatrix(int[,] matrix)
    {
        int value = 1;
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                matrix[i, j] = value++;
            }
        }
    }

    static async Task ModifyMatrix(int[,] matrix, int startRow, int endRow, string coroutine)
    {
        for (int i = startRow; i < endRow; i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                matrix[i, j] += 10;
                Console.WriteLine($"{coroutine} modifying cell [{i}, {j}] -> {matrix[i, j]}");
            }
        }
    }

    static void PrintMatrix(int[,] matrix)
    {
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                Console.Write($"{matrix[i, j],4}");
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }
}
