namespace GateauKata;

public interface IPie : IPrintable, IOperation, IStatus
{
    int orderReference { get; init; }
    string padStatus { get; }

}