namespace SimpleServer;

static class Program
{
    static async Task Main(string[] args)
    {
        using var cts = new CancellationTokenSource();

        Server.StartListen(cts.Token);

        while (true)
            await Task.Delay(100);
    }
}
