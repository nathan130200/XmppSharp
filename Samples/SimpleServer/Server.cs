using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using XmppSharp;

namespace SimpleServer;

public static class Server
{
    static readonly Socket s_Socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    static readonly List<Connection> s_Connections = [];
    static byte[] s_CertData = default!;
    static readonly string s_CertPass = "localhost";
    public static readonly Jid Hostname = "localhost";

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

    public static X509Certificate2 GenerateCertificate()
        => X509CertificateLoader.LoadPkcs12(s_CertData, s_CertPass, loaderLimits: Pkcs12LoaderLimits.DangerousNoLimits);

    public static void StartListen()
    {
        var pfx = Path.Combine(Directory.GetCurrentDirectory(), "cert.pfx");

        if (File.Exists(pfx))
        {
            s_CertData = File.ReadAllBytes(pfx);

            using (var cert = GenerateCertificate()) // test if certificate is valid
                Debug.WriteLine(cert.ToString());
        }
        else
        {
            using var rsa = RSA.Create(1024);
            var csr = new CertificateRequest("CN=localhost", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            var start = DateTime.Now;
            var end = start.AddYears(50);

            var cert = csr.CreateSelfSigned(start, end);
            s_CertData = cert.Export(X509ContentType.Pfx, s_CertPass);
            File.WriteAllBytes(pfx, s_CertData);
        }

        s_Socket.Bind(new IPEndPoint(IPAddress.Any, 5222));
        s_Socket.Listen(10);

        _ = BeginAccept();
    }

    static async Task BeginAccept()
    {
        while (true)
        {
            try
            {
                var client = await s_Socket.AcceptAsync();
                _ = Task.Run(async () => await EndAccept(client));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Thread.Sleep(1);
        }
    }

    static async Task EndAccept(Socket s)
    {
        var connection = new Connection(s);

        lock (s_Connections)
            s_Connections.Add(connection);

        Console.WriteLine("client connected: " + s.RemoteEndPoint);

        try
        {
            await connection.StartAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        lock (s_Connections)
            s_Connections.Remove(connection);

        Console.WriteLine("client disconnected: " + s.RemoteEndPoint);
    }
}