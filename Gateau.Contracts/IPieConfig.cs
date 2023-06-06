namespace GateauKata;

public interface IPieConfig
{
    double PrepareSeconds { get; init; }
    double BakeSeconds { get; init; }
    double WrapSeconds { get; init; }
}