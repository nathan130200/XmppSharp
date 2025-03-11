using System.Runtime.CompilerServices;
using XmppSharp.Dom;
using XmppSharp.Entities.Options;
using XmppSharp.Protocol.Base;

namespace XmppSharp.Helpers;

/// <summary>
/// Base class for reading XMPP elements.
/// </summary>
/// <remarks>
/// The goal is to replace the need to use parser-specific method to read the XML.
/// </remarks>
public abstract class XmppStreamReader
{
    protected XmppConnectionOptions Options { get; }

    public XmppStreamReader(XmppConnectionOptions options)
    {
        Options = options;
    }

    private volatile bool _disposed;

    protected bool IsDisposed => _disposed;

    public event Action<StreamStream>? OnStreamStart;
    public event Action<XmppElement>? OnStreamElement;
    public event Action? OnStreamEnd;
    public event Action<Exception>? OnError;
    public event Action? OnDisposed;

    protected virtual void FireOnStreamStart(StreamStream e)
        => OnStreamStart?.Invoke(e);

    protected virtual void FireOnStreamElement(XmppElement e)
        => OnStreamElement?.Invoke(e);

    protected virtual void FireOnStreamEnd()
        => OnStreamEnd?.Invoke();

    protected virtual void FireOnError(Exception ex)
        => OnError?.Invoke(ex);

    public virtual TaskAwaiter GetAwaiter()
    {
        return Task.CompletedTask.GetAwaiter();
    }


    public virtual void Pause()
    {

    }

    public virtual void Reset(Stream stream, CancellationToken token)
    {

    }

    protected virtual void Disposing()
    {

    }

    protected void ThrowIfDisposed()
        => Throw.IfDisposed(this, _disposed);

    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
            Disposing();
            OnDisposed?.Invoke();
        }
    }
}