namespace XmppSharp.Abstractions;

// XMPP prefix to distinct from other types.

public delegate void XmppLoggingDelegate(XmppLoggingEventArgs e);

public enum XmppLogLevel
{
    Error,
    Warning,
    Information,
    Debug,
    Verbose,
}