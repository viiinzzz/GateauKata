namespace Gateau.config;

public record PieConfig(
    double PrepareSeconds,
    double BakeSeconds,
    double WrapSeconds
    ) : IPieConfig
{

}