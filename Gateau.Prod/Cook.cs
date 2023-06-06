using System.Collections.Concurrent;
using System.Diagnostics;

namespace GateauKata;

public class Cook : ICook, ILogger
{
    public Cook(
        int prepareCapacityNominal, double preparationSeconds, 
        int bakeCapacityNominal, double bakeSeconds,
        int wrapCapacityNominal, double wrapSeconds,
        int pieTodoCount
        )
    {
        PieConfig = new PieConfig(preparationSeconds, bakeSeconds, wrapSeconds);
        PieTodoCount = pieTodoCount;

        PrepareCapacity = new Capacity(prepareCapacityNominal);
        BakeCapacity = new Capacity(bakeCapacityNominal);
        WrapCapacity = new Capacity(wrapCapacityNominal);

        // PrepareCapacity.WhenConsumed += (sender, args) => log($"       {Pie.pad(PieStatus.Prepared)}{PrepareCapacity.Status} -1  {args.consumer}");
        PrepareCapacity.WhenReleased += (sender, args) => log($"       {Pie.pad(PieStatus.Prepared)}{PrepareCapacity.Status} +1  {args.consumer}");
        // BakeCapacity.WhenConsumed += (sender, args) => log($"       {Pie.pad(PieStatus.Baked)}{BakeCapacity.Status} -1  {args.consumer}");
        BakeCapacity.WhenReleased += (sender, args) => log($"       {Pie.pad(PieStatus.Baked)}{BakeCapacity.Status} +1  {args.consumer}");
        // WrapCapacity.WhenConsumed += (sender, args) => log($"       {Pie.pad(PieStatus.Wrapped)}{WrapCapacity.Status} -1  {args.consumer}");
        WrapCapacity.WhenReleased += (sender, args) => log($"       {Pie.pad(PieStatus.Wrapped)}{WrapCapacity.Status} +1  {args.consumer}");
    }

    public string Status => 
  $"   🥣{PrepareCapacity.Status}"
+ $"  ♨️{BakeCapacity.Status}"
+ $" 📦{WrapCapacity.Status}"
+ $" 🏁{_doneCount}/{PieTodoCount}";

    public ICapacity PrepareCapacity { get; init; }
    public ICapacity BakeCapacity { get; init; }
    public ICapacity WrapCapacity { get; init; }

    public IPieConfig PieConfig { get; init; }
    public int PieTodoCount { get; init; }


    private readonly ConcurrentDictionary<int, IPie> _doing = new();
    public List<IPie> DoingPies => 
        _doing
            .Select(kv => kv.Value)
            .ToList();


    private bool _hasWork = false;
    public bool HasWork => _hasWork;
    private int _enqueueCount = 0;
    private int _doneCount = 0;

    public async Task Work()
    {
        _hasWork = true;
        _doneCount = 0;

        Func<int> enqueue1 = () =>
        {
            var orderReference = _enqueueCount++;
            var pie = new Pie(orderReference, this);

            void WhenPieDone(object sender, DoneEventArgs args)
            {
                var donePie = args.DoneOperation as Pie;
                if (donePie == null)
                {
                    return;
                }

                bool disposed = donePie.Disposing();
                if (!disposed)
                {
                    return;
                }

                var key = donePie.orderReference;
                _doing.Remove(key, out _);
                donePie.WhenDone -= WhenPieDone;
                _doneCount++;
            }
            pie.WhenDone += WhenPieDone;

            _doing.AddOrUpdate(orderReference, _ => pie, (_, _) => pie);
            pie.Start(this);

            return orderReference;
        };


        while (_enqueueCount < PieTodoCount)
        {
            while (_doing.Count > 8)
            {
                Debug.WriteLine($"### doing {_doing.Count} full");
                await Task.Delay(600);
            }

            var orderReference = enqueue1();
            Debug.WriteLine($"### queue #{orderReference:00}");
        }


        while (_doing.Count > 0)
        {
            Debug.WriteLine($"### doing " + _doing.Count);
            await Task.Delay(1000);
        }

        _hasWork = false;
    }


    public void log(string message)
    {
        // if (!message.Contains("Pie#01"))
        // {
        //     return;
        // }

        Debug.WriteLine(message);
    }
    
}

