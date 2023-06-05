using System.Buffers;

namespace GateauKata;

public class Pie : IPie
{
    public int orderReference { get; init; }
    public List<IOperation> Operations { get; init; }
    public IPrintable Target { get; init; }
    public IDuration Duration { get; init; }
    public ILogger Logger { get; init; }

    public Pie(int orderReference, ICook cook)
    {
        this.orderReference = orderReference;
        Target = this;
        Logger = cook;

        var Wrap = new PieOperation(
            this,
            cook.wrapCapacity,
            "............................WRAP📦",
            "..............................WRAP📦",
            1,
            cook,
            () =>
            {
                Logger.log($"{Target.Label}: .......................................DONE🚲");
                WhenDone?.Invoke(this, new DoneEventArgs(this));
            });

        var Bake = new PieOperation(
            this,
            cook.bakeCapacity,
            "...................BAKE♨️",
            DONE: ".....................BAKE♨️",
            3,
            cook,
            () => Wrap.Start(this)
        );

        var Prepare = new PieOperation(
            this,
            cook.prepareCapacity,
            "..........PREP🥣",
            DONE: "............PREP🥣",
            2,
            cook,
            () => Bake.Start(this)

        );

        Operations = new (new[] {
            Prepare, 
            Bake, 
            Wrap
        });

        Duration = new Duration(
            Operations.Sum(op => op.Duration.seconds));
    }

    public bool Start(object sender)
    {
        Logger.log($"{Target.Label}: START🎬");

        return Operations.FirstOrDefault()?.Start(sender)
               ?? false;
    }

    public bool IsStarted => 
        Operations.FirstOrDefault()?.IsStarted
        ?? false;

    public Action Done { get; init; }

    public event DoneEventHandler? WhenDone;

    public string Label => $"Pie#{orderReference:00}";
    public override string ToString() => Label;
}