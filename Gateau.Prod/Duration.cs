namespace GateauKata;

public class Duration : IDuration
{
    public Duration(int seconds)
    {
        this.seconds = seconds;
    }

    public int seconds { get; init; }
    public int milliSeconds => seconds * 1000;
}