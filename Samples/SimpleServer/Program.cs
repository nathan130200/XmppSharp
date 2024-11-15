namespace SimpleServer;

static class Program
{
    static void Main(string[] args)
    {
        Server.StartListen();
        Thread.Sleep(-1);
    }
}
