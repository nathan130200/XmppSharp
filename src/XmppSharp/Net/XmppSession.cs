using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml;
using Jabber.Entities;
using Jabber.Enums;
using Jabber.Exceptions;
using Jabber.Protocol;
using Jabber.Xmpp;

namespace Jabber.Net;

public class XmppSession : IDisposable
{
    private Stream? _stream;
    private XmppServer? _server;
    private volatile XmppSessionState _sessionState;
    private volatile StreamState _streamState;
    private CancellationTokenSource _cts;
    private ConcurrentQueue<(byte[] Buffer, Action<Exception?> Callback)>? _queue = [];
    private XmppParser _parser;
    private volatile bool _disposed;

    public string Id { get; }
    public IPAddress RemoteAddress { get; }
    public Jid Jid { get; private set; }

    public XmppSessionState State => _sessionState;

    public XmppSession(XmppServer server, Socket socket)
    {
        _server = server;
        _stream = new NetworkStream(socket, true);
        _sessionState = XmppSessionState.None;

        Id = _server.GenerateSessionId();
        Jid = new Jid { User = "unknown", Server = _server._config.Hostname };
        RemoteAddress = (socket.RemoteEndPoint as IPEndPoint).Address;

        Debug.Assert(RemoteAddress != null);
    }

    static readonly string DefaultEndTag = "</stream:stream>";

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        _sessionState = XmppSessionState.None;
        _streamState = StreamState.None;

        if (_queue != null)
        {
            if (!_queue.IsEmpty)
            {
                var exception = new ObjectDisposedException(GetType().FullName);

                while (_queue.TryDequeue(out var item))
                    item.Callback?.Invoke(exception);
            }

            _queue = null;
        }

        _stream?.Dispose();
        _stream = null;

        _server?.UnregisterSession(this);
        _server = null;

        GC.SuppressFinalize(this);
    }

    #region Initialization Methods

    public async Task InitializeAsync(CancellationToken token = default)
    {
        await Task.Yield();

        _cts = CancellationTokenSource.CreateLinkedTokenSource(token);
        _streamState = StreamState.All;
        await _server.FireOnClientConnected(this);

        _ = Task.Run(BeginReceive, token);
        _ = Task.Run(BeginSend, token);
    }

    internal void InitParser(bool leaveSocketOpen = true)
    {
        if (_parser != null)
        {
            _parser.OnStreamStart -= OnXmppStartTag;
            _parser.OnStreamElement -= OnXmppElement;
            _parser.OnStreamEnd -= OnXmppEndTag;
            _parser.Dispose();
        }

        if (_disposed)
            return;

        _parser = new XmppParser(_stream!, Encoding.UTF8, _server._config.ParserBufferSize, leaveSocketOpen);
        _parser.OnStreamStart += OnXmppStartTag;
        _parser.OnStreamElement += OnXmppElement;
        _parser.OnStreamEnd += OnXmppEndTag;
    }

    #endregion

    public async Task DisconnectAsync(StreamErrorCondition? reason = default, string? description = default)
    {
        using (var scope = StringBuilderPool.Rent())
        {
            var sb = scope.Value;

            if (reason.TryUnwrap(out var v))
                sb.Append(v.CreateElement(description).ToXml());

            sb.Append(DefaultEndTag);

            await SendAsync(sb.ToString());
        }

        CheckDisposed();
    }

    async Task OnXmppEndTag()
    {
        LogXml(this, StreamState.Read, DefaultEndTag);
        await DisconnectAsync();
    }

    #region Read Thread

    async Task BeginReceive()
    {
        StreamErrorCondition? errorType = default;
        string? errorMsg = default;

        try
        {
            InitParser();

            while (_cts != null && !_cts.IsCancellationRequested)
            {
                await Task.Delay(1);

                if (_disposed)
                    return;

                if (!_streamState.HasFlag(StreamState.Read))
                    continue;

                await _parser.UpdateAsync();
            }
        }
        catch (JabberStreamException ex)
        {
            errorType = ex.Condition;
            errorMsg = ex.Message;
        }
        catch (XmlException)
        {
            errorType = StreamErrorCondition.NotWellFormed;
        }
        catch (Exception)
        {
            errorType = StreamErrorCondition.InternalServerError;
        }

        await DisconnectAsync(errorType, errorMsg);
    }

    #endregion

    #region Write Thread

    async Task BeginSend()
    {
        while (_cts != null && !_cts.IsCancellationRequested)
        {
            await Task.Delay(1);

            if (_disposed)
                return;

            if (!_streamState.HasFlag(StreamState.Write))
                continue;

            if (!_queue.IsEmpty)
            {
                while (_queue != null && _queue.TryDequeue(out var entry))
                {
                    var (buffer, callback) = entry;

                    Exception? exception = default;

                    try
                    {
                        await _stream.WriteAsync(buffer, _cts.Token);
                    }
                    catch (Exception e)
                    {
                        exception = e;
                    }
                    finally
                    {
                        callback?.Invoke(exception);
                    }
                }
            }
        }

        CheckDisposed();
    }

    #endregion

    void CheckDisposed()
    {
        if (!_disposed)
        {
            if (_server != null)
                _ = _server.FireOnClientDisconnected(this);

            Dispose();
        }
    }

    async Task OnXmppStartTag(XmlElement e)
    {
        LogXml(this, StreamState.Read, e.StartTag());

        StreamErrorCondition? condition = null;
        string targetHostname = _server._config.Hostname;

        if (!e.HasAttribute("to"))
            condition = StreamErrorCondition.ImproperAddressing;
        else if (targetHostname != e.GetAttribute("to"))
            condition = StreamErrorCondition.HostUnknown;

        if (e.GetNamespaceOfPrefix(string.Empty) != Namespace.Client)
            condition = StreamErrorCondition.InvalidNamespace;

        e.RemoveAttribute("to");
        e.SetAttribute("from", targetHostname);
        e.SetAttribute("id", Id);
        await SendAsync(e.StartTag());

        if (condition.TryUnwrap(out var v))
        {
            await DisconnectAsync(v);
            return;
        }

        var features = Namespace.Stream.CreateElement("stream:features");
        {
            if (!_sessionState.HasFlag(XmppSessionState.Authenticated))
            {
                if (!_sessionState.HasFlag(XmppSessionState.TlsStarted)
                    && _server._config.TlsPolicy > TlsPolicy.None)
                {
                    var tls = features.C("starttls", Namespace.Tls);

                    if (_server._config.TlsPolicy == TlsPolicy.Required)
                        tls.C("required");
                }

                var m = features.C("mechanisms", Namespace.Sasl);
                {
                    m.C("mechanism").T("PLAIN");
                    //m.C("mechanism").T("WARFACE");
                }
            }
            else
            {
                if (!_sessionState.HasFlag(XmppSessionState.ResourceBinded))
                    features.C("bind", Namespace.Bind);

                if (!_sessionState.HasFlag(XmppSessionState.SessionStarted))
                    features.C("session", Namespace.Session);
            }
        }

        await SendAsync(features);
    }

    async Task OnXmppElement(XmlElement e)
    {
        await Task.Yield();

        LogXml(this, StreamState.Read, e);

        // TODO: 
    }

    #region Xml Logging

    static void LogXml(XmppSession session, StreamState state, object param, Exception? exception = default)
    {
        if (state == StreamState.Write)
            session.FireOnWriteXml(param, exception);
        else
            session.FireOnReadXml(param);
    }

    #endregion 

    #region Async Send Methods

    internal Task SendAsync(string xml)
    {
        if (_disposed)
            return Task.CompletedTask;

        var tcs = new TaskCompletionSource();

        var callback = (Exception? ex) =>
        {
            _ = ex == null
                ? tcs.TrySetResult()
                : tcs.TrySetException(ex);

            LogXml(this, StreamState.Write, xml);
        };

        var buffer = xml.GetBytes();
        _queue.Enqueue((buffer, callback));
        return tcs.Task;
    }

    public Task SendAsync(XmlElement element)
    {
        if (_disposed)
            return Task.CompletedTask;

        var tcs = new TaskCompletionSource();

        var callback = (Exception? ex) =>
        {
            _ = ex == null
                ? tcs.TrySetResult()
                : tcs.TrySetException(ex);

            LogXml(this, StreamState.Write, element, ex);
        };

        var buffer = element.ToXml().GetBytes();
        _queue.Enqueue((buffer, callback));

        return tcs.Task;
    }

    #endregion

    #region Sync Send Methods

    internal void Send(string xml)
    {
        if (_disposed)
            return;

        var callback = (Exception? ex) =>
        {
            LogXml(this, StreamState.Write, xml, ex);
        };

        var buffer = xml.GetBytes();
        _queue.Enqueue((buffer, callback));
    }

    internal void Send(XmlElement element)
    {
        if (_disposed)
            return;

        var callback = (Exception? ex) =>
        {
            LogXml(this, StreamState.Write, element, ex);
        };

        var buffer = element.ToXml().GetBytes();
        _queue.Enqueue((buffer, callback));
    }

    #endregion
}