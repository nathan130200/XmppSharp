namespace SimpleServer;

static class Program
{
    static async Task Main(string[] args)
    {
        Server.StartListen();
        await Task.Delay(-1);
    }
}
