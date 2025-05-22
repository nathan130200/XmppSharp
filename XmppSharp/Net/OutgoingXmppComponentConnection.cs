using XmppSharp.Dom;
using XmppSharp.Protocol.Component;

namespace XmppSharp.Net;

//component protocol xep-0114

/// <summary>
/// Represents an outbound XMPP component connection that implements the XEP-0114 protocol.
/// </summary>
/// <remarks>
/// This class extends <see cref="OutgoingXmppConnection"/> to provide functionality specific to XMPP component connections.
/// </remarks>
public sealed class OutgoingXmppComponentConnection : OutgoingXmppConnection
{
    protected override void InitConnection()
    {
        Send(new Protocol.Base.Stream
        {
            To = Server,
            DefaultNamespace = Namespaces.Accept,
            Version = "1.0",
            Language = "en"
        });
    }

    protected override void OnStreamStart(Protocol.Base.Stream e)
    {
        base.OnStreamStart(e);
        Send(new Handshake(e.Id!, Password));
    }

    protected override void OnStreamElement(XmppElement e)
    {
        ProcessCallbacks(e);

        if (State < XmppConnectionState.Authenticated)
        {
            if (e is Handshake)
            {
                GotoState(XmppConnectionState.Authenticated);
                return;
            }

            Disconnect();
        }
        else
        {
            FireOnElement(e);
        }
    }
}
