using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using XmppSharp.Net;
using XmppSharp.Sasl.Impl;

namespace XmppSharp.Sasl;

/// <summary>
/// Provides a factory for registering and creating SASL handlers for XMPP client connections.
/// </summary>
public static class XmppSaslHandlerFactory
{
    static readonly ConcurrentDictionary<string, Func<OutgoingXmppClientConnection, XmppSaslHandler>> s_Mechanisms = new(StringComparer.OrdinalIgnoreCase);

    static XmppSaslHandlerFactory()
    {
        s_Mechanisms["PLAIN"] = c => new XmppSaslPlainHandler(c);
    }

    /// <summary>
    /// Registers a new mechanism.
    /// </summary>
    /// <param name="mechanismName">Mechanism name.</param>
    /// <param name="factory">Factory function to build the SASL handler.</param>
    public static void RegisterMechanism(string mechanismName, Func<OutgoingXmppClientConnection, XmppSaslHandler> factory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(mechanismName);
        ArgumentNullException.ThrowIfNull(factory);
        s_Mechanisms[mechanismName] = factory;
    }

    /// <summary>
    /// Try to create a SASL handler.
    /// </summary>
    /// <param name="mechanismName">Mechanism name.</param>
    /// <param name="connection">Connection that will authenticate.</param>
    /// <param name="handler">Result of SASL handler instance or <see langword="null" /> if the mechanism was not registered.</param>
    /// <returns><see langword="true" /> if the mechanism was registered and the handler was instantiated; otherwise <see langword="false" />.</returns>
    public static bool TryCreate(string mechanismName, OutgoingXmppClientConnection connection, [NotNullWhen(true)] out XmppSaslHandler? handler)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(mechanismName);
        ArgumentNullException.ThrowIfNull(connection);

        handler = default;

        if (connection == null)
            return false;

        if (s_Mechanisms.TryGetValue(mechanismName, out var result))
            handler = result(connection);

        return handler != null;
    }
}
