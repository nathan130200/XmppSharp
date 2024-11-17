using System.Collections.Concurrent;
using System.Data;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using XmppSharp;
using XmppSharp.Collections;
using XmppSharp.Dom;
using XmppSharp.Expat;
using XmppSharp.Parser;
using XmppSharp.Protocol.Base;
using XmppSharp.Protocol.Core;
using XmppSharp.Protocol.Core.Client;
using XmppSharp.Protocol.Core.Sasl;
using XmppSharp.Protocol.Core.Tls;
using XmppSharp.Protocol.Extensions.XEP0030;
using XmppSharp.Protocol.Extensions.XEP0199;

namespace SimpleServer;

public sealed class Connection : IDisposable
{
    private Socket? _socket;
    private Stream? _stream;
    private ExpatXmppParser? _parser;
    public Jid Jid { get; private set; }
    private volatile bool _disposed;
    private volatile FileAccess _access;
    private readonly ConcurrentQueue<(string? xml, byte[] buffer)> _writeQueue = [];
    private X509Certificate2? _cert;
    private CancellationTokenSource? _cts = new();
    public bool IsAuthenticated { get; private set; }
    private Task? _receiveTask;
    private Task? _sendTask;

    public Connection(Socket socket)
    {
        Jid = new($"unknown@{Server.Hostname}");

        _stream = new NetworkStream(_socket = socket, false);
        _parser = new ExpatXmppParser(ExpatEncoding.UTF8);
        _parser.OnStreamStart += OnStreamStart;
        _parser.OnStreamEnd += OnStreamEnd;
        _parser.OnStreamElement += OnStreamElement;
    }

    internal Task Initialize()
    {
        var tcs = new TaskCompletionSource();
        _cts!.Token.Register(() => tcs.TrySetResult());
        Reset();
        return tcs.Task;
    }

    void Reset()
    {
        if (_cts == null || _cts.IsCancellationRequested)
            return;

        _parser?.Reset();
        _access = FileAccess.ReadWrite;

        if (_receiveTask == null || _receiveTask.IsCompleted)
            _receiveTask = Task.Run(BeginReceive, _cts.Token);

        if (_sendTask == null || _sendTask.IsCompleted)
            _sendTask = Task.Run(BeginSend, _cts.Token);
    }

    async Task BeginSend()
    {
        try
        {
            while (_access.HasFlag(FileAccess.Write))
            {
                if (_cts == null || _cts.IsCancellationRequested)
                    break;

                await Task.Delay(1);

                while (_writeQueue.TryDequeue(out var entry))
                {
                    if (_stream == null)
                        return;

                    await _stream.WriteAsync(entry.buffer);

                    if (entry.xml != null)
                        Console.WriteLine("<{0}> send >>\n{1}\n", Jid, entry.xml);
                }
            }
        }
        catch (Exception ex)
        {
            if (ex is not IOException)
                Console.WriteLine(ex);

            _access &= ~FileAccess.Write;
            _writeQueue.Clear();

            Dispose();
        }
    }

    async Task BeginReceive()
    {
        var buf = new byte[1024];

        try
        {
            while (_access.HasFlag(FileAccess.Read))
            {
                await Task.Delay(1);

                if (_cts == null || _cts.IsCancellationRequested)
                    break;

                var len = await _stream!.ReadAsync(buf);

                if (len <= 0)
                    break;

                _parser!.Write(buf, len);
            }
        }
        catch (Exception ex)
        {
            if (ex is not IOException)
                Console.WriteLine(ex);

            Dispose();
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        _access &= ~FileAccess.Read;
        _socket?.Shutdown(SocketShutdown.Receive);

        Send(Xml.XMPP_STREAM_END);

        _parser?.Dispose();
        _parser = null;

        _ = Task.Delay(_writeQueue.IsEmpty ? 160 : 500)
            .ContinueWith(_ =>
            {
                _access &= ~FileAccess.Write;

                _stream?.Dispose();
                _stream = null;

                _cert?.Dispose();
                _cert = null;

                _cts?.Cancel();
                _cts?.Dispose();
                _cts = null;

                _socket?.Dispose();
                _socket = null;
            });
    }

    public void Send(string xml)
        => _writeQueue.Enqueue((xml, xml.GetBytes()));

    public void Send(Element e)
        => _writeQueue.Enqueue((e.ToString(true), e.GetBytes()));

    public void Route(Element e)
        => _writeQueue.Enqueue((null, e.GetBytes()));

    void OnStreamStart(StreamStream e)
    {
        Console.WriteLine("<{0}> recv <<\n{1}\n", Jid, e.StartTag());

        e.From = Server.Hostname;
        e.GenerateId(IdGenerator.Timestamp);
        e.SwitchDirection();

        Send(e.StartTag());

        var features = new StreamFeatures();

        if (!IsAuthenticated)
        {
            if (_stream is not SslStream)
                features.StartTls = new(StartTlsPolicy.Optional);

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
            features.SupportBind = true;
            features.SupportSession = true;
        }

        Send(features);
    }

    private void OnStreamEnd()
    {
        Console.WriteLine("<{0}> recv <<\n{1}\n", Jid, Xml.XMPP_STREAM_END);
        Send(Xml.XMPP_STREAM_END);
        Dispose();
    }

    public void Disconnect(Element e)
    {
        if (!_disposed)
        {
            Send(e);
            Dispose();
        }
    }

    void OnStreamElement(Element e)
        => AsyncHelper.RunSync(() => HandleStreamElement(e));

    async Task HandleStreamElement(Element e)
    {
        Console.WriteLine("<{0}> recv <<\n{1}\n", Jid, e.ToString(true));

        if (e is StartTls)
        {
            _access &= ~FileAccess.Read;

            Send(new Proceed());

            while (!_writeQueue.IsEmpty)
                await Task.Delay(100);

            _access &= ~FileAccess.Write;

            _ = Task.Run(async () =>
            {
                try
                {
                    Console.WriteLine("<{0}> Starting TLS handshake...", _socket!.RemoteEndPoint);
                    _cert = Server.GenerateCertificate();

                    _stream = new SslStream(_stream!, false);
                    await ((SslStream)_stream).AuthenticateAsServerAsync(_cert);
                    Console.WriteLine("<{0}> TLS handshake completed!", _socket!.RemoteEndPoint);
                    Reset();
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

            Jid = new($"{user}@{Server.Hostname}");
            IsAuthenticated = true;
            Send(new Success());
            Reset();
        }
        else if (e is Stanza stz)
        {
            bool handled = false;

            if (stz is Iq iq)
            {
                if (iq.FirstChild is Bind bind)
                {
                    iq.SwitchDirection();

                    string resource = bind.Resource ?? IdGenerator.Guid.Generate();

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

                        roster.Ver = IdGenerator.Timestamp.Generate();

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

                        client.Send(new Presence(p)
                        {
                            From = Jid,
                        });
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
