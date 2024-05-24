namespace csharp_async_example;

public class SynchronousChef
{
    private int DelayMultiply { get; set; } = 100;

    public void MakeBreakfast()
    {
        BoilWater();
        BoilEggs();
        FryBacon();
        ToastBread();
        ApplyButter();
        ApplyJam();
        PourCoffee();
        PourJuice();
    }

    private void PourCoffee()
    {
        Console.WriteLine("[Chef] Start pouring coffee");
        Thread.Sleep(5 * DelayMultiply);
        Console.WriteLine("[Chef] Coffee Ready");
    }

    private void BoilEggs()
    {
        Console.WriteLine("[Chef] Put Eggs into boiling water");
        Thread.Sleep(70 * DelayMultiply);
        Console.WriteLine("[Chef] Eggs Boiled");
    }

    private void FryBacon()
    {
        Console.WriteLine("[Chef] Throw bacon in pan");
        Thread.Sleep(40 * DelayMultiply);
        Console.WriteLine("[Chef] Bacon Fried");
    }

    private void ToastBread()
    {
        Console.WriteLine("[Chef] Put bread in toaster");
        Thread.Sleep(20 * DelayMultiply);
        Console.WriteLine("[Chef] Bread Toasted");
    }

    private void ApplyButter()
    {
        Console.WriteLine("[Chef] Start spreading Butter");
        Thread.Sleep(15 * DelayMultiply);
        Console.WriteLine("[Chef] Butter applied");
    }

    private void ApplyJam()
    {
        Console.WriteLine("[Chef] Start spreading Jam");
        Thread.Sleep(15 * DelayMultiply);
        Console.WriteLine("[Chef] Jam applied");
    }

    private void PourJuice()
    {
        Console.WriteLine("[Chef] Start pouring Juice");
        Thread.Sleep(5 * DelayMultiply);
        Console.WriteLine("[Chef] Juice Ready");
    }

    private void BoilWater()
    {
        Console.WriteLine("[Chef] Set coffee water to boil");
        Thread.Sleep(200 * DelayMultiply);
        Console.WriteLine("[Chef] Coffee Water is boiled");
    }
}