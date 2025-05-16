using System.Net;
using XmppSharp.Abstractions;
using XmppSharp.Net;
using XmppSharp.Net.Abstractions;
using XmppSharp.Protocol;
using XmppSharp.Protocol.Extensions.XEP0199;

using var client = new XmppOutboundClientConnection
{
    User = "xmppsharp",
    Server = "localhost",
    Resource = Environment.MachineName,
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

_ = new PingManager(client)
{
    Heartbeated = (success, time) =>
    {
        if (!success)
            Console.WriteLine("Connection is idle");
        else
            Console.WriteLine("Server ping: {0}ms", time);
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
            catch(Exception ex)
            {
                Console.WriteLine("Connection failed: " + ex.Message);
            }
        }
    }
}

// ----------------------------------------------------------------

class PingManager
{
    private XmppOutboundClientConnection _connection;
    private DateTimeOffset _lastPingTime = DateTimeOffset.Now;
    private Timer _timer;

    public Action<bool, long> Heartbeated { get; init; }

    public PingManager(XmppOutboundClientConnection c)
    {
        _connection = c;
        _connection.OnStateChanged += OnStateChanged;
    }

    void OnTick(object _)
    {
        Console.WriteLine("\tPing timer tick");
        Heartbeated(false, 0);
        _connection.Disconnect();
    }

    void OnStateChanged(ConnectionStateChangedEventArgs e)
    {
        if (e.NewState == XmppConnectionState.SessionStarted)
        {
            _timer = new(OnTick, null, -1, 60_000);
            e.Connection.OnElement += OnElement;
            Console.WriteLine("\tPing timer started");
        }
        else if (e.NewState == XmppConnectionState.Disconnecting)
        {
            _timer?.Change(-1, -1);
            _timer?.Dispose();
            e.Connection.OnElement -= OnElement;
            Console.WriteLine("\tPing timer stopped");
        }
    }

    void OnElement(ConnectionElementEventArgs e)
    {
        var con = e.Connection;

        if (e.Element is Iq iq && iq.Query is Ping)
        {
            iq.Type = IqType.Result;
            iq.SwitchDirection();
            var elapsed = _lastPingTime - DateTimeOffset.Now;
            _lastPingTime = DateTimeOffset.Now;
            Heartbeated?.Invoke(true, (long)elapsed.TotalMilliseconds);
            con.Send(iq);
        }
    }
}