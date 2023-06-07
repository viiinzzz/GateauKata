using Gateau.config;
using System.Diagnostics;

namespace GateauKata;

public class Prod
{
    public static async Task Main(string[] args)
    {
        Debug.WriteLine("==GateauKata=======");

        var speedFactor = 0.1;

        var me = new Cook(new CookConfig(
            3,
            4,
            2,
            new PieConfig(
                2 * speedFactor,
                3 * speedFactor,
                1 * speedFactor)), new Logger());

        var work = me.MakePies(100);

        double secondLapse = 0;
        var timestamp = () =>
        {
            var s_int = (int)Math.Round(secondLapse);
            var s = s_int % 60;
            var m = s_int / 60;
            return $"{m:00}:{s:00}";
        };

        var printStatus = () =>
        {
            var doing = me.DoingPies;
            var doingStatus = doing.Count == 0 
                ? "none"
                : doing
                    .Select(pie => $"#{pie.Id:00} {pie.Status}")
                    .Aggregate((x, y) => $"{x} {y}");

            Debug.WriteLine(@$"
[{timestamp()}]
            {doingStatus}
");
        };

        double refreshSeconds = 10.0;
        int s_ms = (int)Math.Round(refreshSeconds * 1000);
        while (!work.IsCompleted)
        {
            printStatus();
            await Task.Delay(s_ms);
            secondLapse += refreshSeconds;
        }

        printStatus();
        Debug.WriteLine("All done.");

    }
}
