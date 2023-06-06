namespace GateauKata;

public enum PieStatus {
    NotStarted,
    Started,
    Preparing,
    Prepared,
    Baking,
    Baked,
    Wrapping,
    Wrapped,
    Disposing,
    Disposed
}

public class Pie : IPie, IDisposable
{
    public int orderReference { get; init; }
    public List<IOperation> Operations { get; init; }
    public IPrintable Target { get; init; }
    public IDuration Duration { get; init; }
    public ILogger Logger { get; init; }
    public PieStatus PieStatus { get; private set; }
    public string Status => $"{PieStatus}";
    public string padStatus => pad(PieStatus);

    public static string pad(PieStatus status)
        => status switch
        {
            PieStatus.NotStarted => "..🔴",
            PieStatus.Started => "  🔴",
            PieStatus.Preparing => "  ..🥣",
            PieStatus.Prepared => "    🥣",
            PieStatus.Baking => "          ..♨️",
            PieStatus.Baked => "            ♨️",
            PieStatus.Wrapping => "               ..📦",
            PieStatus.Wrapped => "                 📦",
            PieStatus.Disposing => "                              ..🏁",
            PieStatus.Disposed => "                                🏁",
            _ => "??",
        };

    private bool transition(PieStatus from, PieStatus to)
    {
        if (PieStatus != from)
        {
            // Debug.WriteLine($"${Label} Invalid transition ${from}->${to} currently ${PieStatus}");
            return false;
        }
        PieStatus = to;
        Logger.log($"{Label} {padStatus}");
        return true;
    }

    public Pie(int orderReference, ICook cook)
    {
        this.orderReference = orderReference;
        Target = this;
        Logger = cook;

        PieStatus = PieStatus.NotStarted;

        var Wrap = new PieOperation(this, cook.WrapCapacity, cook.PieConfig.WrapSeconds);
        var Bake = new PieOperation(this, cook.BakeCapacity, cook.PieConfig.BakeSeconds);
        var Prepare = new PieOperation(this, cook.PrepareCapacity, cook.PieConfig.PrepareSeconds);

        Operations = new (new[] {
            Prepare, 
            Bake, 
            Wrap
        });

        Prepare.WhenStarted += (sender, args) => transition(PieStatus.Started, PieStatus.Preparing);
        Bake.WhenStarted += (sender, args) => transition(PieStatus.Prepared, PieStatus.Baking);
        Wrap.WhenStarted += (sender, args) => transition(PieStatus.Baked, PieStatus.Wrapping);

        Prepare.WhenDone += (sender, args) =>
        {
            transition(PieStatus.Preparing, PieStatus.Prepared);

            Bake.Start(cook);
        };
        Bake.WhenDone += (sender, args) =>
        {
            transition(PieStatus.Baking, PieStatus.Baked);

            Wrap.Start(cook);
        };

        Wrap.WhenDone += (sender, args) =>
        {
            transition(PieStatus.Wrapping, PieStatus.Wrapped);

            var AllOperations = this;
            WhenDone?.Invoke(cook, new DoneEventArgs(AllOperations));
        };

        Duration = new Duration(
            Operations.Sum(op => op.Duration.seconds));
    }

    public void Dispose()
        => Disposing();
    public bool Disposing()
    {
        if (PieStatus == PieStatus.Disposing || PieStatus == PieStatus.Disposed)
        {
            return true;
        }

        return transition(PieStatus.Wrapped, PieStatus.Disposing);
    }

    ~Pie()
    {
        transition(PieStatus.Disposing, PieStatus.Disposed);
    }

    public bool Start(object sender)
    {
        transition(PieStatus.NotStarted, PieStatus.Started);

        return Operations.FirstOrDefault()?.Start(sender)
               ?? false;
    }

    public bool IsStarted => 
        Operations.FirstOrDefault()?.IsStarted
        ?? false;


    public event StartedEventHandler? WhenStarted;
    public event DoneEventHandler? WhenDone;

    public string Label => $"Pie#{orderReference:00}";
    public override string ToString() => $"{Label} {padStatus}";

}