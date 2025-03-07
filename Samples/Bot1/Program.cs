using System.Net;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using XmppSharp.Entities;
using XmppSharp.Entities.Options;
using XmppSharp.Net;
using XmppSharp.Protocol.Tls;

const string DefaultTemplate = "[{Timestamp:HH:mm:ss}|{SourceContext}|{Level:u3}] {Message:lj}{NewLine}{Exception}";

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console(theme: AnsiConsoleTheme.Code, outputTemplate: DefaultTemplate)
    .MinimumLevel.Verbose()
    .CreateLogger();

var logging = LoggerFactory.Create(builder =>
{
    builder.AddSerilog();
    builder.SetMinimumLevel(LogLevel.Debug);
});

var resource = "bot";

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
    Logger = logging.CreateLogger($"stresstest@localhost/{resource}"),
};

TaskCompletionSource? reconnecTcs = default;

var bot = new XmppClientConnection(options);

bot.OnStateChanged += (sender, e) =>
{
    if (e.After == XmppConnectionState.Connected)
        reconnecTcs?.TrySetResult();
};

bot.OnDisconnected += sender =>
{
    reconnecTcs?.TrySetResult();
    reconnecTcs = new();
    InitReconnect();
};

_ = bot.ConnectAsync();

await Task.Delay(-1);

void InitReconnect()
{
    _ = Task.Run(async () =>
    {
        try
        {
            await bot.ConnectAsync();
            await reconnecTcs.Task;
        }
        catch (Exception ex)
        {
            Log.Warning(ex.ToString());
        }

        await Task.Delay(5000);
    });
}