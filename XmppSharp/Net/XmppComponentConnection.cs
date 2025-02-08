using XmppSharp.Dom;
using XmppSharp.Protocol.Base;
using XmppSharp.Protocol.Core;
using XmppSharp.Protocol.Core.Component;

namespace XmppSharp.Net;

public class XmppComponentConnection : XmppConnection
{
    public XmppComponentConnection(XmppConnectionOptions options) : base(options)
    {

    }

    protected override void SendStreamHeader()
    {
        var s = new StreamStream
        {
            To = Options.Jid,
            Version = "1.0",
            DefaultNamespace = Namespaces.Accept
        }.StartTag();

        Send(s);
    }

    protected override void HandleStreamStart(StreamStream e)
    {
        if (string.IsNullOrWhiteSpace(e.Id))
            throw new JabberStreamException(StreamErrorCondition.InvalidXml);

        Send(new Handshake(e.Id!, Options.Password));
    }

    protected override void HandleStreamElement(XmppElement e)
    {
        if (!State.HasFlag(XmppConnectionState.Authenticated))
        {
            if (e is StreamError se)
                throw new JabberStreamException(se.Condition ?? StreamErrorCondition.UndefinedCondition);

            if (e is Handshake)
                State |= XmppConnectionState.Authenticated;
        }
        else
        {
            if (e is Stanza stz)
            {
                if (!string.IsNullOrWhiteSpace(stz.Id) && Callbacks.TryRemove(stz.Id!, out var tcs))
                {
                    _ = Task.Run(() => tcs.TrySetResult(stz));
                    return;
                }

                if (e is Message message) FireOnMessage(message);
                if (e is Presence presence) FireOnPresence(presence);
                if (e is Iq iq) FireOnIq(iq);
            }
        }
    }
}
