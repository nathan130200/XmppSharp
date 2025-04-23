namespace XmppSharp.Net.Extensions.Abstractions;

using XmppSharp.Protocol.Client;
using XmppSharp.Protocol.Component;

/// <summary>
/// Represents the base class of a connection module.
/// </summary>
public abstract class BaseExtension : IDisposable
{
    private volatile bool _initialized;
    private XmppConnection _connection;

    /// <summary>
    /// Connection this module belongs to.
    /// </summary>
    protected XmppConnection Connection => _connection;

    /// <summary>
    /// Determines whether this module is properly initialized.
    /// </summary>
    protected internal bool Initialized => _initialized;

    internal void Setup(XmppConnection connection)
    {
        if (_initialized)
            throw new InvalidOperationException($"Module '{GetType().FullName}' already initialized.");

        _initialized = true;
        _connection = connection;
        Setup();
    }

    /// <summary>
    /// This function is invoked when the connection is considered online, that is, 
    /// on the <see cref="XmppClientConnection"/> after <see cref="Bind" /> query, on the 
    /// <see cref="XmppComponentConnection" /> after <see cref="Handshake" /> element.
    /// </summary>
    protected internal virtual void Setup()
    {

    }

    /// <summary>
    /// Function available for the module to clean up previously allocated resources.
    /// <para>
    /// Before being disposed, access to the connection is still available (that is, it is not yet null)
    /// </para>
    /// </summary>
    protected virtual void Disposing()
    {

    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (!_initialized)
            return;

        _initialized = false;
        Disposing();
        _connection = null!;
        GC.SuppressFinalize(this);
    }
}