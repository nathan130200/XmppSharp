using System.Runtime.Serialization;
using XmppSharp.Protocol.Base;
using XmppSharp.Protocol.Core.Sasl;

namespace XmppSharp;

public class JabberException : Exception
{
    public JabberException()
    {
    }

    public JabberException(string? message) : base(message)
    {
    }

    public JabberException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected JabberException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}

public class JabberStreamException : JabberException
{
    public StreamError Element { get; }
    public StreamErrorCondition? Condition => Element.Condition;
    public string? Text => Element.Text;

    public JabberStreamException(StreamError element) : base()
    {
        Element = element;
    }
}

public class JabberSaslException : JabberException
{
    public Failure Element { get; }
    public FailureCondition? Condition => Element.Condition;

    public JabberSaslException(Failure element)
    {
        Element = element;
    }
}