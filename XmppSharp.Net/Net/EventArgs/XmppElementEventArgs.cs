using XmppSharp.Dom;

namespace XmppSharp.Net.EventArgs;

public class XmppConnectionEventArgs : System.EventArgs
{
    public IXmppConnection Connection { get; init; }
}

public class XmppElementEventArgs : XmppConnectionEventArgs
{
    public XmppElement Element { get; init; }
}

public class StateChangedEventArgs : XmppConnectionEventArgs
{
    public XmppConnectionState OldState { get; init; }
    public XmppConnectionState NewState { get; init; }
}
