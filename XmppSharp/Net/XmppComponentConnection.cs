using XmppSharp.Dom;
using XmppSharp.Exceptions;
using XmppSharp.Protocol.Base;
using XmppSharp.Protocol.Component;

namespace XmppSharp.Net;

public class XmppComponentConnection : XmppConnection
{
    public string Server { get; set; }
    public string Password { get; set; }

    protected override void InitConnection()
    {
        Send(new StreamStream
        {
            DefaultNamespace = Namespaces.Accept,
            To = Server,
            Version = "1.0",
            Language = "en"
        });
    }

    protected override void HandleStreamStart(StreamStream e)
    {
        if (string.IsNullOrWhiteSpace(e.Id))
            throw new JabberException("Server did not sent Stream ID.");

        StreamId = e.Id;

        Send(new Handshake(e.Id, Password));
    }

    protected override void HandleStreamElement(XmppElement e)
    {
        if (!IsAuthenticated)
        {
            if (e is Handshake && !IsAuthenticated)
            {
                IsAuthenticated = true;
                Jid = new(Server);
                FireOnSessionStarted();
            }
        }
        else
        {
            if (e is Stanza stz)
                FireOnStanza(stz);
        }
    }
}
