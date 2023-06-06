using System.Diagnostics;

namespace GateauKata;

public class Prod
{
    public static async Task Main(string[] args)
    {
        Debug.WriteLine("==GateauKata=======");
        
        var me = new Cook(
           3, 0.2,
            4, 0.3,
            2, 0.1,
            20
            );

        Debug.WriteLine("Start working...");
        var work = me.Work();

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
                    .Select(pie => $"#{pie.orderReference:00} {pie.Status}")
                    .Aggregate((x, y) => $"{x} {y}");

            Debug.WriteLine(@$"
[{timestamp()}] {me.Status}
            {doingStatus}
");
        };

        double refreshSeconds = 1.0;
        int s_ms = (int)Math.Round(refreshSeconds * 1000);
        while (!work.IsCompleted)
        {
            printStatus();
            await Task.Delay(s_ms);
            secondLapse += refreshSeconds;
        }

        printStatus();
        Debug.WriteLine("All done.");

//         Debug.WriteLine(@"
// Cool down...");
//         await Task.Delay(5000);
//         printStatus();
    }
}
