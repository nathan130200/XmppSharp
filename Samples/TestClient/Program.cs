using System.Net;
using XmppSharp.Net;

using var client = new XmppOutboundClientConnection
{
    User = "xmppsharp",
    Server = "localhost",
    Resource = Environment.MachineName,
    EndPoint = new DnsEndPoint("localhost", 5222),
    Password = "youshallnotpass",
    SaslMechanismSelector = m
        => m.FirstOrDefault(x => x.MechanismName == "PLAIN")
};

client.OnError += ex =>
{
    Console.WriteLine(ex);
};

client.OnStateChanged += (e) =>
{
    Console.WriteLine("state changed: {0} -> {1}", e.OldState, e.NewState);
};

client.OnReadXml += xml =>
{
    Console.WriteLine("recv <<\n{0}\n", xml);
};

client.OnWriteXml += xml =>
{
    Console.WriteLine("send >>\n{0}\n", xml);
};

try
{
    await client.ConnectAsync();
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}

await Task.Delay(-1);
