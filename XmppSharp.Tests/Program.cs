using System.Net;
using System.Net.Sockets;
using XmppSharp;
using XmppSharp.Protocol;
using XmppSharp.Protocol.Sasl;
using XmppSharp.Protocol.StreamFeatures;
using XmppSharp.Xmpp;
using XmppSharp.Xmpp.Dom;

using var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
socket.Bind(new IPEndPoint(IPAddress.Any, 5222));
socket.Listen(10);

while (true)
{
    using (var client = await socket.AcceptAsync())
    using (var stream = new NetworkStream(client, true))
    {
        using var semaphore = new SemaphoreSlim(1, 1);

        async Task SendXml(Element e)
        {
            try
            {
                await semaphore.WaitAsync();
                await stream.WriteAsync(e.ToString().GetBytes());
                Console.WriteLine("{0}|send >>\n{1}\n", e.GetType().FullName, e.ToString(true));
            }
            finally
            {
                semaphore.Release();
            }
        }

        async Task SendRawXml(string xml)
        {
            try
            {
                await semaphore.WaitAsync();
                await stream.WriteAsync(xml.GetBytes());
                Console.WriteLine("send >>\n{0}\n", xml);
            }
            finally
            {
                semaphore.Release();
            }
        }

        Jid jid = default;
        bool isAuthenticated = false;

        using (var parser = new Parser())
        {
            parser.Reset(stream);

            parser.OnStreamStart += async e =>
            {
                Console.WriteLine("recv <<\n{0}\n", e.StartTag());

                if (!isAuthenticated)
                {
                    jid = new Jid()
                    {
                        Domain = e.To.ToString()
                    };
                }

                e.SwitchDirection();

                await SendRawXml(e.StartTag());

                var features = new Features();

                if (!isAuthenticated)
                {
                    features.Mechanisms = new()
                    {
                        SupportedMechanisms =
                        [
                            new("PLAIN")
                        ]
                    };
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(jid.Resource))
                        features.Bind = new();
                }

                await SendXml(features);
            };

            parser.OnStreamElement += async e =>
            {
                Console.WriteLine("{0}|recv <<\n{1}\n", e.GetType().FullName, e.ToString(true));

                if (!isAuthenticated)
                {
                    if (e is Auth auth)
                    {
                        if (auth.MechanismName == "PLAIN")
                        {
                            var sasl = auth.GetContentFromBase64String().Split((char)0);
                            var username = sasl.Length == 2 ? sasl[0] : sasl[1];
                            var password = sasl.Length == 2 ? sasl[1] : sasl[2];

                            jid = jid with
                            {
                                Local = username
                            };

                            isAuthenticated = true;
                            await SendXml(new Success());
                            parser.Reset(stream);
                        }
                    }
                }
                else
                {
                    if (e is Iq iq)
                    {
                        if (iq.Bind != null)
                        {
                            jid = jid with { Resource = iq.Bind.Resource };
                            iq.Bind.Resource = null;
                            iq.Bind.Jid = jid;
                            iq.SwitchDirection();
                            iq.Type = IqType.Result;
                            await SendXml(iq);
                        }
                        else if (iq.Session != null)
                        {
                            iq.SwitchDirection();
                            iq.Type = IqType.Result;
                            await SendXml(iq);
                        }
                    }
                }
            };

            parser.OnStreamEnd += () =>
            {
                Console.WriteLine("recv <<\n{0}\n", Xml.JabberEndTag);
                parser.Dispose();
                return Task.CompletedTask;
            };

            while (await parser.ReadAsync())
                await Task.Delay(0);
        }
    }

    await Task.Delay(0);
}