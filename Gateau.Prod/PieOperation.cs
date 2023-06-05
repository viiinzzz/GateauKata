namespace GateauKata;

public class PieOperation : IOperation {
    public PieOperation(
        IPie pie,
        ICapacity capacity,
        string START,
        string DONE,
        int durationSeconds, 
        ILogger logger,
        Action done
        )
    {
        Target = pie;
        Capacity = capacity;
        capacity.Available += (sender, args) =>
        {
            if (!IsStarted) Start(this);
        };
        this.START = START;
        this.DONE = DONE;
        Duration = new Duration(durationSeconds);
        Logger = logger;
        Done = done;
        IsStarted = false;
    }

    public IPrintable Target { get; init; }
    private ICapacity Capacity { get; init; }
    public string START { get; init; }
    public string DONE { get; init; }
    public IDuration Duration { get; init; }
    private ILogger Logger { get; init; }
    public Action Done { get; init; } 
    public bool IsStarted { get; private set; }

    public event DoneEventHandler WhenDone;

    public bool Start(object sender)
    {
        if (IsStarted)
        {
            throw new OperationException(
                "already started");
        }

        if (!Capacity.Consume(this))
        {
            return false;
        }
        
        IsStarted = true;
        DoIt(sender);
        return true;
    }

    public async Task DoIt(object sender)
    {
        Logger.log($"{Target.Label}: {START}");

        await Task.Delay(Duration.milliSeconds);

        //Logger.log($"{Target.Label}: {DONE}");
        Capacity.Release(this);

        Done();
        WhenDone?.Invoke(sender, new DoneEventArgs(this));
    }

}