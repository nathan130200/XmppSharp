using System.Collections.Concurrent;
using System.IO.Pipes;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using XmppSharp.Dom;
using XmppSharp.Protocol.Base;
using XmppSharp.Protocol.Core;

namespace XmppSharp.Net;

public abstract class XmppConnection
{
    protected volatile XmppConnectionState _state;

    protected ConcurrentDictionary<string, TaskCompletionSource<Stanza>> _callbacks = new();
    protected ConcurrentQueue<Func<CancellationToken, Task>> _taskQueue = new();
    protected ConcurrentQueue<(byte[] Bytes, string? Xml)> _sendQueue = new();
    protected ExpatXmppParser? _parser;
    protected Stream? _stream;
    protected Socket? _socket;

    // disposed flag:
    // 0 = not disposed
    // 1 = half disposed, disallow read
    // 2 = full disposed, disallow read & write, and cleanup connection.
    protected volatile byte _disposed = 0;

    // 
    protected volatile FileAccess _access = FileAccess.ReadWrite;

    public XmppConnectionOptions Options { get; } = new();

    public bool IsConnected => _state.HasFlag(XmppConnectionState.Connected);
    public bool IsEncrypted => _state.HasFlag(XmppConnectionState.Encrypted);
    public bool IsAuthenticated => _state.HasFlag(XmppConnectionState.Authenticated);
    public XmppConnectionState State => _state;

    public event Action<XmppConnection, Exception> OnError;
    public event Action<XmppConnection, (PipeDirection Direction, string Xml)> OnDebugXml;
    public event Action<XmppConnection, (XmppConnectionState Before, XmppConnectionState After)> OnStateChanged;
    public event Action<XmppConnection, Stanza> OnStanza;

    protected void ChangeState(Func<XmppConnectionState, XmppConnectionState> updateVaue)
    {
        lock (this)
        {
            var oldState = _state;
            var newState = updateVaue(oldState);

            if (oldState != newState)
            {
                _state = newState;
                OnStateChanged?.Invoke(this, (oldState, newState));
            }
        }
    }

    protected void FireOnError(Exception ex) => OnError?.Invoke(this, ex);
    protected void FireOnReadXml(string xml) => OnDebugXml?.Invoke(this, (PipeDirection.In, xml));
    protected void FireOnWriteXml(string xml) => OnDebugXml?.Invoke(this, (PipeDirection.Out, xml));
    protected void FireOnStanza(Stanza stz) => OnStanza?.Invoke(this, stz);

    protected virtual async Task BeginReceive(CancellationToken token)
    {
        var buf = new byte[Math.Max(1024, Options.RecvBufferSize)];

        try
        {
            int len;

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

                len = await _stream!.ReadAsync(buf, token);

                if (len <= 0)
                    break;

                _parser?.Parse(buf, len);
            }
        }
        catch (Exception ex)
        {
            FireOnError(ex);
            Disconnect();
        }

        buf = null;
    }

    public async Task<Iq?> RequestIqAsync(Iq stz, TimeSpan timeout = default, CancellationToken token = default)
    {
        var result = await RequestStanzaAsync(stz, timeout, token); ;
        return result as Iq;
    }

    public async Task<Message?> RequestMessageAsync(Message stz, TimeSpan timeout = default, CancellationToken token = default)
    {
        var result = await RequestStanzaAsync(stz, timeout, token); ;
        return result as Message;
    }

    public async Task<Presence?> RequestPresenceAsync(Presence stz, TimeSpan timeout = default, CancellationToken token = default)
    {
        var result = await RequestStanzaAsync(stz, timeout, token);
        return result as Presence;
    }

    public async Task<Stanza> RequestStanzaAsync(Stanza stz, TimeSpan timeout = default, CancellationToken token = default)
    {
        if (string.IsNullOrWhiteSpace(stz.Id))
            stz.GenerateId();

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(token);

        var tcs = new TaskCompletionSource<Stanza>();
        _callbacks[stz.Id!] = tcs;

        if (timeout != default)
            cts.CancelAfter(timeout);

        Send(stz);

        return await tcs.Task;
    }

    protected abstract void SendStreamHeader();

    protected virtual async Task BeginSend(CancellationToken token)
    {
        try
        {
            while (_disposed < 2)
            {
                await Task.Delay(16, token);

                if (_sendQueue.TryDequeue(out var tuple))
                {
                    var (bytes, xml) = tuple;

                    await _stream!.WriteAsync(bytes, token);

                    if (!string.IsNullOrWhiteSpace(xml))
                        FireOnWriteXml(xml);
                }
            }
        }
        catch (Exception ex)
        {
            FireOnError(ex);
            Disconnect();
        }
    }

    protected virtual void QueueWrite(byte[] bytes, string? xml = default)
        => _sendQueue.Enqueue((bytes, xml));

    protected virtual void QueueTask(Func<CancellationToken, Task> action)
    {
        if (_disposed > 0)
            return;

        _taskQueue.Enqueue(action);
    }

    public virtual void Send(string xml)
    {
        if (_disposed > 1) // cannot write anymore
            return;

        QueueWrite(xml.GetBytes(), Options.Verbose ? xml : default);
    }

    public virtual void Send(XmppElement element)
    {
        if (_disposed > 1) // cannot write anymore
            return;

        var xml = element.GetBytes();

        QueueWrite(xml, Options.Verbose ? element.ToString(true) : default);
    }

    public virtual void Disconnect(string? xml = default)
    {
        var buf = new StringBuilder();

        if (!string.IsNullOrWhiteSpace(xml))
            buf.Append(xml);

        xml = buf.Append(Xml.XmppStreamEnd).ToString();

        QueueWrite(xml.GetBytes(), Options.Verbose ? xml : default);

        Dispose();
    }

    public virtual async Task ConnectAsync(CancellationToken token = default)
    {
        if (_state.HasFlag(XmppConnectionState.Connected))
            return;

        _disposed = 0;
        _access = FileAccess.ReadWrite;

        ChangeState(_ => XmppConnectionState.Connected);

        try
        {
            _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            await _socket.ConnectAsync(Options.EndPoint, token);

            _stream = new NetworkStream(_socket, true);

            if (Options.StartTlsOnConnect)
            {
                _stream = new SslStream(_stream);
                await ((SslStream)_stream).AuthenticateAsClientAsync(Options.TlsOptions, token);
                _state |= XmppConnectionState.Encrypted;
            }

            InitParser();
            SendStreamHeader();

            _ = BeginReceive(token);
            _ = BeginSend(token);
        }
        catch (Exception ex)
        {
            FireOnError(ex);
            ChangeState(_ => XmppConnectionState.Disconnected);
        }
    }

    protected virtual void HandleStreamStart(StreamStream e)
    {

    }

    protected virtual void HandleStreamElement(XmppElement e)
    {

    }

    protected virtual void InitParser()
    {
        _parser = new ExpatXmppParser(Options.Encoding);

        _parser.OnStreamStart += e =>
        {
            if (Options.Verbose)
                FireOnReadXml(e.StartTag());

            try
            {
                HandleStreamStart(e);
            }
            catch (Exception ex)
            {
                FireOnError(ex);
                Disconnect();
            }
        };

        _parser.OnStreamElement += e =>
        {
            if (Options.Verbose)
                FireOnReadXml(e.ToString(true));

            try
            {
                if (e is StreamError se)
                {
                    var condition = se.Condition ?? StreamErrorCondition.UndefinedCondition;
                    throw new JabberStreamException(condition, se.Text ?? condition.ToString());
                }

                if (e is Stanza stz && !string.IsNullOrEmpty(stz.Id) && _callbacks.TryRemove(stz.Id, out var cb))
                {
                    cb.TrySetResult(stz);
                    return;
                }

                HandleStreamElement(e);
            }
            catch (Exception ex)
            {
                FireOnError(ex);
                Disconnect();
            }
        };

        _parser.OnStreamEnd += () =>
        {
            if (Options.Verbose)
                FireOnReadXml(Xml.XmppStreamEnd);

            Disconnect();
        };
    }

    protected virtual void Disposing()
    {

    }

    public void Dispose()
    {
        if (_disposed > 0)
            return;

        _disposed = 1;
        _access &= ~FileAccess.Read;
        _parser?.XmlParser?.Suspend(false);

        foreach (var (_, tcs) in _callbacks)
            tcs.TrySetCanceled();

        _callbacks.Clear();

        Disposing();

        _ = Task.Delay(Options.DisconnectTimeout)
                .ContinueWith(_ =>
                {
                    _parser?.Dispose();
                    _parser = null;

                    _stream?.Dispose();
                    _stream = null;

                    ChangeState(_ => XmppConnectionState.Disconnected);
                });
    }

    public virtual void Disconnect(XmppElement element)
    {
        Throw.IfNull(element);

        var buf = new StringBuilder()
            .Append(element.ToString(false))
            .Append(Xml.XmppStreamEnd)
            .ToString();

        QueueWrite(buf.GetBytes(), Options.Verbose ? buf.ToString() : default);

        Dispose();
    }
}