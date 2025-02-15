using XmppSharp.Protocol.Core.Sasl;

namespace XmppSharp;

public class JabberSaslException : JabberException
{
    public FailureCondition Condition { get; }

    public JabberSaslException(FailureCondition condition) : base(condition.ToString())
    {
        Condition = condition;
    }

    public JabberSaslException(FailureCondition condition, string? message) : base(message ?? condition.ToString())
    {
        Condition = condition;
    }
}