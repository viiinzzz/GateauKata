using System.Diagnostics;

namespace GateauKata;

public class Logger : ILogger
{
    // public Logger()
    // {
    //     for (int i = 0; i < 20; i++)
    //     {
    //         Debug.WriteLine("");
    //     }
    // }

    public void log(string message)
    {
        Debug.WriteLine(message);
    }
}