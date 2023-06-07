using System.Collections.Concurrent;
using Gateau.config;

namespace GateauKata;

public class Cook
{
    private readonly ICookConfig _config;
    private readonly ILogger _logger;

    private int _prepareRetryDelay;
    private int _bakeRetryDelay;
    private int _wrapRetryDelay;

    private int _prepareStartDelay;
    private int _bakeStartDelay;
    private int _wrapStartDelay;

    private int _makeBestDelay;


    public Cook(ICookConfig config, ILogger logger)
    {
        _config = config;
        _logger = logger;

        _prepareRetryDelay = (int)_config.PieConfig.PrepareSeconds * 1000 / _config.PrepareCapacity;
        _bakeRetryDelay = (int)_config.PieConfig.BakeSeconds * 1000 / _config.BakeCapacity;
        _wrapRetryDelay = (int)_config.PieConfig.WrapSeconds * 1000 / _config.WrapCapacity;

        _prepareStartDelay = 100;
        _bakeStartDelay = _prepareStartDelay + (int)_config.PieConfig.PrepareSeconds * 1000;
        _wrapStartDelay = _prepareStartDelay + (int)_config.PieConfig.PrepareSeconds * 1000;

        _makeBestDelay = (int)((_config.PieConfig.PrepareSeconds
                         + _config.PieConfig.BakeSeconds
                         + _config.PieConfig.WrapSeconds) * 1000);

        _logger.log(@$"
Je suis le maître patissier 👨‍🍳

🥣   Je peux préparer {_config.PrepareCapacity} gâteaux en même temps
♨️    Je peux cuire {_config.BakeCapacity} gâteaux en même temps
📦   Je peux emballer {_config.WrapCapacity} gâteaux en même temps

Un gâteau est prêt lorsqu'il a terminé ces 3 étapes

🥣   préparation : durée {Math.Round(_config.PieConfig.PrepareSeconds, 1)} secondes
♨️    cuisson : durée {Math.Round(_config.PieConfig.BakeSeconds, 1)} secondes
📦   emballage : durée {Math.Round(_config.PieConfig.WrapSeconds, 1)} secondes
");
    }

readonly ConcurrentDictionary<int, IPie> _doingPies = new();
    public List<IPie> DoingPies
        => _doingPies
            .Select(kv=> kv.Value)
            .ToList();

    public async Task MakePies(int pieTodoCount)
    {
        _logger.log(@$"
J'ai reçu une commande pour {pieTodoCount} gâteaux 🥧
");

        BlockingCollection<IPie> prepare = new(_config.PrepareCapacity);
        BlockingCollection<IPie> bake = new(_config.BakeCapacity);
        BlockingCollection<IPie> wrap = new(_config.WrapCapacity);

        Task feedPrepare = Task.Run(async () =>
        {
            for (int id = 0; id < pieTodoCount; id++)
            {
                var pie = new Pie(id, _config.PieConfig, _logger);
                _doingPies.AddOrUpdate(id, pie, (_, _) => pie);

                do
                {
                    try
                    {
                        prepare.Add(pie);
                        pie.Status = PieStatus.Started;

                        break;
                    }
                    catch (InvalidOperationException)
                    {
                        _logger.log("prepare max capacity reached.");
                        await Task.Delay(_prepareRetryDelay);
                    }
                } while (true);
            }
            
            prepare.CompleteAdding();
        });

        int preparedPieCount = 0;
        Task consumePrepare = Task.Run(async () =>
        {
            await Task.Delay(_prepareStartDelay);

            while (true)
            {
                bool nomore = false;

                List<IPie> piesToPrepare = new();
                for (int k = 0; k < _config.PrepareCapacity; k++)
                {
                    try
                    {
                        var pieToPrepare = prepare.Take();
                        piesToPrepare.Add(pieToPrepare);
                    }
                    catch (InvalidOperationException)
                    {
                        nomore = true;
                        break;
                    }
                }

                var preparedPies = await Prepare(piesToPrepare);
                preparedPieCount += preparedPies.Length;

                do
                {
                    try
                    {
                        foreach (var preparedPie in preparedPies)
                        {
                            bake.Add(preparedPie);
                        }

                        if (preparedPieCount >= pieTodoCount)
                        {
                            bake.CompleteAdding();
                        }

                        break;
                    }
                    catch (InvalidOperationException)
                    {
                        _logger.log("bake max capacity reached.");
                        await Task.Delay(_bakeRetryDelay);
                    }
                } while (true);

                if (nomore)
                {
                    break;
                }
            }
        });

        int bakedPieCount = 0;
        Task consumeBake = Task.Run(async () =>
        {
            await Task.Delay(_bakeStartDelay);
            
            while (true)
            {
                bool nomore = false;
                
                List<IPie> piesToBake = new();
                for (int k = 0; k < _config.BakeCapacity; k++)
                {
                    try {
                        var pieToBake = bake.Take();
                        piesToBake.Add(pieToBake);
                    }
                    catch (InvalidOperationException)
                    {
                        nomore = true;
                        break;
                    }
                }

                var bakedPies = await Bake(piesToBake);
                bakedPieCount += bakedPies.Length;
                do
                {
                    try
                    {
                        foreach (var bakedPie in bakedPies)
                        {
                            wrap.Add(bakedPie);
                        }

                        if (bakedPieCount >= pieTodoCount)
                        {
                            wrap.CompleteAdding();
                        }

                        break;
                    }
                    catch (InvalidOperationException)
                    {
                        _logger.log("wrap max capacity reached.");
                        await Task.Delay(_wrapRetryDelay);
                    }
                } while (true);


                if (nomore)
                {
                    break;
                }
            }
        });

        int wrappedPieCount = 0;
        int readyPieCount = 0;
        Task consumeWrap = Task.Run(async () =>
        {
            await Task.Delay(_wrapStartDelay);
            
            while (true)
            {
                bool nomore = false;

                List<IPie> piesToWrap = new();
                for (int k = 0; k < _config.WrapCapacity; k++)
                {
                    try
                    {
                        var pieToWrap = wrap.Take();
                        piesToWrap.Add(pieToWrap);
                    }
                    catch (InvalidOperationException)
                    {
                        nomore = true;
                        break;
                    }
                }

                var wrappedPies = await Wrap(piesToWrap);
                wrappedPieCount += wrappedPies.Length;

                foreach (var wrappedPie in wrappedPies)
                {
                    bool ready = _doingPies.TryRemove(wrappedPie.Id, out _);
                    if (ready)
                    {
                        wrappedPie.Status = PieStatus.Ready;
                        readyPieCount++;
                    }
                }

                if (nomore)
                {
                    break;
                }
            }
        });

        Action logStatus = () => _logger.log(""
 + $"        🥣{preparedPieCount} {prepare.Count}/{_config.PrepareCapacity}"
 + $"  ♨️{bakedPieCount:###} {bake.Count}/{_config.BakeCapacity}"
 + $"  📦{wrappedPieCount:###} {wrap.Count}/{_config.WrapCapacity}"
 + $"  🏁{readyPieCount:###}/{pieTodoCount:###}"
        );

        Task monitor = Task.Run(async () =>
        {
            while (true)
            {
                logStatus();
                var dt = _makeBestDelay * pieTodoCount * 5 / 100;
                await Task.Delay(dt);
            }
        });

        var work = Task.WhenAll(
            feedPrepare, 
            consumePrepare,
            consumeBake, 
            consumeWrap);

        await Task.WhenAny(work, monitor);
        logStatus();
    }




    public async Task<IPie[]> Prepare(IEnumerable<IPie> pies)
    {
        var payload = pies as IPie[] ?? pies.ToArray();
        if (!payload.Any()) return Array.Empty<IPie>();

        foreach (var pie in payload) pie.Status = PieStatus.Preparing;

        _logger.log($"🥣 Je prépare les gâteaux {payload.Select(pie => pie.Label).Aggregate((x, y) => x + " " + y)}");
        await Task.Delay((int)_config.PieConfig.PrepareSeconds * 1000);

        foreach (var pie in payload) pie.Status = PieStatus.Prepared;
        return payload;
    }

    public async Task<IPie[]> Bake(IEnumerable<IPie> pies)
    {
        var payload = pies as IPie[] ?? pies.ToArray();
        if (!payload.Any()) return Array.Empty<IPie>();

        foreach (var pie in payload) pie.Status = PieStatus.Baking;

        _logger.log($"♨️ Je cuis les gâteaux {payload.Select(pie => pie.Label).Aggregate((x, y) => x + " " + y)}");
        await Task.Delay((int)_config.PieConfig.PrepareSeconds * 1000);

        foreach (var pie in payload) pie.Status = PieStatus.Baked;
        return payload;
    }

    public async Task<IPie[]> Wrap(IEnumerable<IPie> pies)
    {
        var payload = pies as IPie[] ?? pies.ToArray();
        if (!payload.Any()) return Array.Empty<IPie>();

        foreach (var pie in payload) pie.Status = PieStatus.Wrapping;

        _logger.log($"📦 J'emballe les gâteaux {payload.Select(pie => pie.Label).Aggregate((x, y) => x + " " + y)}");
        await Task.Delay((int)_config.PieConfig.PrepareSeconds * 1000);

        foreach (var pie in payload) pie.Status = PieStatus.Wrapped;
        return payload;
    }

}