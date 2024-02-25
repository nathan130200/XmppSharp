using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using XmppSharp.Entities;
using XmppSharp.Protocol;

namespace XmppSharp.Net;

public class XmppServer
{
    internal readonly XmppServerConfiguration _config;
    private Socket? _socket;
    private CancellationTokenSource? _cts;
    private readonly List<XmppSession> _sessions = [];
    private readonly IPEndPoint _endpoint;

    public event ParameterizedAsyncEventHandler<XmppSession> OnClientConnected = default!;
    public event ParameterizedAsyncEventHandler<XmppSession> OnClientDisconnected = default!;

    public XmppServer(XmppServerConfiguration? config = default)
    {
        _config = config ?? new();

        if (!IPAddress.TryParse(_config.EndPoint.Host, out var address))
            address = IPAddress.Loopback;

        _endpoint = new IPEndPoint(address, _config.EndPoint.Port);
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    }

    internal Task FireOnClientConnected(XmppSession s)
        => OnClientConnected.InvokeAsync(s);

    internal Task FireOnClientDisconnected(XmppSession s)
        => OnClientDisconnected.InvokeAsync(s);

    public async Task StartAsync(CancellationToken token = default)
    {
        await Task.Yield();

        Debug.Assert(_socket != null);

        _socket.Bind(_endpoint);
        _socket.Listen(_config.ListenBacklog);
        _cts = CancellationTokenSource.CreateLinkedTokenSource(token);

        _ = Task.Run(BeginAccept, token);
    }

    public async Task StopAsync()
    {
        await Task.Yield();

        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;

        if (_sessions != null)
        {
            var task = Parallel.ForEachAsync(GetSessions(), async (session, _) =>
            {
                try
                {
                    await session.DisconnectAsync(StreamErrorCondition.SystemShutdown);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            });

            lock (_sessions)
                _sessions.Clear();

            await Task.WhenAny(Task.Delay(_config.GracefullyDisconnectTimeout), task);
        }

        _socket?.Dispose();
        _socket = null;
    }

    async Task BeginAccept()
    {
        _sessions.Clear();

        while (_cts != null && !_cts.IsCancellationRequested)
        {
            await Task.Delay(1);

            try
            {
                var client = await _socket!.AcceptAsync(_cts.Token);
                _ = Task.Run(async () => await EndAccept(client));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }
    }

    async Task EndAccept(Socket socket)
    {
        var session = new XmppSession(this, socket);

        lock (_sessions)
            _sessions.Add(session);

        await session.InitializeAsync(_cts!.Token);
    }

    internal bool UnregisterSession(XmppSession session)
    {
        lock (_sessions)
            return _sessions.Remove(session);
    }

    internal string GenerateSessionId()
    {
        string result;

        while (true)
        {
            result = Random.Shared.Next().ToString("X4");

            if (GetSession(x => x.Id == result) == null)
                break;
        }

        return result;
    }

    public IEnumerable<XmppSession> GetSessions()
    {
        if (_sessions is null)
            return [];

        lock (_sessions)
        {
            XmppSession[] result;

            lock (_sessions)
                result = _sessions.ToArray();

            return result;
        }
    }

    public IEnumerable<XmppSession> GetSessions(Func<XmppSession, bool> predicate)
        => GetSessions().Where(predicate);

    public XmppSession? GetSession(Func<XmppSession, bool> predicate)
        => GetSessions().FirstOrDefault(predicate);
}
