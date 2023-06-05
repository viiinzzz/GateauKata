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


    public event AvailableEventHandler? Available;

    public bool Consume(object sender)
    {
        if (!IsAvailable)
        {
            return false;
        }

        Current--;
        return true;
    }

    public void Release(object sender)
    {
        if (IsFull)
        {
            throw new CapacityFullException();
        }

        Current++;

        Available?.Invoke(sender, new AvailableEventArgs());
    }

    public string Status => $"{Nominal - Current}/{Nominal}";
    public override string ToString() => Status;
}