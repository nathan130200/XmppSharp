using System.Net;
using XmppSharp.Net;

using var connection = new XmppOutboundComponentConnection
{
    Server = "xmppsharp",
    ConnectServer = new DnsEndPoint("localhost", 5275),
    Password = "youshallnotpass",
};

connection.OnError += ex =>
{
    Console.WriteLine(ex);
};

connection.OnConnected += () =>
{
    Console.WriteLine("connected");
};

connection.OnDisconnected += () =>
{
    Console.WriteLine("disconnected");
};

connection.OnSessionStarted += () =>
{
    Console.WriteLine("session started");
};

connection.OnReadXml += xml =>
{
    Console.WriteLine("recv <<\n{0}\n", xml);
};

connection.OnWriteXml += xml =>
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
    Console.WriteLine("Ping between component and server: {0}ms", time);
};

connection.RegisterExtension(ext);

try
{
    await connection.ConnectAsync();
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}

await Task.Delay(-1);
