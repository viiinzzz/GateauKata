using System.Reflection.Emit;

namespace GateauKata;

public class PieOperation : IOperation {
    public PieOperation(
        IPie pie,

        ICapacity capacity,
        double durationSeconds
        )
    {
        this.pie = pie;

        Capacity = capacity;

        void WhenReleased(object sender, ReleaseEventArgs args)
        {
            var started = Start(sender);
            if (started)
            {
                capacity.WhenReleased -= WhenReleased;
            }
            
        }

        capacity.WhenReleased += WhenReleased;

        Duration = new Duration(durationSeconds);
        IsStarted = false;
    }
    
    private IPie pie { get; init; }
    private ICapacity Capacity { get; init; }
    public IDuration Duration { get; init; }
    public bool IsStarted { get; private set; }

    public event StartedEventHandler WhenStarted;
    public event DoneEventHandler WhenDone;

    public bool Start(object sender)
    {
        // lock (this)
        // {
            if (IsStarted)
            {
                // throw new OperationException(
                //     "already started");

                return true;
            }

            if (!Capacity.Consume(this, pie))
            {
                return false;
            }

            IsStarted = true;
        // }

        DoIt(sender);
        return true;
    }

    public async Task DoIt(object sender)
    {
        WhenStarted?.Invoke(sender, new StartedEventArgs(this));

        //
        //
        await Task.Delay(Duration.milliSeconds);
        //
        //

        Capacity.Release(this, pie);
        WhenDone?.Invoke(sender, new DoneEventArgs(this));
    }

    public override string ToString()
        => $"{pie.Label} {pie.padStatus}";
}