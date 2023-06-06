namespace GateauKata;

public class Duration : IDuration
{
    public Duration(double seconds)
    {
        this.seconds = seconds;
    }

    public double seconds { get; init; }
    public int milliSeconds => (int)Math.Round(seconds * 1000);
}