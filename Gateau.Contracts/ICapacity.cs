namespace GateauKata;

public interface ICapacity : IStatus
{
    int Nominal { get; }
    int Current { get; }

    bool IsAvailable { get; }
    bool IsFull { get; }

    bool Consume(object sender);
    void Release(object sender);

    event AvailableEventHandler Available;
}