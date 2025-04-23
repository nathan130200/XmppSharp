using System.Net;
using XmppSharp.Net;
using XmppSharp.Net.Extensions;

using var client = new XmppClientConnection
{
    Server = "localhost",
    ConnectServer = new DnsEndPoint("localhost", 5222),
    User = "xmppsharp",
    Password = "youshallnotpass",
    AuthenticationMechanism = "PLAIN",
    SslOptions = new()
    {
        RemoteCertificateValidationCallback = delegate
        {
            return true;
        }
    }
};

client.OnError += ex =>
{
    Console.WriteLine(ex);
};

client.OnConnected += () =>
{
    Console.WriteLine("connected");
};

client.OnDisconnected += () =>
{
    Console.WriteLine("disconnected");
};

client.OnSessionStarted += () =>
{
    Console.WriteLine("session started");
};

client.OnReadXml += xml =>
{
    Console.WriteLine("recv <<\n{0}\n", xml);
};

client.OnWriteXml += xml =>
{
    Console.WriteLine("send >>\n{0}\n", xml);
};

var ext = new PingExtension(new()
{
    UseClientPingRequests = true,
    Interval = TimeSpan.FromSeconds(5),
    Timeout = TimeSpan.FromSeconds(10)
});

ext.OnElapsed += time =>
{
    Console.WriteLine("Ping between client and server: {0}ms", time);
};

client.RegisterExtension(ext);

try
{
    await client.ConnectAsync();
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}

await Task.Delay(-1);
