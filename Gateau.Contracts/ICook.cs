namespace GateauKata;

public interface ICook : ILogger, IStatus
{
    Task Work();
    bool HasWork { get; }

    IPieConfig PieConfig { get;  }
    ICapacity PrepareCapacity { get; }
    ICapacity BakeCapacity { get; }
    ICapacity WrapCapacity { get; }

    int PieTodoCount { get; }
}

public interface ILogger
{
    void log(string message);
}