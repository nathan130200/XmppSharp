using XmppSharp.Dom;

namespace XmppSharp.Net.Abstractions;

public sealed class ConnectionElementEventArgs
{
    public XmppConnection Connection { get; init; }
    public XmppElement Element { get; init; }
}
