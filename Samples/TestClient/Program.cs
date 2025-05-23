using System.Net;
using XmppSharp.Logging;
using XmppSharp.Net;
using XmppSharp.Protocol;
using XmppSharp.Protocol.Extensions.XEP0199;

using var client = new OutgoingXmppClientConnection
{
    User = "xmppsharp",
    Server = "localhost",
    Resource = Environment.MachineName,
    EndPoint = new DnsEndPoint("localhost", 5222),
    Password = "youshallnotpass",
    VerbosityLevel = XmppLogScope.Full
};

client.OnLog += static (sender, e) =>
{
    var self = (OutgoingXmppClientConnection)sender;
    Console.WriteLine($"[{e.Timestamp:HH:Mm:ss}] <{self.Jid}> {e.Message}");

    if (e.Exception != null)
        Console.WriteLine(e.Exception);
};

client.OnElement += e =>
{
    var con = e.Connection;

    if (e.Element is Iq iq && iq.Query is Ping)
    {
        iq.SwitchDirection();
        iq.Type = IqType.Result;
        iq.Query = null;
        con.Send(iq);
    }
};

while (true)
{
    while (!client.IsConnected)
    {
        await Task.Delay(3000);

        if (client.State == XmppConnectionState.Connecting)
            continue;

        if (client.State == XmppConnectionState.Disconnected)
        {
            try
            {
                await client.ConnectAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex + "\n\n");
            }
        }
    }
}