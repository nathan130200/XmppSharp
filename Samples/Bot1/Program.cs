using System.Net;
using XmppSharp.Net;
using XmppSharp.Protocol.Base;
using XmppSharp.Protocol.Core;

var options = new XmppConnectionOptions
{
    EndPoint = new DnsEndPoint("localhost", 5275),
    Jid = "cache.warface",
    Password = "youshallnotpass",
    OnlineVerbose = true
};

var component = new XmppComponentConnection(options);
{
    component.OnError += (sender, ex) =>
    {
        Console.WriteLine(ex);
    };

    component.OnReadXml += (sender, xml)
        => Console.WriteLine("recv <<\n{0}\n", xml);

    component.OnWriteXml += (sender, xml)
        => Console.WriteLine("send >>\n{0}\n", xml);

    component.OnIq += (sender, e) =>
    {
        e.SwitchDirection();
        e.Type = IqType.Error;

        e.Error = new()
        {
            Type = StanzaErrorType.Cancel,
            Condition = StanzaErrorCondition.FeatureNotImplemented
        };

        component.Send(e);
    };
}
await component.ConnectAsync();