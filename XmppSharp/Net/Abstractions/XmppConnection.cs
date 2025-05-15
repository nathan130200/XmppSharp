using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using Expat;
using XmppSharp.Dom;
using XmppSharp.Exceptions;
using XmppSharp.Protocol.Base;

namespace XmppSharp.Net.Abstractions;

public abstract class XmppConnection : IDisposable
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
    protected ConcurrentQueue<(byte[]? Bytes, TaskCompletionSource? Completion)>? _writeQueue;

    public int RecvBufferSize { get; set; } = 4096;
    public int ReadIdleWaitTimeMs { get; set; } = 160;
    public int WriteIdleWaitTimeMs { get; set; } = 160;
    public int DisconnectWaitTimeMs { get; set; } = 3000;

    public event Action<(XmppConnection Connection, XmppConnectionState OldState, XmppConnectionState NewState)> OnStateChanged;
    public event Action<(XmppConnection Connection, Exception Exception)> OnError;
    public event Action<(XmppConnection Connection, string Xml)>? OnReadXml;
    public event Action<(XmppConnection Connection, string Xml)>? OnWriteXml;

    volatile XmppConnectionState _state;

    public XmppConnectionState State => _state;

    protected void FireOnError(Exception ex)
    {
        OnError?.Invoke(new()
        {
            Connection = this,
            Exception = ex
        });
    }

    protected void GotoState(XmppConnectionState newValue)
    {
        lock (this)
        {
            var oldValue = _state;

            if (oldValue != newValue)
            {
                //
                // new state must be:
                // 
                // goes from any state to disconnected
                // new state >= last state 
                //
                // if we rollback state,
                // may corrupt connection
                // internal state
                //

                if (newValue != 0 && newValue < oldValue)
                    throw new InvalidOperationException("The connection state cannot be rolled back.");

                _state = newValue;

                OnStateChanged?.Invoke(new()
                {
                    Connection = this,
                    OldState = oldValue,
                    NewState = newValue
                });
            }
        }
    }

    protected void InitParser()
    {
        _parser?.Dispose();
        _writeQueue = [];
        _parser = new XmppParser(ExpatEncoding.UTF8, true);

        _parser.OnStreamStart += e =>
        {
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

    protected virtual void OnStreamStart(Protocol.Base.Stream e)
    {

    }

    protected virtual void OnStreamElement(XmppElement e)
    {

    }

    protected virtual void OnStreamEnd()
    {

    }

    protected virtual void DisposeImpl(bool disposing)
    {

    }

    public void Dispose()
    {
        if (_disposed > 0)
            return;

        _disposed = 1;
        _access &= ~FileAccess.Read;
        GotoState(XmppConnectionState.Disconnecting);
        _parser?.Suspend(false);

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

                _writeQueue = null;
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
        });

        GC.SuppressFinalize(this);
    }

    protected async Task ReadLoop(CancellationToken token)
    {
        var buf = new byte[Math.Max(1024, RecvBufferSize)];

        try
        {
            while (_disposed < 1)
            {
                await Task.Delay(ReadIdleWaitTimeMs, token);

                if (!_access.HasFlag(FileAccess.Read))
                    continue;

                var len = await _stream!.ReadAsync(buf, token);

                if (len <= 0)
                    break;

                _parser!.Parse(buf, len, len == 0);
            }
        }
        catch (Exception ex)
        {
            FireOnError(ex);
        }
        finally
        {
            Disconnect();
            buf = null;
        }
    }

    volatile bool _hasWriteError;

    protected async Task WriteLoop(CancellationToken token)
    {
        try
        {
            var exceptions = new List<Exception>();

            while (_disposed < 2)
            {
                await Task.Delay(WriteIdleWaitTimeMs, token);

                if (!_access.HasFlag(FileAccess.Write))
                    continue;

                var queue = _writeQueue;

                if (queue == null)
                    break;

                while (queue.TryDequeue(out var entry))
                {
                    try
                    {
                        if (entry.Bytes?.Length > 0)
                            await _stream!.WriteAsync(entry.Bytes!, token);
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);
                        entry.Completion?.TrySetException(ex);
                    }
                    finally
                    {
                        entry.Completion?.TrySetResult();
                    }
                }

                if (exceptions.Count > 0)
                    throw new AggregateException(exceptions);
            }
        }
        catch (Exception ex)
        {
            _hasWriteError = true;
            FireOnError(ex);
        }
        finally
        {
            Disconnect();
        }
    }

    public void Send(XmppElement element)
    {
        if (_disposed > 1)
            return;

        ArgumentNullException.ThrowIfNull(element);

        var xml = element.ToString(false);

        _writeQueue!.Enqueue(new()
        {
            Bytes = xml.GetBytes()
        });

        OnWriteXml?.Invoke(new()
        {
            Connection = this,
            Xml = xml
        });
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

        OnWriteXml?.Invoke(new()
        {
            Connection = this,
            Xml = xml
        });

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

            var xml = sb.Append(Xml.XmppStreamEnd).ToString();

            _writeQueue?.Enqueue(new()
            {
                Bytes = xml.GetBytes()
            });

            OnWriteXml?.Invoke(new()
            {
                Connection = this,
                Xml = xml
            });
        }

        Dispose();
    }

    public void Disconnect(StreamErrorCondition condition, string? text = default)
        => Disconnect(new StreamError(condition, text));
}