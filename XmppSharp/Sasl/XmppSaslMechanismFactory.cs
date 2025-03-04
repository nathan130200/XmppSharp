using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using XmppSharp.Net;

namespace XmppSharp.Sasl;

public static class XmppSaslMechanismFactory
{
    static readonly ConcurrentDictionary<string, Func<XmppClientConnection, XmppSaslHandler>> s_SaslFactory = new(StringComparer.OrdinalIgnoreCase);

    static XmppSaslMechanismFactory()
    {
        s_SaslFactory["PLAIN"] = c => new XmppSaslPlainHandler(c);
    }

    public static void RegisterMechanism(string mechanismName, Func<XmppClientConnection, XmppSaslHandler> factory)
        => s_SaslFactory[mechanismName] = factory;

    public static bool TryCreate(string mechanism, XmppClientConnection connection, [NotNullWhen(true)] out XmppSaslHandler? handler)
    {
        handler = default;

        if (connection == null)
            return false;

        if (s_SaslFactory.TryGetValue(mechanism, out var result))
            handler = result(connection);

        return handler != null;
    }
}
