using System.Collections.Concurrent;
using System.Text;
using XmppSharp.Dom;
using XmppSharp.Protocol.Core.Sasl;

namespace XmppSharp.Net;

public abstract class SaslHandler
{
    static ConcurrentDictionary<string, Type> s_HandlerTypes = new(StringComparer.OrdinalIgnoreCase);

    public static void RegisterSaslHandler(string mechanismName, Type handlerType)
    {
        Throw.IfStringNullOrWhiteSpace(mechanismName);
        Throw.IfNull(handlerType);

        if (handlerType.BaseType != typeof(SaslHandler))
            throw new InvalidOperationException($"SASL handler of type '{handlerType}' is not valid handler.");

        s_HandlerTypes[mechanismName] = handlerType;
    }

    static SaslHandler()
    {
        RegisterSaslHandler("PLAIN", typeof(SaslPlainHandler));
    }

    public static SaslHandler CreateHandler(XmppClientConnection connection, string mechanismName)
    {
        var handlerType = s_HandlerTypes[mechanismName];

        if (Activator.CreateInstance(handlerType, true) is not SaslHandler handler)
            throw new ArgumentException($"Unable to create SASL handler for mechanism '{mechanismName}'...");

        return handler;
    }

    public virtual void Init(XmppClientConnection c)
    {

    }

    public virtual bool Invoke(XmppClientConnection c, XmppElement e)
    {
        return false;
    }
}

public class SaslPlainHandler : SaslHandler
{
    public override void Init(XmppClientConnection c)
    {
        var payload = new StringBuilder()
            .Append('\0')
            .Append(c.Options.Jid.Local)
            .Append('\0')
            .Append(c.Options.Password)
            .ToString()
            .GetBytes();

        c.Send(new Auth
        {
            Mechanism = "PLAIN",
            Value = Convert.ToBase64String(payload)
        });
    }

    public override bool Invoke(XmppClientConnection c, XmppElement e)
    {
        if (e is Success)
            return true;

        if (e is Failure failure)
            throw new JabberSaslException(failure.Condition ?? FailureCondition.TemporaryAuthFailure);

        return false;
    }
}