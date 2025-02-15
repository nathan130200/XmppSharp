using System.Net;
using System.Security.Cryptography.X509Certificates;
using XmppSharp.Net;
using XmppSharp.Protocol.Base;
using XmppSharp.Protocol.Core;
using XmppSharp.Protocol.Core.Tls;

var resourceNum = 1;

var clients = new List<XmppClientConnection>();

for (int i = 0; i < 64; i++)
{
    var options = new XmppClientConnectionOptions
    {
        EndPoint = new DnsEndPoint("localhost", 5222),
        Jid = $"dedicated@warface/GameServer-{resourceNum++}",
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


    var c = new XmppClientConnection(options);
    {

        c.OnError += (sender, ex) =>
        {
            Console.WriteLine(ex);
        }; /*
        
        c.OnReadXml += (sender, xml)
            => Console.WriteLine("recv <<\n{0}\n", xml);

        c.OnWriteXml += (sender, xml)
            => Console.WriteLine("send >>\n{0}\n", xml);
        */

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

        var wasPresenceSent = false;

        c.OnStateChanged += (sender, e) =>
        {
            if (e.After.HasFlag(XmppConnectionState.SessionStarted) && !wasPresenceSent)
            {
                wasPresenceSent = true;

                var stz = new Presence(PresenceType.Available)
                {
                    To = sender.Jid.Domain,
                };

                sender.Send(stz);

                Console.WriteLine("Client {0} online!", sender.Jid);
            }
        };
    }

    clients.Add(c);
}

_ = Task.Run(async () =>
{
    foreach (var c in clients)
        await c.ConnectAsync(false);
});

using var cts = new CancellationTokenSource();

Console.CancelKeyPress += (s, e) =>
{
    e.Cancel = true;
    cts?.Cancel();
};

while (!cts.IsCancellationRequested)
{
    var numClients = clients.Count(x => x.State.HasFlag(XmppConnectionState.SessionStarted));
    Console.Title = "Bot 1 - Online Clients: " + numClients + " / " + clients.Count + " (" + ((numClients / (float)clients.Count) * 100f) + ")";
    await Task.Delay(100);
}

Console.Write("Disconnecting... ");

foreach (var c in clients)
{
    var stz = new Presence(PresenceType.Available)
    {
        Show = PresenceShow.Away,
        To = c.Jid.Domain
    };

    c.Send(stz);

    await Task.Delay(160);

    c.Disconnect();
}

Console.WriteLine("done!");