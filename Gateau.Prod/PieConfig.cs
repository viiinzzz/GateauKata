namespace GateauKata;

public record PieConfig(double PrepareSeconds, double BakeSeconds, double WrapSeconds) : IPieConfig
{

}