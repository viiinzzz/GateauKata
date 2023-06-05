namespace GateauKata;

public delegate void DoneEventHandler(object sender, DoneEventArgs e);

public record DoneEventArgs(IOperation DoneOperation)
{

}
