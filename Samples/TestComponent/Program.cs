using System.Net;
using XmppSharp.Abstractions;
using XmppSharp.Net;

using var client = new XmppOutboundComponentConnection
{
    Server = "xmppsharp.localhost",
    EndPoint = new DnsEndPoint("localhost", 5222),
    Password = "youshallnotpass",
    LogLevel = XmppLogLevel.Verbose
};

client.OnLog += (e) =>
{
    lock (client)
    {
        var self = (e.Sender as XmppOutboundClientConnection)!;
        Console.WriteLine($"[{e.Timestamp:HH:Mm:ss}] [{e.Level}] <{self.Jid}> {e.Message}");

        if (e.Exception != null)
            Console.WriteLine(e.Exception);
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
                Console.WriteLine("Connection failed: " + ex.Message);
            }
        }
    }
}