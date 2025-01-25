using System.Runtime.CompilerServices;
using XmppSharp.Dom;
using XmppSharp.Protocol.Base;

namespace XmppSharp.Parser;

/// <summary>
/// Base class for XML processing for jabber protocol.
/// </summary>
public abstract class XmppParser : IDisposable
{
    /// <summary>
    /// Determines whether the parser has been disposed.
    /// </summary>
    protected volatile bool _disposed;

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
            Disposing();
        }

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Helper function to free resources allocated by the parser implementation.
    /// </summary>
    protected virtual void Disposing()
    {

    }

    /// <summary>
    /// Event fired when the XMPP open tag is parsed in the XML stream.
    /// </summary>
    public event Action<StreamStream>? OnStreamStart;

    /// <summary>
    /// Event fired for any other element.
    /// </summary>
    public event Action<Element>? OnStreamElement;

    /// <summary>
    /// Event fired when the XMPP close tag is parsed in the XML stream.
    /// </summary>
    public event Action? OnStreamEnd;

    /// <summary>
    /// Fires the <see cref="OnStreamStart" /> event.
    /// </summary>
    /// <param name="element"></param>
    protected virtual void FireOnStreamStart(StreamStream element)
        => OnStreamStart?.Invoke(element);

    /// <summary>
    /// Fires the <see cref="OnStreamElement" /> event.
    /// </summary>
    /// <param name="element"></param>
    protected virtual void FireOnStreamElement(Element element)
        => OnStreamElement?.Invoke(element);

    /// <summary>
    /// Fires the <see cref="OnStreamEnd" /> event.
    /// </summary>
    protected virtual void FireOnStreamEnd()
        => OnStreamEnd?.Invoke();
}
