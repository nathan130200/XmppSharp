using System.Collections.Concurrent;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using XmppSharp.Dom;
using XmppSharp.Entities;
using XmppSharp.Entities.Options;
using XmppSharp.Exceptions;
using XmppSharp.Protocol.Base;

namespace XmppSharp.Net;

public abstract class XmppConnection
{
    volatile XmppConnectionState _state;

    protected ConcurrentDictionary<string, TaskCompletionSource<Stanza>> _callbacks = new();
    protected ConcurrentQueue<Func<CancellationToken, Task>> _taskQueue = new();
    protected ConcurrentQueue<(byte[] Bytes, string? Xml, TaskCompletionSource? Completion)> _sendQueue = new();
    protected IXmppParser? _parser;
    protected Stream? _stream;
    protected Socket? _socket;

    public string? StreamId { get; protected set; }
    public Jid Jid { get; protected set; }

    // disposed flag:
    // 0 = not disposed
    // 1 = half disposed, disallow read
    // 2 = full disposed, disallow read & write, and cleanup connection.
    protected volatile byte _disposed = 0;
    protected volatile FileAccess _access = FileAccess.ReadWrite;

    public XmppConnectionOptions Options { get; }

    protected internal ILogger Logger => Options.Logger ?? NullLogger.Instance;

    public XmppConnection(XmppConnectionOptions options)
        => Options = options;

    void Setup()
    {
        _tokenSource?.Dispose();
        _tokenSource?.Cancel();
        _tokenSource = null!;

        _callbacks?.Clear();
        _callbacks = new();

        _taskQueue?.Clear();
        _taskQueue = new();

        if (_sendQueue != null)
        {
            while (_sendQueue.TryDequeue(out var t))
                t.Completion?.TrySetCanceled();
        }

        _sendQueue = new();
    }

    protected virtual void InitKeepAlive()
    {
        if (!Options.EnableKeepAlive)
            return;

        var buf = " ".GetBytes();

        var token = _tokenSource.Token;

        _ = Task.Run(async () =>
        {
            while (!_tokenSource.IsCancellationRequested)
            {
                if (_disposed > 0)
                {
                    Logger.LogDebug("Keep alive aborted! Connection is disposing...");
                    break;
                }

                var tcs = new TaskCompletionSource();

                try
                {
                    Logger.LogDebug("Sending keep alive...");

                    _sendQueue.Enqueue((buf, null, tcs));

                    var delayTask = Task.Delay(Options.KeepAliveTimeout, token);

                    var result = await Task.WhenAny(
                        delayTask,
                        tcs.Task);

                    if (result == delayTask)
                    {
                        Logger.LogDebug("Keep alive timeout! Connection is dead.");
                        Dispose();
                        return;
                    }

                    Logger.LogDebug("Keep alive sent! Connection is alive.");
                }
                catch (Exception ex)
                {
                    Logger.LogDebug(ex, "Sending keep alive failed.");
                    Dispose();
                    break;
                }

                await Task.Delay(Options.KeepAliveInterval, token);
            }
        });
    }

    public bool IsConnected => _state.HasFlag(XmppConnectionState.Connected);
    public bool IsEncrypted => _state.HasFlag(XmppConnectionState.Encrypted);
    public bool IsAuthenticated => _state.HasFlag(XmppConnectionState.Authenticated);
    public XmppConnectionState State => _state;

    public event Action<XmppConnection, (XmppConnectionState Before, XmppConnectionState After)> OnStateChanged;
    public event Action<XmppConnection, Stanza> OnStanza;
    public event Action<XmppConnection> OnReady;
    public event Action<XmppConnection> OnDisconnected;

    protected void ChangeState(Func<XmppConnectionState, XmppConnectionState> updateVaue)
    {
        lock (this)
        {
            var oldState = _state;
            var newState = updateVaue(oldState);

            if (oldState != newState)
            {
                _state = newState;
                Logger.LogTrace("Changed state to: {NewState}", newState);
                OnStateChanged?.Invoke(this, (oldState, newState));
            }
        }
    }

    protected void FireOnDisconnected() => OnDisconnected?.Invoke(this);

    protected void FireOnReady() => OnReady?.Invoke(this);

    protected void FireOnStanza(Stanza stz) => OnStanza?.Invoke(this, stz);

    protected virtual async Task BeginReceive()
    {
        var token = _tokenSource.Token;
        var buf = new byte[Math.Max(1024, Options.RecvBufferSize)];

        try
        {
            int len;

            while (_disposed < 1)
            {
                await Task.Delay(16, token);

                while (_taskQueue.TryDequeue(out var action))
                {
                    await action(token);
                    continue;
                }

                if (!_access.HasFlag(FileAccess.Read))
                    continue;

                len = await _stream!.ReadAsync(buf, token);

                if (len <= 0)
                    break;

                ((ExpatXmppParser)_parser!).Parse(buf, len);
            }

            Logger.LogDebug("Read loop task terminated gracefully.");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An error occurred in the read loop task.");
        }
        finally
        {
            Disconnect();
        }
    }

    public async Task<TStanza> RequestStanzaAsync<TStanza>(TStanza request, TimeSpan timeout = default, CancellationToken token = default)
        where TStanza : Stanza
    {
        if (string.IsNullOrWhiteSpace(request.Id))
            request.GenerateId();

        var tcs = new TaskCompletionSource<Stanza>();

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(token);
        cts.Token.Register(() => tcs.TrySetCanceled());

        _callbacks[request.Id!] = tcs;

        if (timeout != default)
            cts.CancelAfter(timeout);

        Send(request);

        var result = await tcs.Task;

        return (TStanza)result;
    }

    protected abstract void SendStreamHeader();

    protected virtual async Task BeginSend()
    {
        var token = _tokenSource.Token;

        try
        {
            while (_disposed < 2)
            {
                await Task.Delay(16, token);

                if (_sendQueue == null)
                    break;

                if (_sendQueue.TryDequeue(out var tuple))
                {
                    var (bytes, xml, tcs) = tuple;

                    try
                    {
                        await _stream!.WriteAsync(bytes, CancellationToken.None);

                        if (!string.IsNullOrWhiteSpace(tuple.Xml))
                            Logger.LogTrace("send >>\n{Xml}\n", tuple.Xml);
                    }
                    catch (Exception ex)
                    {
                        tcs?.TrySetException(ex);
                        throw;
                    }
                    finally
                    {
                        tcs?.TrySetResult();
                    }
                }
            }

            Logger.LogDebug("Write loop task terminated gracefully.");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An error occurred in the write loop task.");
        }
        finally
        {
            Dispose();
        }
    }

    protected virtual void QueueWrite(byte[] bytes, string? xml = default, TaskCompletionSource? completion = default)
        => _sendQueue.Enqueue((bytes, xml, completion));

    protected virtual void QueueTask(Func<CancellationToken, Task> action)
    {
        if (_disposed > 0)
            return;

        _taskQueue.Enqueue(action);
    }

    public virtual void Send(string xml)
    {
        if (_disposed > 1)
            return;

        if (string.IsNullOrEmpty(xml))
            return;

        var verbose = Logger.IsEnabled(LogLevel.Trace);
        QueueWrite(xml.GetBytes(), verbose ? xml : default);
    }

    public virtual void Send(XmppElement element)
    {
        if (_disposed > 1)
            return;

        Throw.IfNull(element);

        var xml = element.GetBytes();
        var isVerbose = Logger.IsEnabled(LogLevel.Trace);
        QueueWrite(xml, isVerbose ? element.ToString(true) : default);
    }

    public Task SendAsync(string xml)
    {
        if (_disposed > 1)
            return Task.FromCanceled(CancellationToken.None);

        if (string.IsNullOrEmpty(xml))
            return Task.CompletedTask;

        var verbose = Logger.IsEnabled(LogLevel.Trace);

        var tcs = new TaskCompletionSource();
        QueueWrite(xml.GetBytes(), verbose ? xml : default, tcs);
        return tcs.Task;
    }

    public Task SendAsync(XmppElement element)
    {
        if (_disposed > 1)
            return Task.FromCanceled(CancellationToken.None);

        Throw.IfNull(element);

        var xml = element.GetBytes();
        var isVerbose = Logger.IsEnabled(LogLevel.Trace);
        var tcs = new TaskCompletionSource();
        QueueWrite(xml, isVerbose ? element.ToString(true) : default, tcs);
        return tcs.Task;
    }

    volatile bool _isConnecting = false;

    CancellationTokenSource _tokenSource;

    public virtual async Task ConnectAsync(CancellationToken token = default)
    {
        if (_isConnecting)
        {
            Logger.LogDebug("Trying to connect while connection attempt is already in progress.");
            return;
        }

        _isConnecting = true;

        if (_state.HasFlag(XmppConnectionState.Connected))
            return;

        Setup();

        _disposed = 0;
        _access = FileAccess.ReadWrite;

        _tokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);

        try
        {
            Options.Validate();

            _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            await _socket.ConnectAsync(Options.EndPoint, token);

            ChangeState(_ => XmppConnectionState.Connected);

            _stream = new NetworkStream(_socket, true);

            if (Options.StartTlsOnConnect)
            {
                _stream = new SslStream(_stream);
                await ((SslStream)_stream).AuthenticateAsClientAsync(Options.TlsOptions, token);
                _state |= XmppConnectionState.Encrypted;
            }

            InitParser();
            SendStreamHeader();

            _ = BeginReceive();
            _ = BeginSend();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Connection failed");
            Dispose();
        }
        finally
        {
            _isConnecting = false;
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
            if (Logger.IsEnabled(LogLevel.Trace))
                Logger.LogTrace("recv <<\n{Xml}\n", e.StartTag());

            try
            {
                HandleStreamStart(e);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "An error occurred while processing XMPP stream start.");

                XmppElement? element = null;

                if (ex is JabberStreamException jse)
                    element = new StreamError(jse.Condition, jse.Message);

                Disconnect(element);
            }
        };

        _parser.OnStreamElement += e =>
        {
            if (Logger.IsEnabled(LogLevel.Trace))
                Logger.LogTrace("recv <<\n{Xml}\n", e.ToString(true));

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
            catch (JabberSaslException ex)
            {
                Logger.LogError(ex, "SASL authentication failed.");
                Disconnect();
            }
            catch (JabberStreamException ex)
            {
                Logger.LogError(ex, "The remote server closed the connection with an error.");
                Disconnect();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "An error occurred while processing XMPP stream element.");
                Disconnect();
            }
        };

        _parser.OnStreamEnd += () =>
        {
            Logger.LogDebug("XMPP stream end received from the server. Closing connection...");

            if (Logger.IsEnabled(LogLevel.Trace))
                Logger.LogTrace("recv <<\n{Xml}\n", Xml.XmppStreamEnd);

            try
            {
                HandleStreamEnd();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "An error occurred while processing XMPP stream end.");
            }
            finally
            {
                Disconnect();
            }
        };
    }

    protected virtual void HandleStreamEnd()
    {

    }

    protected virtual void Disposing()
    {
        if (_parser is ExpatXmppParser expat)
            expat.XmlParser?.Suspend(false);
    }

    public void Dispose()
    {
        if (_disposed > 0)
            return;

        _disposed = 1;
        _access &= ~FileAccess.Read;

        _tokenSource?.Cancel();
        _tokenSource?.Dispose();
        _tokenSource = null!;

        {
            Logger.LogDebug("Cancel {Num} pending stanza callbacks.", _callbacks.Count);

            foreach (var (_, tcs) in _callbacks)
                tcs.TrySetCanceled();

            _callbacks.Clear();
        }

        if (!_taskQueue.IsEmpty)
            Logger.LogDebug("There are still {Num} pending tasks in the queue (may cause memory leaks)", _taskQueue.Count);

        Disposing();

        _ = Task.Run(async () =>
        {
            var timeout = Task.Delay(Options.DisconnectTimeout);

            Logger.LogDebug("Flush send queue...");

            while (!_sendQueue.IsEmpty)
            {
                await Task.Delay(160);

                if (timeout.IsCompleted)
                    break;
            }

            _sendQueue.Clear();
            _sendQueue = null!;

            Logger.LogDebug("Send queue {State}!", timeout.IsCompleted ? "timed out" : "completed");

            _disposed = 2;
            _access &= ~FileAccess.Write;

            _parser?.Dispose();
            _parser = null;

            _stream?.Dispose();
            _stream = null;

            ChangeState(_ => XmppConnectionState.Disconnected);

            FireOnDisconnected();
        });
    }

    public virtual void Disconnect(XmppElement? element = default)
    {
        var buf = new StringBuilder();

        if (element != null)
            buf.Append(element.ToString(false));

        var xml = buf.Append(Xml.XmppStreamEnd).ToString();
        var isVerbose = Logger.IsEnabled(LogLevel.Trace) == true;
        QueueWrite(xml.GetBytes(), isVerbose ? xml : default);

        Dispose();
    }
}