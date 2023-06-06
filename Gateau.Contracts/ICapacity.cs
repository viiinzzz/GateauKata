namespace GateauKata;

public interface ICapacity : IStatus
{
    int Nominal { get; }
    int Current { get; }

    bool IsAvailable { get; }
    bool IsFull { get; }

    bool Consume(object sender, IStatus consumer);
    bool Release(object sender, IStatus consumer);

    event ConsumeEventHandler WhenConsumed;
    event ReleaseEventHandler WhenReleased;
}