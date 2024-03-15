using System.Net;
using System.Net.Sockets;
using XmppSharp;
using XmppSharp.Protocol;
using XmppSharp.Protocol.Base;
using XmppSharp.Protocol.Sasl;
using XmppSharp.Protocol.StreamFeatures;
using XmppSharp.Xmpp;
using XmppSharp.Xmpp.Dom;

var syncLock = new object();

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
                OnXml(true, e);
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
                OnXml(true, xml);
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

            parser.OnStreamStart += async n =>
            {
                var e = n as XmppSharp.Protocol.Base.Stream;

                OnXml(false, e.StartTag());

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
                OnXml(false, e);

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
                        else
                        {
                            iq.SwitchDirection();
                            iq.Type = IqType.Error;
                            iq.Error = new StanzaError
                            {
                                Type = StanzaErrorType.Cancel,
                                Condition = StanzaErrorCondition.RecipientUnavailable
                            };

                            await SendXml(iq);
                        }
                    }
                }
            };

            parser.OnStreamEnd += () =>
            {
                OnXml(false, Xml.JabberEndTag);
                parser.Dispose();
                return Task.CompletedTask;
            };

            while (await parser.ReadAsync())
                await Task.Delay(0);
        }
    }

    await Task.Delay(0);
}

void OnXml<T>(bool isSend, T value)
{
    lock (syncLock)
    {
        string xml = value is Element e
            ? e.ToString(true)
            : value.ToString();

        var type = value.GetType();

        if (value is string)
            type = typeof(Element);
        {

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write('[');

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(type.FullName);

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("] ");
        }

        Console.ForegroundColor = isSend ? ConsoleColor.Red : ConsoleColor.Green;
        Console.Write(isSend ? "send" : "recv");

        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write((isSend ? " >>" : " <<") + ":\n");

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"{xml}\n");
    }
}