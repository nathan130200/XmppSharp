using System.Net;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using XmppSharp.Net;

const string DefaultTemplate = "[{Timestamp:HH:mm:ss}|{SourceContext}|{Level:u3}] {Message:lj}{NewLine}{Exception}";

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console(theme: AnsiConsoleTheme.Code, outputTemplate: DefaultTemplate)
    .MinimumLevel.Debug()
    .CreateLogger();

Console.Title = "STRESS TEST BOT";

#pragma warning disable
Console.SetBufferSize(Console.BufferWidth, Console.BufferHeight);
#pragma warning restore

var logging = LoggerFactory.Create(builder =>
{
    builder.AddSerilog();
    builder.SetMinimumLevel(LogLevel.Critical);
});

var bots = new List<StressTestBot>();

var comp = new XmppComponentConnection
{
    Server = "stresstest",
    Password = "youshallnotpass",
    ConnectServer = new IPEndPoint(IPAddress.Loopback, 5275)
};

/*
comp.OnReadXml += x
    => Console.WriteLine("recv <<\n{0}\n", x);

comp.OnWriteXml += x
    => Console.WriteLine("send >>\n{0}\n", x);*/

comp.OnError += ex =>
{
    Console.WriteLine("ERROR: " + ex);
};

comp.OnSessionStarted += () =>
{
    Console.WriteLine("Component stresstest online");
};

comp.OnDisconnected += () =>
{
    Console.WriteLine("Component stresstest offline");
};

await comp.ConnectAsync();

for (int i = 0; i < 512; i++)
    bots.Add(new StressTestBot(i));

_ = Task.Run(async () =>
{
    while (true)
    {
        var numBots = bots.Count;

        var numConnected = bots.Count(x => x.IsConnected);
        var ratioConnected = (numConnected / (float)numBots) * 100f;

        var numOnline = bots.Count(x => x.IsOnline);
        var ratioOnline = (numOnline / (float)numBots) * 100f;


        var buf = string.Format("\rStress Test - connected: {0:F1}% - online: {1:F1}%", ratioConnected, ratioOnline).PadRight(Console.BufferWidth);
        Console.WriteLine(buf);
        await Task.Delay(500);
    }
});

foreach (var bot in bots)
    bot.Start();

await Task.Delay(-1);

class StressTestBot : IDisposable
{
    int _botId;
    XmppClientConnection? _connection;
    Timer _timer;

    public StressTestBot(int botId)
    {
        _botId = botId;
        _timer = new(Timer_OnTick, null, -1, -1);
    }

    public void Dispose()
    {
        _connection?.Dispose();
        _connection = null;
    }

    const int DefaultReconnectTime = 3000;

    volatile bool _isOnline;

    public bool IsConnected => _connection?.IsConnected == true;

    public bool IsOnline => IsConnected && _isOnline;

    void Init()
    {
        _connection = new XmppClientConnection
        {
            User = "stresstest",
            Password = "youshallnotpass",
            Resource = $"bot_{_botId}",
            ConnectServer = new IPEndPoint(IPAddress.Loopback, 5222),
            Server = "localhost",
            AuthenticationMechanism = "PLAIN",
            SslOptions =
            {
                RemoteCertificateValidationCallback = delegate { return true; }
            }
        };

        _connection.OnConnected += () =>
        {
            _timer.Change(-1, -1);
        };

        _connection.OnSessionStarted += () =>
        {
            //Console.WriteLine("[bot {0}] online", _botId);
            _isOnline = true;
        };

        _connection.OnDisconnected += () =>
        {
            //Console.WriteLine("[bot {0}] offline", _botId);
            Dispose();
            _timer.Change(0, DefaultReconnectTime);
            _isOnline = false;
        };

        if (_botId == -1)
        {
            _connection.OnError += ex =>
            {
                Console.WriteLine("[bot {0}] {1}", _botId, ex);
            };

            _connection.OnReadXml += e =>
            {
                Console.WriteLine("[bot {0}] recv <<\n{1}\n", _botId, e);
            };

            _connection.OnWriteXml += e =>
            {
                Console.WriteLine("[bot {0}] send >>\n{1}\n", _botId, e);
            };
        }
    }

    public void Start()
        => _timer.Change(0, DefaultReconnectTime);

    async void Timer_OnTick(object? state)
    {
        try
        {
            if (_connection == null)
            {
                Init();
                await _connection!.ConnectAsync();
            }
        }
        catch (Exception ex)
        {
            if (_botId == -1)
                Console.WriteLine(ex);

            Dispose();
        }
    }
}