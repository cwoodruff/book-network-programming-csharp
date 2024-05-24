namespace csharp_async_example;

public class AsynchronousChef
{
    private int DelayMultiply { get; set; } = 100;

    public async Task MakeBreakfast()
    {
        var waterTask = BoilWater();
        var eggsTask = BoilEggs();
        var baconTask = FryBacon();
        var breadTask = ToastBread();

        await breadTask;
        await ApplyButter();
        await ApplyJam();
            
        await Task.WhenAll(eggsTask, baconTask, waterTask);
        await PourCoffee();
        await PourJuice();
    }

    private async Task PourCoffee()
    {
        Console.WriteLine("[Chef] Start pouring coffee");
        await Task.Delay(5 * DelayMultiply);
        Console.WriteLine("[Chef] Coffee Ready");
    }

    private async Task BoilEggs() 
    {
        Console.WriteLine("[Chef] Put Eggs into boiling water");
        await Task.Delay(70 * DelayMultiply);
        Console.WriteLine("[Chef] Eggs Boiled");
    }

    private async Task FryBacon()
    {
        Console.WriteLine("[Chef] Throw bacon in pan");
        await Task.Delay(40 * DelayMultiply);
        Console.WriteLine("[Chef] Bacon Fried");
    }

    private async Task ToastBread()
    {
        Console.WriteLine("[Chef] Put bread in toaster");
        await Task.Delay(20 * DelayMultiply);
        Console.WriteLine("[Chef] Bread Toasted");
    }

    private async Task ApplyButter()
    {
        Console.WriteLine("[Chef] Start spreading Butter");
        await Task.Delay(15 * DelayMultiply);
        Console.WriteLine("[Chef] Butter applied");
    }

    private async Task ApplyJam()
    {
        Console.WriteLine("[Chef] Start spreading Jam");
        await Task.Delay(15 * DelayMultiply);
        Console.WriteLine("[Chef] Jam applied");
    }

    public async Task PourJuice()
    {
        Console.WriteLine("[Chef] Start pouring Juice");
        await Task.Delay(5 * DelayMultiply);
        Console.WriteLine("[Chef] Juice Ready");
    }

    private async Task BoilWater()
    {
        Console.WriteLine("[Chef] Set coffee water to boil");
        await Task.Delay(200 * DelayMultiply);
        Console.WriteLine("[Chef] Coffee Water is boiled");
    }
    
    public async Task<string> TellAJoke()
    {
        return "Joke";
    }
}