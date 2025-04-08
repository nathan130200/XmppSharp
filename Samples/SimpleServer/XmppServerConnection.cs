using System.Collections.Concurrent;
using System.Data;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Expat;
using XmppSharp;
using XmppSharp.Collections;
using XmppSharp.Dom;
using XmppSharp.Protocol;
using XmppSharp.Protocol.Base;
using XmppSharp.Protocol.Client;
using XmppSharp.Protocol.Extensions.XEP0030;
using XmppSharp.Protocol.Extensions.XEP0199;
using XmppSharp.Protocol.Sasl;
using XmppSharp.Protocol.Tls;

namespace SimpleServer;

public sealed class XmppServerConnection : IDisposable
{
    private Socket? _socket;
    private Stream? _stream;
    private XmppParser? _parser;
    public Jid Jid { get; private set; }
    private volatile byte _disposed;
    private volatile FileAccess _access;
    readonly ConcurrentQueue<Func<CancellationToken, Task>> _taskQueue = [];
    private readonly ConcurrentQueue<(string? xml, byte[] buffer)> _writeQueue = [];
    private X509Certificate2? _cert;
    public bool IsAuthenticated { get; private set; }

    public XmppServerConnection(Socket socket)
    {
        Jid = new($"unknown@{Server.Hostname}");

        _stream = new NetworkStream(_socket = socket, false);
        _parser = new XmppParser(ExpatEncoding.UTF8);
        _parser.OnStreamStart += OnStreamStart;
        _parser.OnStreamEnd += OnStreamEnd;
        _parser.OnStreamElement += OnStreamElement;
    }

    internal async Task StartAsync(CancellationToken token)
    {
        _access = FileAccess.ReadWrite;
        await Task.WhenAll(BeginReceive(token), BeginSend(token));
    }

    async Task BeginSend(CancellationToken token)
    {
        try
        {
            while (_disposed < 2)
            {
                await Task.Delay(16, token);

                if (!_access.HasFlag(FileAccess.Write))
                    continue;

                if (_writeQueue.TryDequeue(out var entry))
                {
                    await _stream!.WriteAsync(entry.buffer, token);

                    if (!string.IsNullOrEmpty(entry.xml))
                        Console.WriteLine("<{0}> send >>\n{1}\n", Jid, entry.xml);

                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            _access &= ~FileAccess.Write;
            _writeQueue.Clear();

            Dispose();
        }
    }

    async Task BeginReceive(CancellationToken token)
    {
        var buf = new byte[1024];

        try
        {
            while (_disposed < 1)
            {
                await Task.Delay(16, token);

                if (_taskQueue.TryDequeue(out var action))
                {
                    await action(token);
                    continue;
                }

                if (!_access.HasFlag(FileAccess.Read))
                    continue;

                var len = await _stream!.ReadAsync(buf, token);

                if (len <= 0)
                    break;

                _parser!.Parse(buf, len);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            Dispose();
        }
    }

    public void Dispose()
    {
        if (_disposed > 0)
            return;

        _disposed++;
        _access &= ~FileAccess.Read;
        _socket?.Shutdown(SocketShutdown.Receive);

        Send(Xml.XmppStreamEnd);

        _parser?.Dispose();
        _parser = null;

        _ = Task.Delay(_writeQueue.IsEmpty ? 160 : 500)
            .ContinueWith(_ =>
            {
                _disposed++;
                _access &= ~FileAccess.Write;

                _stream?.Dispose();
                _stream = null;

                _cert?.Dispose();
                _cert = null;

                _socket?.Dispose();
                _socket = null;

                Console.WriteLine("{0} offline!", Jid);
            });
    }

    public void Send(string xml)
        => _writeQueue.Enqueue((xml, xml.GetBytes()));

    public void Send(XmppElement e) => _writeQueue.Enqueue((e.ToString(true), e.GetBytes()));

    public void Route(XmppElement e) => _writeQueue.Enqueue((null, e.GetBytes()));

    void OnStreamStart(StreamStream e)
    {
        Console.WriteLine("<{0}> recv <<\n{1}\n", Jid, e);

        e.From = Server.Hostname;
        e.GenerateId();
        Send(e.StartTag());

        var features = new StreamFeatures();

        if (!IsAuthenticated)
        {
            if (_stream is not SslStream)
                features.StartTls = new(TlsPolicy.Optional);

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
            features.SupportsBind = true;
            features.SupportsSession = true;
        }

        Send(features);
    }

    private void OnStreamEnd()
    {
        Console.WriteLine("<{0}> recv <<\n{1}\n", Jid, Xml.XmppStreamEnd);
        Send(Xml.XmppStreamEnd);
        Dispose();
    }

    public void Disconnect(XmppElement e)
    {
        if (!(_disposed < 2))
            return;

        Send(e);
        Dispose();
    }

    void OnStreamElement(XmppElement e)
        => HandleStreamElement(e).GetAwaiter().GetResult();

    async Task HandleStreamElement(XmppElement e)
    {
        Console.WriteLine("<{0}> recv <<\n{1}\n", Jid, e.ToString(true));

        if (e is StartTls)
        {
            _access &= ~FileAccess.Read;

            Send(new Proceed());

            while (!_writeQueue.IsEmpty)
                await Task.Delay(16);

            _taskQueue.Enqueue(async (token) =>
            {
                await Task.Yield();

                try
                {
                    Console.WriteLine("<{0}> Starting TLS handshake...", _socket!.RemoteEndPoint);
                    _cert = Server.GenerateCertificate();

                    _stream = new SslStream(_stream!, false);

                    await ((SslStream)_stream).AuthenticateAsServerAsync(new SslServerAuthenticationOptions
                    {
                        ServerCertificate = _cert,
                        CertificateRevocationCheckMode = X509RevocationMode.NoCheck,
                        ClientCertificateRequired = false
                    }, token);

                    Console.WriteLine("<{0}> TLS handshake completed!", _socket!.RemoteEndPoint);

                    _parser?.Reset();
                    _access = FileAccess.ReadWrite;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("TLS handshake failed: " + ex);
                    Dispose();
                }
            });

            return;
        }
        else if (e is Auth auth)
        {
            if (auth.Mechanism != "PLAIN")
            {
                Disconnect(new Failure(FailureCondition.InvalidMechanism));
                return;
            }

            var sasl = Convert.FromBase64String(auth.Value!)
                    .GetString()
                    .Split('\0');

            string? user, pass;

            if (sasl.Length == 3)
            {
                user = sasl[1];
                pass = sasl[2];
            }
            else
            {
                user = sasl[0];
                pass = sasl[1];
            }

            if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass))
            {
                Disconnect(new Failure(FailureCondition.IncorrectEncoding));
                return;
            }

            _access &= ~FileAccess.Read;
            Jid = new($"{user}@{Server.Hostname}");
            IsAuthenticated = true;

            _taskQueue.Enqueue(async (token) =>
            {
                await Task.Yield();
                Send(new Success());
                _parser!.Reset();
                _access |= FileAccess.Read;
            });
        }
        else if (e is Stanza stz)
        {
            bool handled = false;

            if (stz is Iq iq)
            {
                if (iq.FirstChild is Bind bind)
                {
                    iq.SwitchDirection();

                    string resource = bind.Resource ?? Guid.NewGuid().ToString("d");

                    var search = Jid! with { Resource = resource };

                    if (Server.Connections.Any(x => FullJidComparer.Shared.Compare(x.Jid, search) == 0))
                    {
                        iq.Type = IqType.Error;

                        iq.Error = new StanzaError
                        {
                            Type = StanzaErrorType.Cancel,
                            Condition = StanzaErrorCondition.Conflict
                        };

                        Send(iq);
                    }
                    else
                    {
                        Jid = Jid with { Resource = resource };
                        iq.Type = IqType.Result;
                        bind.Resource = null;
                        bind.Jid = Jid;
                        Send(iq);
                    }

                    handled = true;
                }
                else if (iq.FirstChild is Session)
                {
                    Console.WriteLine("{0} online!", Jid);

                    iq.SwitchDirection();
                    iq.Type = IqType.Result;
                    Send(iq);

                    handled = true;
                }

                if (iq.To == null || iq.To == Server.Hostname)
                {
                    if (iq.FirstChild is DiscoInfo discoInfo)
                    {
                        discoInfo.AddIdentity(Identities.Component.C2S);
                        discoInfo.AddIdentity(Identities.Component.Router);
                        discoInfo.AddIdentity(Identities.Component.Presence);
                        discoInfo.AddIdentity(Identities.Server.IM);

                        discoInfo.AddFeature(new Feature(Namespaces.DiscoInfo));
                        discoInfo.AddFeature(new Feature(Namespaces.DiscoItems));
                        discoInfo.AddFeature(new Feature(Namespaces.Ping));

                        iq.SwitchDirection();
                        iq.Type = IqType.Result;

                        Send(iq);

                        handled = true;
                    }
                    else if (iq.FirstChild is DiscoItems discoItems)
                    {
                        iq.SwitchDirection();
                        iq.Type = IqType.Result;
                        Send(iq);

                        handled = true;
                    }
                    else if (iq.FirstChild is Roster roster)
                    {
                        iq.SwitchDirection();
                        iq.Type = IqType.Result;

                        roster.Ver = "1";

                        var contacts = Server.Connections.Where(x => x.IsAuthenticated)
                            .Where(x => x != this)
                            .Select(x => x.Jid.Bare)
                            .Distinct();

                        foreach (var j in contacts)
                            roster.AddRosterItem(j, subscription: RosterSubscriptionType.Both);

                        Send(iq);

                        handled = true;
                    }
                    else if (iq.FirstChild is Ping)
                    {
                        iq.SwitchDirection();
                        iq.Type = IqType.Result;
                        Send(iq);

                        handled = true;
                    }
                }

                if (!handled)
                {
                    var client = Server.Connections.FirstOrDefault(x => x.Jid.IsFullEquals(stz.To));

                    if (client != null)
                        stz.From = Jid;
                    else
                    {
                        client = Server.Connections.FirstOrDefault(x => x.IsAuthenticated && x.Jid.IsBareEquals(stz.To));

                        if (client != null)
                            stz.From = Jid.Bare;
                        else
                            goto _next;
                    }

                    client.Route(stz);
                    handled = true;
                }
            }

            if (stz is Message m)
            {
                if (m.Type != MessageType.Chat || m.To?.ToString()?.Contains("conference") == true)
                    goto _next;

                handled = true;

                m.From = Jid;

                var client = Server.Connections.FirstOrDefault(x => x.IsAuthenticated && x.Jid.IsBareEquals(m.To));

                if (client != null)
                    client.Route(m);
                else
                {
                    m.SwitchDirection();
                    m.To = Jid;
                    m.Type = MessageType.Error;
                    m.Error = new StanzaError()
                    {
                        Type = StanzaErrorType.Wait,
                        Condition = StanzaErrorCondition.RecipientUnavailable
                    };

                    Route(m);
                }
            }

            if (stz is Presence p)
            {
                handled = true;

                if (p.To?.Domain?.Contains("conference") == true)
                    goto _next;

                if (p.To == null || p.To == Server.Hostname)
                {
                    foreach (var client in Server.Connections.Where(c => c.IsAuthenticated))
                    {
                        if (client == this) continue;

                        var newPresencec = (Presence)p.Clone();
                        newPresencec.From = Jid;
                        client.Send(newPresencec);
                    }
                }
                else
                {
                    var client = Server.Connections.FirstOrDefault(x => x.IsAuthenticated && x.Jid.IsBareEquals(p.To));

                    if (client == null)
                    {
                        p.Type = PresenceType.Error;
                        p.Error = new StanzaError()
                        {
                            Type = StanzaErrorType.Wait,
                            Condition = StanzaErrorCondition.RecipientUnavailable
                        };
                    }
                    else
                    {
                        p.Type = PresenceType.Available;
                        client.Route(p);
                    }

                    Send(p);
                }
            }

        _next:

            if (!handled)
            {
                stz.SwitchDirection();
                stz.Type = "error";

                stz.Error = new StanzaError()
                {
                    Type = StanzaErrorType.Cancel,
                    Condition = StanzaErrorCondition.FeatureNotImplemented
                };

                Send(stz);

                return;
            }

            // Handle other XMPP stanzas.
        }

        // Handle other XMPP elements.
    }
}
