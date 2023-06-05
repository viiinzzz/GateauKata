namespace GateauKata;

public class Prod
{
    public static async Task Main(string[] args)
    {
        Console.Out.WriteLine("==GateauKata=======");

        int prepareCapacityNominal = 3,
            bakeCapacityNominal = 4,
            wrapCapacityNominal = 2,
            pieTodoCount = 100;

        var me = new Cook(
            prepareCapacityNominal,
            bakeCapacityNominal,
            wrapCapacityNominal,
            pieTodoCount
            );

        Console.WriteLine("Start working...");
        me.Work();

        int secondLapse = 0;
        var timestamp = () =>
        {
            var s = secondLapse % 60;
            var m = secondLapse / 60;
            return $"{m:00}:{s:00}";
        };

        var printStatus = () => 
            Console.WriteLine($"[{timestamp()}] {me.Status}");

        while (me.HasWork)
        {
            printStatus();
            await Task.Delay(10000);
            secondLapse++;
        }

        printStatus();
        Console.WriteLine("All done.");
    }
}
