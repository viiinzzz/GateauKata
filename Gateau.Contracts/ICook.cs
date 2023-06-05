namespace GateauKata;

public interface ICook : ILogger, IStatus
{
    void Work();
    bool HasWork { get; }

    ICapacity prepareCapacity { get; init; }
    ICapacity bakeCapacity { get; init; }
    ICapacity wrapCapacity { get; init; }
    int PieTodoCount { get; init; }
}

public interface ILogger
{
    void log(string message);
}