using System.Diagnostics;

namespace GateauKata;

public class Logger : ILogger
{

    public void log(string message)
    {
        Console.Out.WriteLine(message);
        Debug.WriteLine(message);
    }
}