using System.Diagnostics;

namespace GateauKata;

public class Capacity : ICapacity
{
    public Capacity(int nominal)
    {
        Nominal = nominal;
        Current = nominal;
    }
    
    public int Nominal { get; init; }
    public int Current { get; private set; }
    
    public bool IsAvailable => Current > 0;
    public bool IsFull => Current >= Nominal;


    public event ConsumeEventHandler? WhenConsumed;
    public event ReleaseEventHandler? WhenReleased;

    public bool Consume(object sender, IStatus consumer)
    {
        lock (this)
        {
            if (!IsAvailable)
            {
                return false;
            }

            Current--;
            WhenConsumed?.Invoke(sender, new ConsumeEventArgs(consumer));
            return true;
        }
    }

    public bool Release(object sender, IStatus consumer)
    {
        lock (this)
        {
            if (IsFull)
            {
                return false;
            }

            Current++;
            WhenReleased?.Invoke(sender, new ReleaseEventArgs(consumer));
            return true;
        }
    }

    public string Status => $"{Nominal - Current}/{Nominal}";
    public override string ToString() => Status;
}