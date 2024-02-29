using XmppSharp.Protocol;

namespace XmppSharp.Exceptions;

public class JabberException : Exception
{
    public StreamErrorCondition Error { get; }

    public JabberException(StreamErrorCondition error) : this(error, string.Empty)
    {
    }

    public JabberException(StreamErrorCondition error, string message) : base(message)
    {
        Error = error;
    }
}
