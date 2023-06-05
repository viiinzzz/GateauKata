namespace GateauKata;

public interface IOperation
{
    public IPrintable Target { get; init; }
    IDuration Duration { get; init; }
    public Action Done { get; init; }
    bool IsStarted { get; }
    bool Start(object sender);

    event DoneEventHandler WhenDone;
}