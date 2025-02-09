using System.Buffers;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using Expat;
using XmppSharp.Dom;
using XmppSharp.Parser;
using XmppSharp.Protocol.Base;
using XmppSharp.Protocol.Core;

namespace XmppSharp.Net;

public class XmppConnection : IDisposable
{
    public event Action<XmppConnection, string> OnReadXml;
    public event Action<XmppConnection, string> OnWriteXml;

    public XmppConnectionOptions Options => _options;

    internal XmppConnectionOptions _options;
    internal ConcurrentQueue<(byte[]? Buffer, TaskCompletionSource? Completion, string? DebugXml)> _sendQueue = new();
    protected ConcurrentDictionary<string, TaskCompletionSource<Stanza>> Callbacks { get; private set; } = new();

    protected Socket? Socket { get; private set; }
    protected Stream? Stream { get; set; }
    protected ExpatXmppParser? Parser { get; private set; }

    public XmppConnectionState State { get; protected set; }
    internal SemaphoreSlim? _semaphore = new(1, 1);
    internal volatile FileAccess _access;
    internal volatile byte _disposed;

    protected XmppConnection(XmppConnectionOptions options)
    {
        _options = options;
    }

    public async Task ConnectAsync()
    {
        _options.Validate();

        try
        {
            Socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            await Socket.ConnectAsync(_options.EndPoint);

            Stream = new NetworkStream(Socket, false);

            State |= XmppConnectionState.Connected;
            _access = FileAccess.ReadWrite;

            InitParser();
            SendStreamHeader();

            await Task.WhenAll(BeginReceive(), BeginSend());
        }
        catch
        {
            Dispose();
            throw;
        }
    }

    public void Send(string xml)
    {
        if (_disposed == 2)
            return;

        var buffer = xml.GetBytes();
        _sendQueue.Enqueue((buffer, null, _options.Verbose ? xml : null));
    }

    public void Send(XmppElement element)
    {
        if (_disposed == 2)
            return;

        var buffer = element.ToString(false).GetBytes();
        _sendQueue.Enqueue((buffer, null, _options.Verbose ? element.ToString(true) : null));
    }

    public Task SendAsync(string xml)
    {
        if (_disposed == 2)
            return Task.CompletedTask;

        var tcs = new TaskCompletionSource();
        var buffer = xml.GetBytes();
        _sendQueue.Enqueue((buffer, tcs, _options.Verbose ? xml : null));
        return tcs.Task;
    }

    public Task SendAsync(XmppElement element)
    {
        if (_disposed == 2)
            return Task.CompletedTask;

        var tcs = new TaskCompletionSource();
        var buffer = element.ToString(false).GetBytes();
        _sendQueue.Enqueue((buffer, tcs, _options.Verbose ? element.ToString(true) : null));
        return tcs.Task;
    }

    public async Task<Stanza> SendStanzaAsync(Stanza stz, TimeSpan timeout = default, CancellationToken token = default)
    {
        Throw.IfNull(stz);

        if (string.IsNullOrWhiteSpace(stz.Id))
            stz.GenerateId();

        var tcs = new TaskCompletionSource<Stanza>();

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(token);
        cts.Token.Register(() => tcs.TrySetCanceled());

        if (timeout > TimeSpan.Zero)
            cts.CancelAfter(timeout);

        Callbacks[stz.Id!] = tcs;

        Send(stz);

        return await tcs.Task;
    }

    public void Disconnect(XmppElement? element = default)
    {
        if (_disposed > 0)
            return;

        var xml = new StringBuilder();

        if (element != null)
            xml.Append(element.ToString(false));

        Send(xml.Append(Xml.XmppStreamEnd).ToString());

        Dispose();
    }

    protected void FireOnReadXml(string xml) => OnReadXml?.Invoke(this, xml);
    protected void FireOnWriteXml(string xml) => OnWriteXml?.Invoke(this, xml);

    protected virtual void InitParser()
    {
        // FIXME: Due weird reason, soft-parser reset does not work in expat if server re-send XML prolog.
        // An work-around to fix this, just delete old parser and create new one.
        _access &= ~FileAccess.Read;

        var oldParser = Parser;

        Parser = new ExpatXmppParser(ExpatEncoding.UTF8);

        Parser.OnStreamStart += e =>
        {
            if (_options.Verbose)
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

        Parser.OnStreamElement += element =>
        {
            if (_options.Verbose)
                FireOnReadXml(element.ToString(true));

            if (element is StreamError se)
                throw new JabberStreamException(se.Condition ?? StreamErrorCondition.UndefinedCondition);

            try
            {
                HandleStreamElement(element);
            }
            catch (Exception ex)
            {
                FireOnError(ex);
                Disconnect();
            }
        };

        Parser.OnStreamEnd += () =>
        {
            Send(Xml.XmppStreamEnd);

            if (_options.Verbose)
                FireOnReadXml(Xml.XmppStreamEnd);

            try
            {
                HandleStreamEnd();
            }
            catch (Exception ex)
            {
                FireOnError(ex);
            }

            Dispose();
        };

        oldParser?.Dispose();

        _access |= FileAccess.Read;
    }

    async Task BeginReceive()
    {
        int numBytes = _options.ReceiveBufferSize;

        if (numBytes <= 0)
            numBytes = 4096;

        var buffer = ArrayPool<byte>.Shared.Rent(numBytes);

        try
        {
            while (_disposed < 1)
            {
                await Task.Delay(1);

                if (!_access.HasFlag(FileAccess.Read))
                    continue;

                Throw.IfNull(Stream);
                Throw.IfNull(Parser);

                numBytes = await Stream!.ReadAsync(buffer);

                if (numBytes <= 0)
                    break;

                Parser?.Parse(buffer, numBytes, numBytes <= 0);
            }
        }
        catch (Exception ex)
        {
            FireOnError(ex);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }

        Dispose();
    }

    async Task BeginSend()
    {
        while (_disposed < 2)
        {
            await Task.Delay(1);

            if (!_access.HasFlag(FileAccess.Write))
                continue;

            try
            {
                await _semaphore!.WaitAsync();

                while (_sendQueue.TryDequeue(out var tuple))
                {
                    var (buffer, tcs, xml) = tuple;

                    try
                    {
                        if (buffer?.Length > 0)
                            await Stream!.WriteAsync(buffer);

                        if (xml != null && _options.Verbose)
                            FireOnWriteXml(xml);
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
            catch (Exception ex)
            {
                FireOnError(ex);
                break;
            }
            finally
            {
                _semaphore?.Release();
            }
        }

        Dispose();
    }

    protected void FireOnError(Exception ex) => OnError?.Invoke(this, ex);
    protected void FireOnMessage(Message e) => OnMessage?.Invoke(this, e);
    protected void FireOnPresence(Presence e) => OnPresence?.Invoke(this, e);
    protected void FireOnIq(Iq e) => OnIq?.Invoke(this, e);

    protected virtual void Disposing()
    {

    }

    protected virtual void SendStreamHeader()
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        if (_disposed > 0)
            return;

        _disposed = 1;
        GC.SuppressFinalize(this);

        _access &= ~FileAccess.Read;

        var isConnected = State.HasFlag(XmppConnectionState.Connected);
        State &= ~XmppConnectionState.Connected;

        if (Callbacks != null)
        {
            foreach (var (_, tcs) in Callbacks)
                tcs?.TrySetCanceled();

            Callbacks.Clear();
            Callbacks = null!;
        }

        if (isConnected)
            Socket?.Shutdown(SocketShutdown.Receive);

        Parser?.Dispose();

        Disposing();

        _semaphore?.Dispose();
        _semaphore = null;

        Parser = null;

        if (_sendQueue.IsEmpty || Options.DisconnectTimeout <= TimeSpan.Zero)
            CleanupSocket(isConnected);
        else
        {
            _ = Task.Delay(Options.DisconnectTimeout)
                .ContinueWith(_ => CleanupSocket(isConnected));
        }
    }

    void CleanupSocket(bool isConnected)
    {
        _disposed = 2;
        _access &= ~FileAccess.Write;

        if (isConnected)
            Socket?.Shutdown(SocketShutdown.Send);

        State = 0;

        try
        {
            Stream?.Dispose();
            Stream = null;
        }
        catch { }

        try
        {
            Socket?.Dispose();
            Socket = null;
        }
        catch { }
    }

    public event Action<XmppConnection, Exception>? OnError;
    public event Action<XmppConnection, Presence>? OnPresence;
    public event Action<XmppConnection, Message>? OnMessage;
    public event Action<XmppConnection, Iq>? OnIq;

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
