using System.Buffers;
using System.Collections.Concurrent;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using XmppSharp.Dom;
using XmppSharp.Parser;
using XmppSharp.Protocol.Base;
using XmppSharp.Protocol.Core;

namespace XmppSharp;

public abstract class XmppConnection : IDisposable
{
    public Jid? Jid { get; set; }
    public XmppConnectionState State => _connectionState;

    internal readonly bool _leaveSocketOpen = false;
    internal Socket? _socket;
    internal Stream? _stream;
    internal volatile ConcurrentQueue<(string xml, byte[] bytes)>? _sendQueue;
    internal StanzaGrabber<Iq>? _iqGrabber;
    internal StanzaGrabber<Message>? _messageGrabber;
    internal StanzaGrabber<Presence>? _presenceGrabber;
    internal volatile XmppConnectionState _connectionState;
    internal volatile XmppSocketState _socketState;
    internal XmppParser? _parser;

    public XmppConnection()
    {

    }

    public XmppConnection(Socket socket)
    {
        _socket = socket;
        _stream = new NetworkStream(socket, false);
        _leaveSocketOpen = true;
    }

    public void SetParser(XmppParser newParser)
    {
        if (_parser != null)
        {
            _parser.Dispose();
            _parser = null;
        }

        if (_connectionState != XmppConnectionState.Disconnected)
            throw new InvalidOperationException("Unable to change parser while connection is active.");

        ThrowHelper.ThrowIfNull(newParser);

        _parser = newParser;
    }

    void InitParser()
    {
        _parser ??= new XmppStreamReader(_stream!);

        _parser.OnStreamStart += e =>
        {
            try
            {
                AsyncHelper.RunSync(() => HandleStreamStart(e));
            }
            catch (Exception ex)
            {
                OnError?.Invoke(ex);
                Disconnect();
            }
        };

        _parser.OnStreamElement += e =>
        {
            try
            {
                AsyncHelper.RunSync(() => HandleStreamElement(e));
            }
            catch (Exception ex)
            {
                OnError?.Invoke(ex);
                Disconnect();
            }
        };

        _parser.OnStreamEnd += () =>
        {
            OnReadXml?.Invoke(Xml.XmppStreamEnd);
            Disconnect();
        };

        if (_parser is IXmppStreamTokenizer tokenizer)
            _ = Task.Run(() => TokenizeXmlAsync(tokenizer));
        else if (_parser is IXmppStreamReader reader)
            _ = Task.Run(() => ReadXmlAsync(reader));
        else
            throw new InvalidOperationException("Unknown parser type.");
    }

    async Task ReadXmlAsync(IXmppStreamReader reader)
    {
        try
        {
            while (!_socketState.HasFlag(XmppSocketState.Disposing))
            {
                await Task.Delay(1);

                if (!_socketState.HasFlag(XmppSocketState.Readable))
                    continue;

                if (!reader.Advance())
                    break;
            }
        }
        catch (Exception ex)
        {
            OnError?.Invoke(ex);
        }
        finally
        {
            Disconnect();
        }
    }

    static int s_DefaultBufferSize = 1024;

    public static int DefaultBufferSize
    {
        set
        {
            if (value <= 0)
                value = 1024;

            s_DefaultBufferSize = value;
        }
    }

    async Task TokenizeXmlAsync(IXmppStreamTokenizer tokenizer)
    {
        var buf = ArrayPool<byte>.Shared.Rent(s_DefaultBufferSize);

        try
        {
            while (!_socketState.HasFlag(XmppSocketState.Disposing))
            {
                await Task.Delay(1);

                if (!_socketState.HasFlag(XmppSocketState.Readable))
                    continue;

                if (_stream == null)
                    break;

                int len = await _stream.ReadAsync(buf);

                if (len <= 0)
                    break;

                tokenizer.Write(buf, len);
            }
        }
        catch (Exception ex)
        {
            OnError?.Invoke(ex);
        }
        finally
        {
            Disconnect();
            ArrayPool<byte>.Shared.Return(buf);
        }
    }

    public bool IsConnected
        => _connectionState.HasFlag(XmppConnectionState.Connected)
        && !_socketState.HasFlag(XmppSocketState.Disposing);

    Task? _connectTask;

    public void Connect(string host, int port)
    {
        if (_connectTask != null)
            throw new InvalidOperationException("A connection attempt is already in progress.");

        var endpoint = new DnsEndPoint(host, port);

        _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);

        _connectTask = Task.Run(async () =>
        {
            try
            {
                await _socket.ConnectAsync(endpoint);
                _stream = new NetworkStream(_socket, false);
                OnSocketConnected();
            }
            catch (Exception ex)
            {
                OnError?.Invoke(ex);
                Dispose();
            }
            finally
            {
                _connectTask = null!;
            }
        });
    }

    public event Action<Iq>? OnIq;
    public event Action<Message>? OnMessage;
    public event Action<Presence>? OnPresence;
    public event Action<Element>? OnXml;

    public event Action<string>? OnReadXml;
    public event Action<string>? OnWriteXml;
    public event Action<Exception>? OnError;

    public void Disconnect(StreamErrorCondition? condition = default, string? text = default)
    {
        if (_socketState.HasFlag(XmppSocketState.Disposed))
            return;

        var xml = new StringBuilder();

        if (condition.HasValue)
        {
            xml.Append(new StreamError(condition)
            {
                Text = text
            });
        }

        xml.Append(Xml.XmppStreamEnd);

        Send(xml.ToString());

        Dispose();
    }

    public virtual void Send(string xml)
    {
        if (_socketState.HasFlag(XmppSocketState.Disposed))
            return;

        _sendQueue?.Enqueue((xml, xml.GetBytes()));
    }

    public virtual void Send(Element e)
    {
        if (_socketState.HasFlag(XmppSocketState.Disposed))
            return;

        _sendQueue?.Enqueue((e.ToString(true), e.GetBytes()));
    }

    ~XmppConnection()
    {
        _stream = null;
        _socket = null;
        _parser = null;
        _sendQueue = null;
        _iqGrabber = null;
        _messageGrabber = null;
        _presenceGrabber = null;
    }

    public void Dispose()
    {
        if (_socketState.HasFlag(XmppSocketState.Disposing))
            return;

        _socketState &= ~XmppSocketState.Readable;
        _socketState |= XmppSocketState.Disposing;

        if (!_leaveSocketOpen)
            _socket?.Shutdown(SocketShutdown.Receive);

        _iqGrabber?.Dispose();
        _messageGrabber?.Dispose();
        _presenceGrabber?.Dispose();

        _parser?.Dispose();

        _ = Task.Run(async () =>
        {
            // wait for pending data

            while (_sendQueue != null && !_sendQueue.IsEmpty)
                await Task.Delay(160);

            _socketState &= ~XmppSocketState.Writable;
            _socketState |= XmppSocketState.Disposed;

            if (!_leaveSocketOpen)
                _socket?.Dispose();

            _stream?.Dispose();
        });

        GC.SuppressFinalize(this);
    }

    public event Action<XmppConnection, (XmppConnectionState Before, XmppConnectionState After)>? OnStateChanged;

    protected void ChangeState(XmppConnectionState newState, bool replace = false)
    {
        var oldState = _connectionState;

        if (replace)
            _connectionState = newState;
        else
            _connectionState |= newState;

        OnStateChanged?.Invoke(this, (oldState, newState));
    }

    protected virtual void OnSocketConnected()
    {
        _connectTask = null;

        try
        {
            InitParser();
            ChangeState(XmppConnectionState.Connected, true);
            SendStreamHeader();
        }
        catch (Exception ex)
        {
            OnError?.Invoke(ex);
            Dispose();
        }
    }

    public string StreamId
    {
        get;
        protected set;
    }

    protected virtual void SendStreamHeader()
    {
        var cultureName = CultureInfo.CurrentCulture.Name;

        if (string.IsNullOrWhiteSpace(cultureName))
            cultureName = "en";

        var header = new StreamStream
        {
            To = Jid!.Domain,
            Language = cultureName,
            Version = "1.0"
        };

        Send(header.StartTag());
    }

    protected virtual Task HandleStreamStart(StreamStream e)
    {
        if (string.IsNullOrWhiteSpace(e.Id))
            Disconnect(StreamErrorCondition.UnsupportedStanzaType);
        else
            StreamId = e.Id;

        return Task.CompletedTask;
    }

    protected virtual Task HandleStreamElement(Element e)
    {
        if (e is StreamError error)
            ThrowHelper.Rethrow(new JabberStreamException(error));

        return Task.CompletedTask;
    }

    protected virtual Task HandleStreamEnd()
    {
        Disconnect();
        return Task.CompletedTask;
    }
}
