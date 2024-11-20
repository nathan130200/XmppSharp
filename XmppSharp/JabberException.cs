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
    public StreamErrorCondition? Condition { get; }

    public JabberStreamException(StreamErrorCondition condition) : base()
    {
        Condition = condition;
    }

    public JabberStreamException(StreamError error) : base(error.Text)
    {
        Data.Add("Element", error);
        Condition = error.Condition;
    }
}

public class JabberSaslException : JabberException
{
    public FailureCondition? Condition { get; }

    public JabberSaslException(FailureCondition? condition)
    {
        Condition = condition;
    }

    public JabberSaslException(Failure failure) : base(failure.Text ?? "Authentication failed")
    {
        Data.Add("Element", failure);
        Condition = failure.Condition;
    }
}