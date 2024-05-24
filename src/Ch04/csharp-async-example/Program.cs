using System.Diagnostics;

namespace csharp_async_example;

public abstract class Program
{
    private static Stopwatch? Stopwatch { get; set; }

    static void Main(string[] args)
    {
        Stopwatch = new Stopwatch();
        Stopwatch.Start();
        Console.WriteLine($"[Kitchen] Started making breakfast - Synchronous Chef");

        MakeBreakfastWithSynchronousChef();

        Console.WriteLine($"[Kitchen] Breakfast done in '{Stopwatch.Elapsed.Seconds}'s '{Stopwatch.Elapsed.Milliseconds}'ms");
        Stopwatch.Restart();

        Console.WriteLine("---");
        Console.WriteLine($"[Kitchen] Started making breakfast - Asynchronous Chef");

        MakeBreakfastWithAsynchronousChef().Wait();

        Console.WriteLine($"[Kitchen] Breakfast done in '{Stopwatch.Elapsed.Seconds}'s '{Stopwatch.Elapsed.Milliseconds}'ms");
    }

    static void MakeBreakfastWithSynchronousChef()
    {
        var chef = new SynchronousChef();
        chef.MakeBreakfast();
    }

    static async Task MakeBreakfastWithAsynchronousChef()
    {
        var chef = new AsynchronousChef();
        await chef.MakeBreakfast();
    }

    static async Task MakeBreakfastAndDisturbChef()
    {
        var chef = new AsynchronousChef();
        var breakfastTask = chef.MakeBreakfast();

        Task.Delay(1500).Wait();
        Console.WriteLine($"[Kitchen] a joke from chef: '{chef.TellAJoke().Result}'");

        Task.Delay(9000).Wait();
        chef.PourJuice().Wait();

        await breakfastTask;
    }
}