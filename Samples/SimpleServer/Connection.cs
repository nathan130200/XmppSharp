using System.Collections.Concurrent;
using System.Data;
using System.Net.Sockets;
using System.Text;
using XmppSharp;
using XmppSharp.Collections;
using XmppSharp.Dom;
using XmppSharp.Expat;
using XmppSharp.Parser;
using XmppSharp.Protocol;
using XmppSharp.Protocol.Base;
using XmppSharp.Protocol.Client;
using XmppSharp.Protocol.Sasl;

namespace SimpleServer;

public sealed class Connection : IDisposable
{
    private Socket? _socket;
    private Stream? _stream;
    private ExpatXmppParser? _parser;
    private Jid? _jabberId = null;
    private volatile bool _disposed;
    private volatile FileAccess _access = FileAccess.ReadWrite;
    private ConcurrentQueue<byte[]> _writeQueue = [];

    public Connection(Socket socket)
    {
        _stream = new NetworkStream(_socket = socket, false);
        _parser = new ExpatXmppParser(ExpatEncoding.UTF8);
        _parser.OnStreamStart += OnStreamStart;
        _parser.OnStreamEnd += OnStreamEnd;
        _parser.OnStreamElement += OnStreamElement;
    }

    internal void Setup()
    {
        Server.RegisterConnection(this);

        new Thread(BeginSend)
        {
            Name = "Xmpp Connection/Send Thread",
            IsBackground = true
        }.Start();

        new Thread(BeginReceive)
        {
            Name = "Xmpp Connection/Receive Thread",
            IsBackground = true
        }.Start();
    }

    void BeginSend()
    {
        try
        {
            while (!_disposed)
            {
                Thread.Sleep(1);

                if (!_access.HasFlag(FileAccess.Write))
                    continue;

                while (_writeQueue.TryDequeue(out var buffer))
                {
                    if (_stream == null)
                        return;

                    Thread.Sleep(1);

                    _stream.Write(buffer);
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

    void BeginReceive()
    {
        var buf = new byte[256];

        try
        {
            while (!_disposed)
            {
                Thread.Sleep(1);

                if (!_access.HasFlag(FileAccess.Read))
                {
                    Console.WriteLine("read is paused");
                    continue;
                }

                if (_stream == null || _parser == null)
                    break;

                var len = _stream.Read(buf);

                if (len <= 0)
                    break;

                Console.WriteLine("recv (num bytes): " + len);

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

        _socket?.Dispose();
        _socket = null;

        _parser?.Dispose();
        _parser = null;

        Server.UnregisterConnection(this);
    }

    void SendInternal(byte[] buffer)
    {
        if (!_access.HasFlag(FileAccess.Write))
            return;

        _writeQueue.Enqueue(buffer);
    }

    public void Send(object data)
    {
        if (data is not string s)
            s = data.ToString()!;

        SendInternal(s.GetBytes());

        Console.WriteLine("send >>\n{0}\n", s);
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

    void OnStreamElement(Element e)
    {
        Console.WriteLine("recv <<\n{0}\n", e.ToString(true));

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
        }
    }
}
