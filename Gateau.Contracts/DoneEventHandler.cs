namespace GateauKata;

public delegate void DoneEventHandler(object sender, DoneEventArgs e);

public record DoneEventArgs(IOperation DoneOperation)
{

}

public delegate void StartedEventHandler(object sender, StartedEventArgs e);

public record StartedEventArgs(IOperation StartedOperation)
{

}
