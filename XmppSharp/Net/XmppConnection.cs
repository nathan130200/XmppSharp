using Expat;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using XmppSharp.Abstractions;
using XmppSharp.Dom;
using XmppSharp.Exceptions;
using XmppSharp.Net.EventArgs;
using XmppSharp.Protocol.Base;

namespace XmppSharp.Net;

/// <summary>
/// Represents the base class for XMPP connections.
/// </summary>
public abstract class XmppConnection : IXmppConnection, IDisposable
{
    public Jid Jid { get; protected set; }

    public bool IsConnected => _disposed == 0
        && _state > XmppConnectionState.Disconnected
        && _state < XmppConnectionState.Disconnecting;

    public bool IsAuthenticated => IsConnected && _state >= XmppConnectionState.Authenticated;
    public bool IsSessionStarted => _state == XmppConnectionState.SessionStarted;

    protected Socket? _socket;
    protected System.IO.Stream? _stream;
    protected volatile byte _disposed;
    protected volatile FileAccess _access = FileAccess.ReadWrite;
    protected XmppParser? _parser;
    protected readonly ConcurrentQueue<(byte[]? Bytes, TaskCompletionSource? Completion)> _writeQueue = [];

    public int RecvBufferSize { get; set; } = 4096;
    public int ReadIdleWaitTimeMs { get; set; } = 160;
    public int WriteIdleWaitTimeMs { get; set; } = 160;
    public int DisconnectWaitTimeMs { get; set; } = 3000;
    public XmppLogLevel LogLevel { get; set; } = XmppLogLevel.Information;

    public event Action<StateChangedEventArgs>? OnStateChanged;
    public event Action<XmppElementEventArgs>? OnElement;
    public event XmppLoggingDelegate? OnLog;

    private volatile XmppConnectionState _state;
    private readonly List<XmppCallbackInfo> _callbacks = [];

    protected void FireOnElement(XmppElement e)
        => OnElement?.Invoke(new() { Connection = this, Element = e });

    protected void FireOnLog(XmppLogLevel level, string message)
    {
        if (LogLevel < level)
            return;

        try
        {
            OnLog?.Invoke(new()
            {
                Sender = this,
                Timestamp = DateTime.Now,
                Level = level,
                Message = message
            });
        }
        catch (Exception ex)
        {
            Trace.TraceError(ex.ToString());
        }
    }

    /// <summary>
    /// Represents a callback used to handle XMPP elements with a specified name, function, and priority.
    /// </summary>
    protected class XmppCallbackInfo
    {
        /// <summary>
        /// Gets the name associated with this callback, primarily for debugging purposes.
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// Gets a function that determines whether a specified <see cref="XmppElement"/> satisfies a condition.
        /// </summary>
        public Func<XmppElement, bool> Function { get; init; }

        /// <summary>
        /// Gets the <see cref="TaskCompletionSource{TResult}"/> used to signal the completion of an operation  that
        /// produces an <see cref="XmppElement"/>.
        /// </summary>
        public TaskCompletionSource<XmppElement> Completion { get; init; }

        /// <summary>
        /// Gets the priority level of the XMPP element callback.
        /// </summary>
        public XmppCallbackPriority Priority { get; init; }
    }


    public async Task<XmppElement> WaitForElement(Func<XmppElement, bool> match,
        string? name = default,
        XmppCallbackPriority priority = XmppCallbackPriority.Normal,
        CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(match);

        var tcs = new TaskCompletionSource<XmppElement>();

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(token);

        cts.Token.Register(() => tcs.TrySetCanceled());

        XmppCallbackInfo cb;

        lock (_callbacks)
        {
            cb = new XmppCallbackInfo
            {
                Name = string.IsNullOrWhiteSpace(name) ? Guid.NewGuid().ToString("d") : name,
                Function = match,
                Completion = tcs,
                Priority = priority,
            };

            FireOnLog(XmppLogLevel.Verbose, $"WaitForElement(): Register callback ({cb.Name}): {match}");

            _callbacks.Add(cb);
        }

        try
        {
            return await tcs.Task;
        }
        finally
        {
            lock (_callbacks)
            {
                if (_callbacks.Remove(cb))
                    FireOnLog(XmppLogLevel.Verbose, $"WaitForElement(): Removed dead callback: {cb.Name}");

                tcs.TrySetCanceled();
            }
        }
    }

    public async Task<TStanza> RequestStanzaAsync<TStanza>(TStanza stz,
        XmppCallbackPriority priority = XmppCallbackPriority.Normal,
        CancellationToken token = default)
        where TStanza : Stanza
    {
        if (string.IsNullOrWhiteSpace(stz.Id))
            stz.GenerateId();

        var stanzaId = stz.Id;

        var task = WaitForElement(x => x is TStanza el && el.Id == stanzaId,
            $"stanza:{typeof(TStanza).Name.ToLower()}/{stanzaId}",
            priority,
            token);

        Send(stz);

        return (TStanza)await task;
    }

    public XmppConnectionState State => _state;

    protected void FireOnError(Exception ex, string? message = default)
    {
        OnLog?.Invoke(new()
        {
            Sender = this,
            Timestamp = DateTime.Now,
            Level = XmppLogLevel.Error,
            Message = message ?? ex.Message,
            Exception = ex
        });
    }

    /// <summary>
    /// Transitions the connection to a new state, ensuring state progression rules are followed.
    /// <para>
    /// This method enforces the following rules for state transitions:
    /// <list type="bullet">
    /// <item><description>Transitions to the <see cref="XmppConnectionState.Disconnected"/> state are always
    /// allowed.</description></item>
    /// <item><description>Transitions to a state earlier than the current state are not
    /// allowed, except for <see cref="XmppConnectionState.Disconnected"/>.</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// If the state changes successfully, the <see cref="OnStateChanged"/> event is raised to notify subscribers of the state change.
    /// </para>
    /// </summary>
    /// <param name="newValue">The new <see cref="XmppConnectionState"/> to transition to.</param>
    /// <exception cref="InvalidOperationException">Thrown if an attempt is made to transition to a state that is earlier than the current state, as this could
    /// corrupt the internal connection state.</exception>
    protected void GotoState(XmppConnectionState newValue)
    {
        lock (this)
        {
            var oldValue = _state;

            if (oldValue != newValue)
            {
                //
                // new state must be one of:
                // 
                // goes from any state to disconnected
                // new state >= last state 
                //
                // if we rollback state,
                // may corrupt connection
                // internal state
                //

                if (newValue != 0 && newValue < oldValue)
                {
                    FireOnLog(XmppLogLevel.Error, $"Rejected an attempt to rollback connection state ({oldValue} -> {newValue})");
                    throw new InvalidOperationException("The connection state cannot be rolled back.");
                }

                _state = newValue;

                FireOnLog(XmppLogLevel.Verbose, $"Connection state changed: {oldValue} -> {newValue}");

                try
                {
                    OnStateChanged?.Invoke(new()
                    {
                        Connection = this,
                        OldState = oldValue,
                        NewState = newValue
                    });
                }
                catch (Exception ex)
                {
                    FireOnError(ex);
                }
            }
        }
    }

    /// <summary>
    /// Initializes the XMPP parser and sets up event handlers for processing stream events.
    /// </summary>
    /// <remarks>This method configures the internal XMPP parser to handle incoming XML streams. It sets up 
    /// event handlers for stream start, stream elements, and stream end events. Each event handler  processes the
    /// corresponding XML data and invokes the appropriate callbacks. If an error occurs  during event processing, the
    /// error is logged, and the connection is terminated.</remarks>
    /// <exception cref="JabberStreamException"></exception>
    protected void InitParser()
    {
        FireOnLog(XmppLogLevel.Verbose, "Init parser");

        _parser?.Dispose();
        _parser = new XmppParser(ExpatEncoding.UTF8, true);

        _parser.OnStreamStart += e =>
        {
            OnReadXml(e.ToString());

            try
            {
                OnStreamStart(e);
            }
            catch (Exception ex)
            {
                FireOnError(ex);
                Disconnect();
            }
        };

        _parser.OnStreamElement += e =>
        {
            OnReadXml(e.ToString(false));

            try
            {
                if (e is StreamError se)
                    throw new JabberStreamException(se.Condition, se.Text);

                OnStreamElement(e);
            }
            catch (Exception ex)
            {
                FireOnError(ex);
                Disconnect();
            }
        };

        _parser.OnStreamEnd += () =>
        {
            OnReadXml(Xml.XmppEndTag);

            try
            {
                OnStreamEnd();
            }
            catch (Exception ex)
            {
                FireOnError(ex);
                Disconnect();
            }
        };
    }


    /// <summary>
    /// Called when a XMPP stream is received.
    /// </summary>
    /// <param name="e">The <see cref="Stream"/> instance representing the stream that has started.</param>
    protected virtual void OnStreamStart(Protocol.Base.Stream e)
    {

    }

    /// <summary>
    /// Handles an incoming XMPP stream element.
    /// </summary>
    /// <param name="e">The <see cref="XmppElement"/> representing the incoming stream element to process. Cannot be null.</param>
    protected virtual void OnStreamElement(XmppElement e)
    {

    }

    /// <summary>
    /// Invokes registered callbacks for the specified <see cref="XmppElement"/> in order of priority.
    /// </summary>
    /// <param name="e">The <see cref="XmppElement"/> to pass to the callbacks for processing.</param>
    protected void ProcessCallbacks(XmppElement e)
    {
        lock (_callbacks)
        {
            var entries = from o in _callbacks.Select((item, index) => (index, item))
                          orderby o.item.Priority descending
                          select o;

            foreach (var (index, cb) in entries)
            {
                if (cb.Function(e))
                {
                    FireOnLog(XmppLogLevel.Verbose, $"{nameof(ProcessCallbacks)}(): Callback[{index}] '{cb.Name}' triggered");
                    _callbacks.RemoveAt(index);
                    cb.Completion.TrySetResult(e);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Invoked when the end of a XMPP stream is reached.
    /// </summary>
    protected virtual void OnStreamEnd()
    {

    }

    /// <summary>
    /// Releases the resources used by the current instance of the class.
    /// </summary>
    /// <remarks>This method is intended to be overridden in derived classes to implement custom disposal
    /// logic. Ensure that any resources are properly released based on the value of the <paramref name="disposing"/>
    /// parameter.</remarks>
    /// <param name="disposing">A value indicating whether the method is being called during a full disposal process (<see langword="true"/>) or
    /// an early disposal process (<see langword="false"/>). When <see langword="true"/>, the instance is fully
    /// disconnected and no further interaction with the underlying socket or stream is allowed.</param>
    protected virtual void DisposeImpl(bool disposing)
    {

    }

    /// <summary>
    /// Releases all resources used by the current instance of the class.
    /// </summary>
    /// <remarks>This method ensures that the connection is properly terminated, resources are cleaned up, 
    /// and any pending operations are handled before disposal. It suspends the parser, cancels  pending write
    /// operations, and disposes of associated resources such as the parser, stream,  and socket. If there are no write
    /// errors and the write queue is empty, a delay is introduced  to allow for proper flushing of the queue before
    /// final disposal.</remarks>
    public void Dispose()
    {
        if (_disposed > 0)
            return;

        _disposed = 1;
        _access &= ~FileAccess.Read;
        GotoState(XmppConnectionState.Disconnecting);
        _parser?.Suspend(false);
        FireOnLog(XmppLogLevel.Verbose, "Dispose called. Begin release connection.");

        DisposeImpl(false);

        int timeoutMs = 160; // small delay, prevent spamming reconnects

        if (!_hasWriteError)
            timeoutMs = _writeQueue != null && !_writeQueue.IsEmpty
                ? Math.Max(DisconnectWaitTimeMs, 1000) : 500; // disconnect time must be at least >= 1 sec
                                                              // if write queue is not empty or else just half second
                                                              // delay to prevent spamming reconnects too.

        _ = Task.Delay(timeoutMs).ContinueWith(_ =>
        {
            // well we wait enough time here to things be sent.

            _disposed = 2;
            _access &= ~FileAccess.Write;

            if (_writeQueue != null)
            {
                while (_writeQueue.TryDequeue(out var entry) == true)
                    entry.Completion?.TrySetCanceled();
            }

            try
            {
                // i hope these below dont throw any exceptions.

                _parser?.Dispose();
                _parser = null;

                _stream?.Dispose();
                _stream = null;

                _socket?.Dispose();
                _socket = null;
            }
            catch (Exception ex)
            {
                FireOnError(ex);
            }
            finally
            {
                GotoState(XmppConnectionState.Disconnected);
                DisposeImpl(true);
            }

            FireOnLog(XmppLogLevel.Verbose, "Dispose task completed.");
        });

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Gets a value indicating whether verbose logging is enabled.
    /// </summary>
    protected bool IsVerbose => LogLevel >= XmppLogLevel.Verbose;

    /// <summary>
    /// Continuously reads data from the underlying stream and processes it using the XML parser.
    /// </summary>
    /// <param name="token">A <see cref="CancellationToken"/> used to signal cancellation of the read loop.</param>
    /// <returns></returns>
    protected async Task ReadLoop(CancellationToken token)
    {
        // expat needs at least 1024 bytes of data.

        var buf = new byte[Math.Max(1024, RecvBufferSize)];

        if (IsVerbose)
            FireOnLog(XmppLogLevel.Verbose, $"Begin read loop task. Buffer size is {buf.Length} bytes.");

        try
        {
            while (_disposed < 1)
            {
                await Task.Delay(ReadIdleWaitTimeMs, token);

                if (!_access.HasFlag(FileAccess.Read))
                {
                    if (IsVerbose)
                        FireOnLog(XmppLogLevel.Verbose, "Read loop paused.");

                    continue;
                }

                var len = await _stream!.ReadAsync(buf, token);

                if (IsVerbose)
                {
                    float ratio = len / (float)buf.Length * 100f;
                    FireOnLog(XmppLogLevel.Verbose, $"Read loop step. Read {len} bytes. (buffer usage: {ratio:F1}%)");
                }

                if (len <= 0)
                    break;

                _parser!.Parse(buf, len, len == 0);
            }

            FireOnLog(XmppLogLevel.Verbose, "Read loop gracefully completed.");
        }
        catch (Exception ex)
        {
            FireOnLog(XmppLogLevel.Verbose, "Read loop errored.");
            FireOnError(ex);
        }
        finally
        {
            Disconnect();
            FireOnLog(XmppLogLevel.Verbose, "End read loop task.");
            buf = null;
        }
    }

    // control if we can skip flushing queue if we had write error.
    volatile bool _hasWriteError;

    /// <summary>
    /// Continuously processes and writes data from the write queue to the underlying stream.
    /// </summary>
    /// <param name="token">A <see cref="CancellationToken"/> used to signal cancellation of the write loop.</param>
    /// <returns></returns>
    protected async Task WriteLoop(CancellationToken token)
    {
        FireOnLog(XmppLogLevel.Verbose, "Begin write loop task.");

        DateTime dt = default;

        try
        {
            while (_disposed < 2)
            {
                await Task.Delay(WriteIdleWaitTimeMs, token);

                if (!_access.HasFlag(FileAccess.Write))
                {
                    if (IsVerbose)
                        FireOnLog(XmppLogLevel.Verbose, "Write loop paused.");

                    continue;
                }

                if (!_writeQueue.IsEmpty)
                {
                    if (IsVerbose)
                    {
                        dt = DateTime.Now;
                        FireOnLog(XmppLogLevel.Verbose, "Begin write loop step");
                    }

                    while (_writeQueue.TryDequeue(out var entry))
                    {
                        try
                        {
                            // sometimes null bytes is enqueued to create some kind of notification entry.
                            // eg: if we will pause stream for TLS, we need ensure all data is sent before start handshake.
                            // so in this case add a null buffer to queue, but an completion source as callback.

                            if (entry.Bytes?.Length > 0)
                            {
                                await _stream!.WriteAsync(entry.Bytes!, token);

                                if (IsVerbose)
                                    FireOnLog(XmppLogLevel.Verbose, $"Write loop step: Wrote {entry.Bytes!.Length} bytes.");
                            }
                        }
                        finally
                        {
                            entry.Completion?.TrySetResult();
                        }
                    }

                    if (IsVerbose)
                    {
                        var elapsedTime = (DateTime.Now - dt).TotalSeconds;
                        FireOnLog(XmppLogLevel.Verbose, $"End write loop step. (took: {elapsedTime:0.00} sec.)");
                    }
                }
            }

            FireOnLog(XmppLogLevel.Verbose, "Write loop gracefully completed.");
        }
        catch (Exception ex)
        {
            _hasWriteError = true;

            while (_writeQueue.TryDequeue(out var entry))
                entry.Completion?.TrySetException(ex);

            FireOnError(ex);

            FireOnLog(XmppLogLevel.Verbose, "Write loop errored.");
        }
        finally
        {
            Dispose();

            FireOnLog(XmppLogLevel.Verbose, "End write loop task.");
        }
    }

    void OnWriteXml(string xml)
    {
        if (LogLevel < XmppLogLevel.Debug)
            return;

        FireOnLog(XmppLogLevel.Debug, $"send >>\n{xml}\n");
    }

    void OnReadXml(string xml)
    {
        if (LogLevel < XmppLogLevel.Debug)
            return;

        FireOnLog(XmppLogLevel.Debug, $"recv <<\n{xml}\n");
    }

    public void Send(XmppElement element)
    {
        if (State < XmppConnectionState.Connected)
            return;

        if (_disposed > 1)
            return;

        ArgumentNullException.ThrowIfNull(element);

        var xml = element.ToString(false);

        _writeQueue!.Enqueue(new()
        {
            Bytes = xml.GetBytes()
        });

        OnWriteXml(xml);
    }

    public Task SendAsync(XmppElement element)
    {
        if (_disposed > 1)
            return Task.CompletedTask;

        ArgumentNullException.ThrowIfNull(element);

        var tcs = new TaskCompletionSource();
        var xml = element.ToString(false);

        _writeQueue!.Enqueue(new()
        {
            Bytes = xml.GetBytes(),
            Completion = tcs
        });

        OnWriteXml(xml);

        return tcs.Task;
    }

    public void Disconnect(XmppElement? element = null)
    {
        if (_disposed > 0)
            return;

        if (!_hasWriteError)
        {
            var sb = new StringBuilder();

            if (element != null)
                sb.Append(element);

            var xml = sb.Append(Xml.XmppEndTag).ToString();

            _writeQueue?.Enqueue(new()
            {
                Bytes = xml.GetBytes()
            });

            OnWriteXml(xml);

            FireOnLog(XmppLogLevel.Verbose, "Queued disconnect packet.");
        }
        else
        {
            FireOnLog(XmppLogLevel.Verbose, "Unable to send disconnect packet due to write loop error.");
        }

        Dispose();
    }

    public void Disconnect(StreamErrorCondition condition, string? text = default)
        => Disconnect(new StreamError(condition, text));
}