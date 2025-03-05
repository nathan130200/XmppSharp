using System.Net;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using XmppSharp.Net;
using XmppSharp.Protocol;
using XmppSharp.Protocol.Tls;

const string DefaultTemplate = "[{Timestamp:HH:mm:ss} <{SourceContext}> {Level:u3}] {Message:lj}{NewLine}{Exception}";

var bots = new List<XmppConnection>();

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console(theme: AnsiConsoleTheme.Code, outputTemplate: DefaultTemplate)
    .MinimumLevel.Verbose()
    .CreateLogger();

var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddSerilog();
    builder.SetMinimumLevel(LogLevel.Debug);
});

for (int i = 0; i < 1; i++)
{
    var resource = $"bot-{i + 1}";

    var options = new XmppClientConnectionOptions
    {
        Username = "stresstest",
        Domain = "localhost",
        Resource = resource,
        Password = "youshallnotpass",
        EndPoint = new IPEndPoint(IPAddress.Loopback, 5222),
        TlsPolicy = TlsPolicy.Required,
        DisconnectTimeout = TimeSpan.FromSeconds(5),
        InitialPresence = new(PresenceType.Available, priority: i),
        Logger = loggerFactory.CreateLogger(resource),
        TlsOptions =
        {
            TargetHost = "localhost"
        }
    };

    bots.Add(new XmppClientConnection(options));
}

bots.Add(new XmppComponentConnection(new()
{
    Domain = "stresstest",
    Password = "youshallnotpass",
    Logger = loggerFactory.CreateLogger("StressTest/Component"),
    EndPoint = new IPEndPoint(IPAddress.Loopback, 5275)
}));

foreach (var bot in bots)
{
    try
    {
        await bot.ConnectAsync();
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
    }
}

await Task.Delay(-1);