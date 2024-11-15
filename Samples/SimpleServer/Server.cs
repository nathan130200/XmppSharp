using System.Net;
using System.Net.Sockets;
using XmppSharp;

namespace SimpleServer;

public static class Server
{
    static readonly Socket s_Socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    static readonly List<Connection> s_Connections = [];
    public static readonly Jid Hostname = "xmppsharp";

    public static void RegisterConnection(Connection c)
    {
        lock (s_Connections)
            s_Connections.Add(c);
    }

    public static void UnregisterConnection(Connection c)
    {
        lock (s_Connections)
            s_Connections.Remove(c);
    }

    public static IEnumerable<Connection> Connections
    {
        get
        {
            Connection[] result;

            lock (s_Connections)
                result = s_Connections.ToArray();

            return result;
        }
    }

    public static void StartListen()
    {
        s_Socket.Bind(new IPEndPoint(IPAddress.Any, 5222));
        s_Socket.Listen(10);

        new Thread(AcceptThread)
        {
            Name = "Xmpp Server/Accept Thread",
            IsBackground = true
        }.Start();
    }

    static void AcceptThread()
    {
        while (true)
        {
            try
            {
                new Connection(s_Socket.Accept()).Setup();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Thread.Sleep(1);
        }
    }
}