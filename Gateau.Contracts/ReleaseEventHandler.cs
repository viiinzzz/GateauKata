namespace GateauKata;

public delegate void ConsumeEventHandler(object sender, ConsumeEventArgs e);

public record ConsumeEventArgs(IStatus consumer)
{

}

public delegate void ReleaseEventHandler(object sender, ReleaseEventArgs e);

public record ReleaseEventArgs(IStatus consumer)
{

}
