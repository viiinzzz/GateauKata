namespace GateauKata;

public interface IPie : IPrintable, IOperation
{
    int orderReference { get; init; }

}