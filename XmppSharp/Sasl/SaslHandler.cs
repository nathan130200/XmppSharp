using XmppSharp.Dom;
using XmppSharp.Exceptions;
using XmppSharp.Net;
using XmppSharp.Protocol.Sasl;

namespace XmppSharp.Sasl;

/// <summary>
/// Base class for processing SASL authentication.
/// </summary>
public abstract class SaslHandler : IDisposable
{
    internal XmppClientConnection _connection;
    volatile bool _disposed;

    /// <summary>
    /// Underlying connection.
    /// </summary>
    protected XmppClientConnection Connection => _connection;

    protected bool IsDisposed => _disposed;

    public SaslHandler(XmppClientConnection connection)
    {
        _connection = connection;
    }

    /// <summary>
    /// Free resources allocated by this handler.
    /// </summary>
    protected virtual void Disposing()
    {

    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        Disposing();
        _connection = null!;

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Initial step of the SASL handler. It should send a <see cref="Auth" /> element to the remote server with the requested mechanism.
    /// <para>
    /// During this step you can also allocate memory, use external APIs or native libraries.
    /// </para>
    /// </summary>
    protected internal virtual void Init()
    {

    }

    /// <summary>
    /// Invokes the SASL handler to process the element.
    /// </summary>
    /// <param name="e">Element that will be processed by the handler.</param>
    /// <returns><see langword="true"/> if the SASL handler has finished and authentication was successful; otherwise the handler will continue processing the next elements (in the case of authentication mechanisms that have more than one auth step).</returns>
    /// <exception cref="JabberSaslException">If authentication fails.</exception>
    /// <remarks>
    /// <para>Once the SASL Handler is declared as authenticated the connection will proceed normally.</para>
    /// </remarks>
    protected internal abstract bool Invoke(XmppElement e);
}