namespace GateauKata;

public interface IPie
{
    int Id { get; init; }
    PieStatus Status { get; set; }
    string Label { get; }
    string padStatus { get; }
    void Transition(PieStatus to);
}