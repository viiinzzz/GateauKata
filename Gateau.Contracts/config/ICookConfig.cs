namespace Gateau.config;

public interface ICookConfig
{
    int PrepareCapacity { get; }
    int BakeCapacity { get; }
    int WrapCapacity { get; }
    IPieConfig PieConfig { get; }
}