using XmppSharp.Dom;
using XmppSharp.Exceptions;
using XmppSharp.Net;
using XmppSharp.Protocol.Sasl;

namespace XmppSharp.Sasl;

/// <summary>
/// Base class for processing SASL authentication.
/// </summary>
public abstract class XmppSaslHandler : IDisposable
{
    internal XmppOutboundClientConnection _connection;
    volatile bool _disposed;

    /// <summary>
    /// Underlying connection.
    /// </summary>
    protected XmppOutboundClientConnection Connection => _connection;

    protected bool Disposed => _disposed;

    public XmppSaslHandler(XmppOutboundClientConnection connection)
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
    /// Initial step of the SASL handler. It usually should send a <see cref="Auth" /> element to the remote server with the requested mechanism.
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
    /// <returns>
    /// <list type="bullet">
    /// <item><see langword="true"/> if the SASL handler has finished and authentication was successful (received <see cref="Success"/> from server)</item>
    /// <item><see langword="false"/> if the SASL handler does not handle current element.</item>
    /// <item>throws <see cref="JabberSaslException"/> if authentication fails. its only way also to </item>
    /// </list>
    /// </returns>
    /// <exception cref="JabberSaslException"/> If authentication fails.
    /// <remarks>
    /// <para>Once the SASL Handler is declared as authenticated the connection will proceed normally.</para>
    /// </remarks>
    protected internal abstract SaslHandlerResult Invoke(XmppElement e);
}