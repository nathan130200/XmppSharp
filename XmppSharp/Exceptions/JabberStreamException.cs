using XmppSharp.Protocol.Base;

namespace XmppSharp;

public class JabberStreamException : JabberException
{
    public StreamErrorCondition Condition { get; }

    public JabberStreamException(StreamErrorCondition condition) : base(condition.ToString())
    {
        Condition = condition;
    }

    public JabberStreamException(StreamErrorCondition condition, string? message = default) : base(message ?? condition.ToString())
    {
        Condition = condition;
    }
}
