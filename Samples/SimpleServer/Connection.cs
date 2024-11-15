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
using XmppSharp.Protocol;
using XmppSharp.Protocol.Base;
using XmppSharp.Protocol.Client;
using XmppSharp.Protocol.Extensions;
using XmppSharp.Protocol.Extensions.XEP0030;
using XmppSharp.Protocol.Sasl;
using XmppSharp.Protocol.Tls;

namespace SimpleServer;

public sealed class Connection : IDisposable
{
    private Socket? _socket;
    private Stream? _stream;
    private ExpatXmppParser? _parser;
    private Jid? _jabberId = null;
    private volatile bool _disposed;
    private volatile FileAccess _access = FileAccess.ReadWrite;
    private ConcurrentQueue<(string xml, byte[] buffer)> _writeQueue = [];
    private X509Certificate2? _cert;

    public Connection(Socket socket)
    {
        _stream = new NetworkStream(_socket = socket, false);
        _parser = new ExpatXmppParser(ExpatEncoding.UTF8);
        _parser.OnStreamStart += OnStreamStart;
        _parser.OnStreamEnd += OnStreamEnd;
        _parser.OnStreamElement += OnStreamElement;
    }

    internal async Task Initialize()
    {
        await Task.WhenAll(
            BeginSend(),
            BeginReceive()
        );
    }

    async Task BeginSend()
    {
        try
        {
            while (!_disposed)
            {
                await Task.Delay(1);

                while (!_access.HasFlag(FileAccess.Write))
                {
                    Console.WriteLine("write paused");

                    if (_disposed)
                    {
                        Console.WriteLine("cancel write: disposed");
                        return;
                    }

                    await Task.Delay(160);
                }

                if (_writeQueue.TryDequeue(out var entry))
                {
                    if (_stream == null)
                        return;

                    await _stream.WriteAsync(entry.buffer);

                    Console.WriteLine("send >>\n{0}\n", entry.xml);
                }
            }
        }
        catch (Exception ex)
        {
            _access &= ~FileAccess.Write;
            Console.WriteLine(ex);
        }
        finally
        {
            Dispose();
        }
    }

    async Task BeginReceive()
    {
        var buf = new byte[256];

        try
        {
            while (!_disposed)
            {
                await Task.Delay(1);

                while (!_access.HasFlag(FileAccess.Read))
                {
                    Console.WriteLine("read paused");

                    if (_disposed)
                    {
                        Console.WriteLine("cancel read: disposed");
                        return;
                    }

                    await Task.Delay(160);
                }

                if (_stream == null || _parser == null)
                    break;

                var len = await _stream.ReadAsync(buf);

                if (len <= 0)
                    break;

                _parser?.Write(buf, len);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        finally
        {
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

        SpinWait sw = default;

        while (!_writeQueue.IsEmpty)
            sw.SpinOnce();

        _access &= ~FileAccess.Write;
        _stream?.Dispose();
        _stream = null;

        _cert?.Dispose();
        _cert = null;

        _socket?.Dispose();
        _socket = null;

        _parser?.Dispose();
        _parser = null;
    }

    public void Send(object data)
    {
        if (data is not string s)
            s = data.ToString()!;

        _writeQueue.Enqueue((s, s.GetBytes()));
    }

    void OnStreamStart(StreamStream e)
    {
        Console.WriteLine("recv <<\n{0}\n", e.StartTag());

        // in real scenario, its better to save Stream ID as unique identifier in connections.
        // also XMPP components require stream ID to create handshake hash.

        e.From = Server.Hostname;
        e.GenerateId(IdGenerator.Timestamp);
        e.SwitchDirection();

        Send(e.StartTag());

        var features = new StreamFeatures();

        if (_jabberId == null)
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

    async void OnStreamElement(Element e)
    {
        Console.WriteLine("recv <<\n{0}\n", e.ToString(true));

        if (e is StartTls)
        {
            _access &= ~FileAccess.Read;
            Send(new Proceed());

            while (!_writeQueue.IsEmpty)
                await Task.Delay(160);

            _access &= ~FileAccess.Write;

            try
            {
                var handshakeTask = Task.Run(async () =>
                {
                    _cert = Server.GenerateCertificate();
                    _stream = new SslStream(_stream!, false);
                    _parser!.Reset();

                    await ((SslStream)_stream).AuthenticateAsServerAsync(_cert);

                    _access = FileAccess.ReadWrite;
                });

                var result = await Task.WhenAny(handshakeTask, Task.Delay(2500));

                if (result != handshakeTask)
                    throw new TimeoutException("TLS handshake timeout!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("TLS handshake failed: " + ex);
                Dispose();
            }

            return;
        }

        if (e is Auth auth)
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

            _jabberId = new($"{user}@{Server.Hostname}");
            Send(new Success());

            _access &= ~FileAccess.Read;
            _parser!.Reset();
            _access |= FileAccess.Read;
        }
        else if (e is Iq iq)
        {
            if (iq.FirstChild is Bind bind)
            {
                iq.SwitchDirection();

                string resource = bind.Resource ?? IdGenerator.Guid.Generate();

                var search = _jabberId! with { Resource = resource };

                if (Server.Connections.Any(x => FullJidComparer.Shared.Compare(x._jabberId, search) == 0))
                {
                    iq.Type = IqType.Error;

                    iq.Error = new StanzaError
                    {
                        Type = StanzaErrorType.Cancel,
                        Condition = StanzaErrorCondition.Conflict
                    };

                    Send(iq);

                    return;
                }
                else
                {
                    _jabberId = _jabberId with { Resource = resource };
                    iq.Type = IqType.Result;
                    bind.Resource = null;
                    bind.Jid = _jabberId;
                    Send(iq);
                }
            }
            else if (iq.FirstChild is Session)
            {
                iq.SwitchDirection();
                iq.Type = IqType.Result;
                Send(iq);
                return;
            }

            if (iq.To == null || iq.To == Server.Hostname)
            {
                if (iq.FirstChild is DiscoInfo discoInfo)
                {
                    discoInfo.AddIdentity(new Identity
                    {
                        Category = "component",
                        Type = "c2s"
                    });

                    discoInfo.AddFeature(new Feature(Namespaces.DiscoInfo));
                    discoInfo.AddFeature(new Feature(Namespaces.DiscoItems));
                    discoInfo.AddFeature(new Feature(Namespaces.Ibb));
                    discoInfo.AddFeature(new Feature(Namespaces.Ping));
                    discoInfo.AddFeature(new Feature(Namespaces.IqVersion));

                    iq.SwitchDirection();
                    iq.Type = IqType.Result;

                    Send(iq);
                }
                else if (iq.FirstChild is DiscoItems discoItems)
                {
                    foreach (var connection in Server.Connections.Where(x => x._jabberId != null))
                    {
                        discoItems.AddItem(new Item { Jid = connection._jabberId });
                    }

                    iq.SwitchDirection();
                    iq.Type = IqType.Result;
                    Send(iq);
                }
                else if (iq.FirstChild is Ping)
                {
                    iq.SwitchDirection();
                    iq.Type = IqType.Result;
                    Send(iq);
                }
                else
                {
                    iq.SwitchDirection();
                    iq.Type = IqType.Error;

                    iq.Error = new StanzaError()
                    {
                        Type = StanzaErrorType.Cancel,
                        Condition = StanzaErrorCondition.FeatureNotImplemented
                    };

                    Send(iq);

                    return;
                }
            }
            else
            {
                var targetConnection = Server.Connections.FirstOrDefault(x => x._jabberId == iq.To);

                if (targetConnection == null)
                {
                    iq.Type = IqType.Error;
                    iq.SwitchDirection();
                    iq.Error = new StanzaError
                    {
                        Type = StanzaErrorType.Wait,
                        Condition = StanzaErrorCondition.RecipientUnavailable

                    };

                    Send(iq);
                }
                else
                {
                    targetConnection.Send(new Iq(iq)
                    {
                        From = _jabberId
                    });
                }
            }
        }
        // Handle other XMPP elements.
    }
}
