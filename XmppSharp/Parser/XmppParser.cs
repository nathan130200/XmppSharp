using System.Runtime.CompilerServices;
using XmppSharp.Dom;
using XmppSharp.Protocol.Base;

namespace XmppSharp.Parser;

public abstract class XmppParser : IDisposable
{
    private volatile bool _disposed;

    protected bool Disposed => _disposed;

    [MethodImpl(MethodImplOptions.NoInlining)]
    protected void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(GetType().FullName);
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
            DisposeCore();
        }

        GC.SuppressFinalize(this);
    }

    protected virtual void DisposeCore()
    {

    }

    public event Action<StreamStream>? OnStreamStart;
    public event Action<Element>? OnStreamElement;
    public event Action? OnStreamEnd;

    protected virtual void FireOnStreamStart(StreamStream element)
        => OnStreamStart?.Invoke(element);

    protected virtual void FireOnStreamElement(Element element)
        => OnStreamElement?.Invoke(element);

    protected virtual void FireOnStreamEnd()
        => OnStreamEnd?.Invoke();
}
