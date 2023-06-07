namespace Gateau.config;

public interface IPieConfig
{
    double PrepareSeconds { get; }
    double BakeSeconds { get; }
    double WrapSeconds { get; }
}