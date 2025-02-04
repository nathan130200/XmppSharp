using XmppSharp.Protocol.Core.Sasl;

namespace XmppSharp;

public class JabberSaslException : JabberException
{
    public FailureCondition Condition { get; }

    public JabberSaslException(FailureCondition condition) : base()
    {
        Condition = condition;
    }

    public JabberSaslException(FailureCondition condition, string message) : base(message)
    {
        Condition = condition;
    }
}