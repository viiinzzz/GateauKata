namespace GateauKata;

public interface IStartable
{
    bool IsStarted { get; }
    bool Start(object sender);
}
public interface IOperation : IStartable
{
    IDuration Duration { get; init; }

    event StartedEventHandler WhenStarted;
    event DoneEventHandler WhenDone;
}