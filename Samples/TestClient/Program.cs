using XmppSharp;
using XmppSharp.Expat;
using XmppSharp.Parser;

using var client = new XmppClientConnection
{
    Jid = new("testclient@localhost/dotnet"),
    AuthenticationMechanism = "PLAIN",
    Password = "!12345@xmpp:",
};

client.OnReadXml += xml =>
{
    lock (client)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("recv <<:\n{0}\n", xml);
        Console.ForegroundColor = ConsoleColor.Gray;
    }
};

client.OnWriteXml += xml =>
{
    lock (client)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("send >>:\n{0}\n", xml);
        Console.ForegroundColor = ConsoleColor.Gray;
    }
};

client.OnStateChanged += e =>
{
    if (e.After.HasFlag(XmppConnectionState.Authenticated))
    {
        // do something
    }
};

client.SetParser(new ExpatXmppParser(ExpatEncoding.UTF8));
client.Connect("127.0.0.1", 5222);

await Task.Delay(-1);