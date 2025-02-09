using System.Net;
using System.Security.Cryptography.X509Certificates;
using XmppSharp.Net;
using XmppSharp.Protocol.Base;
using XmppSharp.Protocol.Core;
using XmppSharp.Protocol.Core.Tls;

var options = new XmppClientConnectionOptions
{
    EndPoint = new DnsEndPoint("localhost", 5222),
    Jid = "dedicated@warface/",
    Password = "gamek01:dedicated",
    AuthenticationMechanism = "PLAIN",
    SslPolicy = StartTlsPolicy.Required,
    Ssl = new()
    {
        TargetHost = "warface",
        CertificateRevocationCheckMode = X509RevocationMode.NoCheck,
        RemoteCertificateValidationCallback = (a, b, c, d) => true
    },
    Verbose = true,
};

var clients = new List<XmppClientConnection>();

for (int i = 0; i < 10; i++)
{

    var c = new XmppClientConnection(options);
    {
        c.OnError += (sender, ex) =>
        {
            Console.WriteLine(ex);
        };

        c.OnReadXml += (sender, xml)
            => Console.WriteLine("recv <<\n{0}\n", xml);

        c.OnWriteXml += (sender, xml)
            => Console.WriteLine("send >>\n{0}\n", xml);

        c.OnIq += (sender, e) =>
        {
            e.SwitchDirection();
            e.Type = IqType.Error;

            e.Error = new()
            {
                Type = StanzaErrorType.Cancel,
                Condition = StanzaErrorCondition.FeatureNotImplemented
            };

            c.Send(e);
        };
    }

    clients.Add(c);
}

var tasks = new List<Task>();

foreach (var c in clients)
    tasks.Add(c.ConnectAsync());

await Task.WhenAll(tasks);