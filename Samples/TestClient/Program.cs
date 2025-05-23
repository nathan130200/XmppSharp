using System.Net;
using XmppSharp.Logging;
using XmppSharp.Net;
using XmppSharp.Net.EventArgs;
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
            catch (Exception ex)
            {
                Console.WriteLine("Connection failed: " + ex.Message);
            }
        }
    }
}

// ----------------------------------------------------------------

class PingManager
{
    private readonly OutgoingXmppClientConnection _connection;
    private DateTimeOffset _lastPingTime = DateTimeOffset.Now;
    private Timer _timer;

    public Action<bool, long> Heartbeated { get; init; }

    public PingManager(OutgoingXmppClientConnection c)
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

    void OnStateChanged(StateChangedEventArgs e)
    {
        if (e.NewState == XmppConnectionState.SessionStarted)
        {
            _timer = new(OnTick, null, -1, 5000);
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

    void OnElement(XmppElementEventArgs e)
    {
        var con = e.Connection;

        if (e.Element is Iq iq && iq.Query is Ping)
        {
            iq.Type = IqType.Result;
            iq.SwitchDirection();
            con.SendAsync(iq).ContinueWith(_ =>
            {
                var elapsed = DateTimeOffset.Now - _lastPingTime;
                Heartbeated?.Invoke(true, (long)elapsed.TotalMilliseconds);
                _lastPingTime = DateTimeOffset.Now;
            });
        }
    }
}