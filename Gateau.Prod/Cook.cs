using System.Collections.Concurrent;

namespace GateauKata;

public class Cook : ICook
{
    public Cook(
        int prepareCapacityNominal, 
        int bakeCapacityNominal, 
        int wrapCapacityNominal,
        int pieTodoCount
        )
    {
        prepareCapacity = new Capacity(prepareCapacityNominal);
        bakeCapacity = new Capacity(bakeCapacityNominal);
        wrapCapacity = new Capacity(wrapCapacityNominal);
        
        PieTodoCount = pieTodoCount;
    }

    public string Status => 
  $"..........PREP:{prepareCapacity.Status}"
+ $".BAKE.{bakeCapacity.Status}"
+ $".WRAP.{wrapCapacity.Status}"
+ $"...DONE.{PieTodoCount - _doing.Count}/{PieTodoCount}";

    public ICapacity prepareCapacity { get; init; }
    public ICapacity bakeCapacity { get; init; }
    public ICapacity wrapCapacity { get; init; }
    public int PieTodoCount { get; init; }


    private readonly ConcurrentDictionary<int, IPie> _doing = new();
    public bool HasWork => _doing.Count > 0;

    public void Work()
    {
        for (int orderReference = 0; orderReference < PieTodoCount; orderReference++)
        {
            var pie = new Pie(orderReference, this);

            pie.WhenDone += (sender, args) =>
            {
                var key = ((Pie)args.DoneOperation)?.orderReference;
                if (!key.HasValue)
                {
                    return;
                }

                _doing.Remove(key.Value, out _);
                
            };

            _doing.AddOrUpdate(
                orderReference,
                _ => pie, 
                (_,_) => pie);

            pie.Start(this);
        }
    }
    

    public void log(string message)
    {
        Console.Out.WriteLine(message);
    }
}
