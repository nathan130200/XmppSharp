using System.Collections.Concurrent;
using XmppSharp.Protocol.Base;

namespace XmppSharp;

public sealed class StanzaGrabber<TStanza> : IDisposable
    where TStanza : Stanza
{
    private volatile bool _disposed;

    private ConcurrentDictionary<string, TaskCompletionSource<TStanza>>? _callbacks;
    private XmppConnection? _connection;

    public StanzaGrabber(XmppConnection connection)
    {
        _callbacks = new ConcurrentDictionary<string, TaskCompletionSource<TStanza>>();
        _connection = connection;
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        foreach (var tcs in _callbacks!.Values)
            tcs.TrySetCanceled();

        _callbacks.Clear();
        _callbacks = null;
        _connection = null;
    }

    void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(GetType().FullName);
    }

    public async Task<TStanza> RequestAsync(TStanza stz, TimeSpan timeout)
    {
        ThrowIfDisposed();

        var tcs = new TaskCompletionSource<TStanza>();

        if (string.IsNullOrWhiteSpace(stz.Id))
            stz.GenerateId(IdGenerator.Random);

        using (var cts = new CancellationTokenSource(timeout))
        {
            cts.Token.Register(() => tcs.TrySetCanceled());
            _callbacks![stz.Id!] = tcs;
            _connection?.Send(stz);
            return await tcs.Task;
        }
    }

    public Task<TStanza> RequestAsync(TStanza stz, CancellationToken token)
    {
        var tcs = new TaskCompletionSource<TStanza>();

        if (string.IsNullOrWhiteSpace(stz.Id))
            stz.GenerateId(IdGenerator.Random);

        token.Register(() => tcs.TrySetCanceled());
        _callbacks![stz.Id!] = tcs;
        _connection?.Send(stz);
        return tcs.Task;
    }
}
