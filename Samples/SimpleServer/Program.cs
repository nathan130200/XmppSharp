namespace SimpleServer;

static class Program
{
    static async Task Main(string[] args)
    {
        Server.StartListen();

        while (true)
            await Task.Delay(100);
    }
}
