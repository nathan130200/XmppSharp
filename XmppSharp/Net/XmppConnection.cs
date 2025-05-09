﻿#nullable enable annotations

using System.Buffers;
using System.Collections.Concurrent;
using System.Text;
using XmppSharp.Dom;
using XmppSharp.Exceptions;
using XmppSharp.Net.Extensions.Abstractions;
using XmppSharp.Protocol.Base;

namespace XmppSharp.Net;

public abstract class XmppConnection : IDisposable
{
    protected internal readonly struct SendQueueEntry
    {
        public byte[] Bytes { get; init; }
        public string? DebugXml { get; init; }
        public TaskCompletionSource Completion { get; init; }
        public CancellationToken Token { get; init; }
    }

    private ConcurrentQueue<SendQueueEntry> _sendQueue = new();
    private ConcurrentDictionary<string, TaskCompletionSource<Stanza>> _callbacks = new();

    protected internal Stream? _stream;
    protected internal XmppParser? _parser;
    protected volatile StreamState _streamState;
    private readonly List<BaseExtension> _modules = [];

    public void RegisterExtension(BaseExtension module)
    {
        ArgumentNullException.ThrowIfNull(module);

        lock (_modules)
        {
            try
            {
                if (IsSessionStarted)
                    module.Setup(this);

                _modules.Add(module);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to initialize module: {module.GetType().FullName}", ex);
            }
        }
    }

    public void UnregisterExtension(BaseExtension module)
    {
        ArgumentNullException.ThrowIfNull(module);

        lock (_modules)
            _modules.Remove(module);

        try
        {
            module.Dispose();
        }
        catch (Exception ex)
        {
            FireOnError(new InvalidOperationException($"Failed to dispose module: {module.GetType().FullName}", ex));
        }
    }

    [Flags]
    protected enum StreamState
    {
        Read = 0b01,
        Write = 0b10,
        ReadWrite = Read | Write
    }

    volatile byte _disposed = 2;

    public string StreamId { get; protected set; }
    public virtual Jid Jid { get; protected internal set; }
    public bool IsAuthenticated { get; protected internal set; }
    public bool IsSessionStarted { get; protected internal set; }
    public bool IsConnected { get; private set; }

    public event Action OnConnected;
    public event Action OnSessionStarted;
    public event Action OnDisconnected;

    protected void FireOnConnected() => OnConnected?.Invoke();

    protected void FireOnSessionStarted()
    {
        try
        {
            OnSessionStarted?.Invoke();
        }
        catch (Exception ex)
        {
            FireOnError(ex);
        }

        lock (_modules)
        {
            var exceptions = new List<Exception>();

            foreach (var module in _modules)
            {
                if (module.Initialized)
                    continue;

                try
                {
                    module.Setup(this);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            if (exceptions.Count > 0)
                FireOnError(new AggregateException("One or more modules failed to setup.", exceptions));
        }
    }

    protected void FireOnDisconnected() => OnDisconnected?.Invoke();

    public event Action<Exception>? OnError;
    public event Action<Stanza>? OnStanza;
    public event Action<string>? OnReadXml, OnWriteXml;

    protected void InitParser()
    {
        IsConnected = true;

        _disposed = 0;

        _parser = new XmppParser();

        _parser.OnStreamStart += e =>
        {
            OnReadXml?.Invoke(e.ToString());

            try
            {
                HandleStreamStart(e);
            }
            catch (Exception ex)
            {
                OnError?.Invoke(ex);
                Disconnect();
            }
        };

        _parser.OnStreamElement += e =>
        {
            OnReadXml?.Invoke(e.ToString(true));

            try
            {
                if (e is Stanza stz && !string.IsNullOrEmpty(stz.Id) && _callbacks.TryRemove(stz.Id, out var tcs))
                {
                    tcs.TrySetResult(stz);
                    return;
                }

                if (e is StreamError se)
                    throw new JabberException($"Stream error received (code: {se.Condition}). {se.Text}");

                HandleStreamElement(e);
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

            try
            {
                HandleStreamEnd();
            }
            catch (Exception ex)
            {
                OnError?.Invoke(ex);
            }
            finally
            {
                Dispose();
            }
        };

        _streamState = StreamState.ReadWrite;

        InitConnection();

        _ = Task.WhenAny(ReadLoop(), WriteLoop())
            .ContinueWith(task =>
            {
                if (task.IsFaulted)
                    FireOnError(task.Exception!);

                Disconnect();
            });
    }

    protected virtual void InitConnection()
    {

    }

    protected internal void FireOnError(Exception ex) => OnError?.Invoke(ex);
    protected void FireOnStanza(Stanza stz) => OnStanza?.Invoke(stz);

    public async Task<Stanza> RequestStanza(Stanza stz, TimeSpan timeout = default, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(stz);

        if (string.IsNullOrEmpty(stz.Id))
            stz.GenerateId();

        using (var cts = CancellationTokenSource.CreateLinkedTokenSource(token))
        {
            if (timeout > TimeSpan.Zero)
                cts.CancelAfter(timeout);

            var tcs = new TaskCompletionSource<Stanza>();
            _callbacks[stz.Id!] = tcs;
            Send(stz);

            return await tcs.Task;
        }
    }

    protected virtual async Task ReadLoop()
    {
        var buffer = ArrayPool<byte>.Shared.Rent(1024);

        try
        {
            while (_disposed < 1)
            {
                await Task.Delay(16);

                if (!_streamState.HasFlag(StreamState.Read))
                    continue;

                int len = await _stream!.ReadAsync(buffer, CancellationToken.None);

                if (len <= 0)
                    break;

                _parser!.Parse(buffer, len);
            }
        }
        catch (Exception ex)
        {
            OnError?.Invoke(ex);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }

        Disconnect();
    }

    protected virtual async Task WriteLoop()
    {
        try
        {
            while (_disposed < 2)
            {
                await Task.Delay(16);

                if (!_streamState.HasFlag(StreamState.Write))
                    continue;

                if (_sendQueue?.TryDequeue(out var entry) == true)
                {
                    try
                    {
                        if (entry.Bytes?.Length > 0)
                            await _stream!.WriteAsync(entry.Bytes, entry.Token);

                        if (entry.DebugXml != null)
                            OnWriteXml?.Invoke(entry.DebugXml);

                        entry.Completion?.TrySetResult();
                    }
                    catch (Exception ex)
                    {
                        entry.Completion?.TrySetException(ex);
                        throw;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            OnError?.Invoke(ex);
        }
        finally
        {
            while (_sendQueue?.TryDequeue(out var entry) == true)
                entry.Completion?.TrySetCanceled();

            Dispose();
        }
    }

    protected internal void AddToSendQueue(SendQueueEntry entry)
    {
        if (_disposed > 1)
            return;

        _sendQueue.Enqueue(entry);
    }

    protected internal void Send(string xml)
    {
        if (_disposed > 1)
            return;

        AddToSendQueue(new()
        {
            Bytes = xml.GetBytes(),
            DebugXml = OnWriteXml != null ? xml : null
        });
    }

    public void Send(XmppElement element)
    {
        if (_disposed > 1)
            return;

        AddToSendQueue(new()
        {
            Bytes = element.ToString(false).GetBytes(),
            DebugXml = OnWriteXml != null ? element.ToString(true) : null
        });
    }

    public async Task SendAsync(XmppElement e)
    {
        await Task.Yield();

        if (_disposed > 1)
            return;

        var tcs = new TaskCompletionSource();

        AddToSendQueue(new()
        {
            Bytes = e.ToString(false).GetBytes(),
            DebugXml = OnWriteXml != null ? e.ToString(true) : null,
            Completion = tcs
        });

        await tcs.Task;
    }

    public void Disconnect(XmppElement? element = default)
    {
        if (_disposed > 0)
            return;

        var xml = new StringBuilder();

        if (element != null)
            xml.Append(element);

        Send(xml.Append(Xml.XmppStreamEnd).ToString());
    }

    protected virtual void Disposing()
    {

    }

    public void Dispose()
    {
        if (_disposed > 0)
            return;

        _disposed = 1;
        _streamState &= ~StreamState.Read;
        IsConnected = false;

        lock (_modules)
        {
            var exceptions = new List<Exception>();

            foreach (var module in _modules)
            {
                try
                {
                    module.Dispose();
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            if (exceptions.Count > 0)
                FireOnError(new AggregateException("One or more modules failed to dispose.", exceptions));

            _modules.Clear();
        }

        Disposing();

        if (_callbacks != null)
        {
            foreach (var (_, tcs) in _callbacks)
                tcs.TrySetCanceled();

            _callbacks.Clear();
            _callbacks = null!;
        }

        _parser?.Dispose();
        _parser = null;

        _ = Task.Run(async () =>
        {
            if (_streamState.HasFlag(StreamState.Write))
            {
                var tcs = new TaskCompletionSource();
                _sendQueue.Enqueue(new() { Completion = tcs });
                await Task.WhenAny(Task.Delay(5000), tcs.Task);
                tcs.TrySetResult();
            }

            if (_sendQueue != null)
            {
                while (_sendQueue.TryDequeue(out var entry))
                    entry.Completion?.TrySetCanceled();

                _sendQueue?.Clear();
                _sendQueue = null!;
            }

            _disposed = 2;
            _streamState &= ~StreamState.Write;

            try
            {
                _stream?.Dispose();
                _stream = null;
            }
            catch { }

            FireOnDisconnected();
        });

        GC.SuppressFinalize(this);
    }

    protected virtual void HandleStreamStart(StreamStream e)
    {

    }

    protected virtual void HandleStreamElement(XmppElement e)
    {

    }

    protected virtual void HandleStreamEnd()
    {

    }
}