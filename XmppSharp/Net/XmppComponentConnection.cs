using System.Globalization;
using Microsoft.Extensions.Logging;
using XmppSharp.Dom;
using XmppSharp.Entities;
using XmppSharp.Entities.Options;
using XmppSharp.Exceptions;
using XmppSharp.Protocol.Base;
using XmppSharp.Protocol.Component;

namespace XmppSharp.Net;

public class XmppComponentConnection : XmppConnection
{
    public new XmppComponentConnectionOptions Options
        => (base.Options as XmppComponentConnectionOptions)!;

    public XmppComponentConnection(XmppComponentConnectionOptions options) : base(options)
    {
    }

    protected override void SendStreamHeader()
    {
        var xml = new StreamStream
        {
            To = Options.Domain,
            DefaultNamespace = Namespaces.Accept,
            Language = CultureInfo.CurrentCulture.Name,
            Version = "1.0"
        };

        Logger.LogDebug("Sending stream start to {Hostname}", Options.Domain);

        Send(xml.StartTag());
    }

    protected override void HandleStreamStart(StreamStream e)
    {
        if (string.IsNullOrEmpty(e.Id))
            throw new JabberStreamException(StreamErrorCondition.InvalidXml, "Expected StreamID from remote server.");

        StreamId = e.Id;

        Logger.LogDebug("Stream start received from remote server. StreamID: {Id}", StreamId);

        var el = new Handshake(StreamId, Options.Password);
        Send(el);
        Logger.LogDebug("Sending component handshake {Token}", el.Value);
    }

    protected override void HandleStreamElement(XmppElement e)
    {
        if (!IsAuthenticated)
        {
            if (e is not Handshake)
                throw new JabberStreamException(StreamErrorCondition.InvalidXml, "Unexpected XML element");

            Logger.LogDebug("Component authenticated.");

            ChangeState(x => x | XmppConnectionState.Authenticated | XmppConnectionState.SessionStarted);

            //InitKeepAlive();

            FireOnOnline();
        }
        else
        {
            FireOnElement(e);
        }
    }
}