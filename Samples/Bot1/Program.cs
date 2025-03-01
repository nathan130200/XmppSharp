using System.IO.Pipes;
using System.Net;
using XmppSharp.Net;
using XmppSharp.Protocol.Core.Tls;

var client = new XmppClientConnection
{
    Options =
    {
        Jid = "csharp@visualstudio/bot",
        EndPoint = new IPEndPoint(IPAddress.Loopback, 5222),
        TlsPolicy = StartTlsPolicy.Optional,
        Verbose = true,
        TlsOptions =
        {
            TargetHost = "warface"
        }
    }
};

client.OnDebugXml += (sender, e) =>
{
    var prefix = e.Direction == PipeDirection.In ? "recv <<" : "send >>";
    Console.WriteLine($"{prefix}:\n{e.Xml}\n");
};

client.OnError += (sender, ex) =>
{
    Console.WriteLine(ex);
};

client.OnStateChanged += (sender, e) =>
{
    Console.WriteLine("{0} -> {1}", e.Before, e.After);
};

await client.ConnectAsync();

await Task.Delay(-1);