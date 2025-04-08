using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using XmppSharp.Net;

namespace XmppSharp.Sasl;

/// <summary>
/// Global class that manage SASL mechanisms.
/// </summary>
public static class SaslFactory
{
    static readonly ConcurrentDictionary<string, Func<XmppClientConnection, SaslHandler>> s_Mechanisms = new(StringComparer.OrdinalIgnoreCase);

    static SaslFactory()
    {
        s_Mechanisms["PLAIN"] = c => new SaslPlainHandler(c);
    }

    /// <summary>
    /// Registers a new mechanism.
    /// </summary>
    /// <param name="mechanismName">Mechanism name.</param>
    /// <param name="factory">Factory function to build the SASL handler.</param>
    public static void RegisterMechanism(string mechanismName, Func<XmppClientConnection, SaslHandler> factory)
    {
        Throw.IfNullOrWhiteSpace(mechanismName);
        Throw.IfNull(factory);
        s_Mechanisms[mechanismName] = factory;
    }

    /// <summary>
    /// Try to create a SASL handler.
    /// </summary>
    /// <param name="mechanismName">Mechanism name.</param>
    /// <param name="connection">Connection that will authenticate.</param>
    /// <param name="handler">Result of SASL handler instance or <see langword="null" /> if the mechanism was not registered.</param>
    /// <returns><see langword="true" /> if the mechanism was registered and the handler was instantiated; otherwise <see langword="false" />.</returns>
    public static bool TryCreate(string mechanismName, XmppClientConnection connection, [NotNullWhen(true)] out SaslHandler? handler)
    {
        Throw.IfNullOrWhiteSpace(mechanismName);
        Throw.IfNull(connection);

        handler = default;

        if (connection == null)
            return false;

        if (s_Mechanisms.TryGetValue(mechanismName, out var result))
            handler = result(connection);

        return handler != null;
    }
}
