namespace Gateau.config;

public record CookConfig(
    int PrepareCapacity,
    int BakeCapacity,
    int WrapCapacity,
    IPieConfig PieConfig
) : ICookConfig
{

}