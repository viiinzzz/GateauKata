using Gateau.config;

namespace GateauKata;

public class Pie : IPie
{
    public int Id { get; init; }

    private PieStatus _status;
    public PieStatus Status
    {
        get => _status;
        set => Transition(value);
    }

    private readonly IPieConfig _config;
    private readonly ILogger _logger;

    public Pie(int id, IPieConfig config, ILogger logger)
    {
        Id = id;
        _status = PieStatus.NotStarted;
        _config = config;
        _logger = logger;
    }

    private static (PieStatus from, PieStatus to)[] authorizedTransitions = {

        (PieStatus.NotStarted, PieStatus.Started),
        (PieStatus.Started, PieStatus.Preparing),
        (PieStatus.Preparing, PieStatus.Prepared),
        (PieStatus.Prepared, PieStatus.Baking),
        (PieStatus.Baking, PieStatus.Baked),
        (PieStatus.Baked, PieStatus.Wrapping),
        (PieStatus.Wrapping, PieStatus.Wrapped),
        (PieStatus.Wrapped, PieStatus.Ready),
    };

    public void Transition(PieStatus to)
    {
        var from = _status;

        foreach (var authorizedTransition in authorizedTransitions)
        {
            if ((from, to) == authorizedTransition)
            {
                _status = to;
                _logger.log($"{Label} {padStatus}");
                return;
            }
        }

        throw new Exception($"${Label} Invalid transition ${from}->${to} currently ${_status}");
    }

    public string Label => $"#{Id:00}";

    public override string ToString() => $"{Label} {padStatus}";

    public string padStatus => pad(Status);

    public static string pad(PieStatus status)
        => status switch
        {
            PieStatus.NotStarted => "..🔴",
            PieStatus.Started => "  🔴",
            PieStatus.Preparing => "  ..🥣",
            PieStatus.Prepared => "    🥣",
            PieStatus.Baking => "              ..♨️",
            PieStatus.Baked => "                ♨️",
            PieStatus.Wrapping => "                        ..📦",
            PieStatus.Wrapped => "                          📦",
            PieStatus.Ready => "                                      🏁",
            _ => "??",
        };
}