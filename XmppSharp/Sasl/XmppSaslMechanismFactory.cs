using System.Collections.Concurrent;
using XmppSharp.Net;
using XmppSharp.Protocol.Core.Sasl;

namespace XmppSharp.Sasl;

public static class XmppSaslMechanismFactory
{
    static readonly ConcurrentDictionary<string, Func<XmppClientConnection, XmppSaslMechanismHandler>> s_SaslFactory = new(StringComparer.OrdinalIgnoreCase);

    static XmppSaslMechanismFactory()
    {
        s_SaslFactory["PLAIN"] = c => new XmppSaslPlainHandler(c);
    }

    public static void RegisterMechanism(string mechanismName, Func<XmppClientConnection, XmppSaslMechanismHandler> factory)
        => s_SaslFactory[mechanismName] = factory;

    public static XmppSaslMechanismHandler CreateNew(XmppClientConnection connection, string mechanism)
    {
        if (s_SaslFactory.TryGetValue(mechanism, out var result))
            return result(connection);

        throw new JabberSaslException(FailureCondition.InvalidMechanism, $"SASL mmechanism '{mechanism}' is not implemented");
    }
}
