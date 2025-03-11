using System.Net;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using XmppSharp.Entities.Options;
using XmppSharp.Net;
using XmppSharp.Protocol.Tls;

const string DefaultTemplate = "[{Timestamp:HH:mm:ss}|{SourceContext}|{Level:u3}] {Message:lj}{NewLine}{Exception}";

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console(theme: AnsiConsoleTheme.Code, outputTemplate: DefaultTemplate)
    .MinimumLevel.Debug()
    .CreateLogger();

var logging = LoggerFactory.Create(builder =>
{
    builder.AddSerilog();
    builder.SetMinimumLevel(LogLevel.Critical);
});

var clients = new List<XmppClientConnection>();

for (int i = 0; i < 512; i++)
{
    var resource = "bot_" + i;

    var options = new XmppClientConnectionOptions
    {
        Username = "stresstest",
        Domain = "stresstest",
        Resource = resource,
        Password = "youshallnotpass",
        //EndPoint = new IPEndPoint(IPAddress.Loopback, 5275),
        EndPoint = new IPEndPoint(IPAddress.Loopback, 5222),
        TlsPolicy = TlsPolicy.Required,
        DisconnectTimeout = TimeSpan.FromSeconds(5),
        //Logger = logging.CreateLogger($"stresstest@localhost/{resource}"),
    };

    TaskCompletionSource? reconnecTcs = default;

    var bot = new XmppClientConnection(options);

    bot.OnConnected += e =>
    {
        reconnecTcs?.TrySetResult();
        Console.WriteLine($"client '{e.Jid}' online");
    };

    bot.OnDisconnected += e =>
    {
        Console.WriteLine($"client '{e.Jid}' offline");
        reconnecTcs?.TrySetResult();
        reconnecTcs = new();
        DoReconnect();
    };

    await bot.ConnectAsync();

    void DoReconnect()
    {
        _ = Task.Run(async () =>
        {
            await bot.ConnectAsync();
            await reconnecTcs.Task;
            await Task.Delay(5000);
        });
    }
}

await Task.Delay(-1);