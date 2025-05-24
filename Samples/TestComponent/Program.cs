using System.Net;
using XmppSharp.Logging;
using XmppSharp;
using XmppSharp.Net;

using var client = new OutgoingXmppComponentConnection
{
    Server = "xmppsharp",
    EndPoint = new DnsEndPoint("localhost", 5275),
    Password = "youshallnotpass",
    VerbosityLevel = XmppLogScope.Full
};

client.OnLog += (sender, e) =>
{
    var self = (OutgoingXmppConnection)sender;
    Console.WriteLine($"[{e.Timestamp:HH:Mm:ss}] [{e.Scope}] <{self.Jid}> {e.Message}");

    if (e.Exception != null)
        Console.WriteLine(e.Exception);
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
                Console.WriteLine(ex);
            }
        }
    }
}