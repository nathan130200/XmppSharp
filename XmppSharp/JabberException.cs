using XmppSharp.Dom;
using XmppSharp.Protocol.Base;
using XmppSharp.Protocol.Core.Sasl;

namespace XmppSharp;

public class JabberException : Exception
{
    public XmppElement? Element { get; protected set; }

    public JabberException()
    {
    }

    public JabberException(string? message) : base(message)
    {
    }

    public JabberException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

public class JabberStreamException : JabberException
{
    public StreamErrorCondition? Condition { get; }

    public JabberStreamException(StreamErrorCondition condition) : base()
    {
        Condition = condition;
    }

    public JabberStreamException(StreamError element) : base()
    {
        Element = element;
        Condition = element.Condition;
    }
}

public class JabberSaslException : JabberException
{
    public FailureCondition? Condition { get; }

    public JabberSaslException(FailureCondition? condition) : base()
    {
        Condition = condition;
    }

    public JabberSaslException(Failure element) : base(element.Text ?? "Authentication failed")
    {
        Element = element;
        Condition = element.Condition;
    }
}