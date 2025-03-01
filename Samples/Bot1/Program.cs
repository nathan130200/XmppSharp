using System.Collections.Concurrent;
using System.IO.Pipes;
using System.Net;
using XmppSharp;
using XmppSharp.Net;
using XmppSharp.Protocol.Core.Tls;

ConcurrentStack<ConsoleColor> _colorStack = [];

using var cts = new CancellationTokenSource();
var clients = new List<XmppClientConnection>();

for (int i = 0; i < 16; i++)
{
    var jid = new Jid($"stresstest@localhost/bot-{i}");

    var client = new XmppClientConnection
    {
        Options =
        {
            Jid = jid,
            EndPoint = new IPEndPoint(IPAddress.Loopback, 5222),
            TlsPolicy = StartTlsPolicy.Optional,
            Verbose = true,
            DisconnectTimeout = TimeSpan.FromSeconds(5),
            TlsOptions =
            {
                TargetHost = "localhost"
            }
        }
    };


    client.OnDebugXml += (sender, e) =>
    {
        PushColor(e.Direction == PipeDirection.In ? ConsoleColor.Green : ConsoleColor.Red);
        var prefix = e.Direction == PipeDirection.In ? "recv <<" : "send >>";
        Console.WriteLine($"({jid}) {prefix}:\n{e.Xml}\n");
        PopColor();
    };


    client.OnError += (sender, ex) =>
    {
        PushColor(ConsoleColor.Yellow);
        Console.WriteLine(ex);
        PopColor();
    };

    PushColor(ConsoleColor.Cyan);
    Console.WriteLine("New client added: " + jid);
    clients.Add(client);
    PopColor();
}

Console.CancelKeyPress += (s, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

_ = Task.Run(async () =>
{
    foreach (var client in clients)
    {
        await client.ConnectAsync();
        await Task.Delay(160);
    }
});

var once = false;

while (!cts.IsCancellationRequested)
{
    if (clients.All(x => x.IsConnected) && !once)
    {
        PushColor(ConsoleColor.Magenta);
        Console.WriteLine("All clients connected");
        PopColor();
        once = true;
    }

    await Task.Delay(1000);
}

foreach (var client in clients)
    client.Disconnect();

while (clients.Any(x => x.IsConnected))
    await Task.Delay(160);

Console.ReadKey(true);

void PushColor(ConsoleColor newColor)
{
    _colorStack.Push(Console.ForegroundColor);
    Console.ForegroundColor = newColor;
}

void PopColor()
{
    if (_colorStack.TryPop(out var old))
        Console.ForegroundColor = old;
    else
        Console.ForegroundColor = ConsoleColor.White;
}