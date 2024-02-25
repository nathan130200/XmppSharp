using System.Xml;
using Jabber.Protocol;

namespace Jabber.Exceptions;

public class JabberStreamException : Exception
{
    public StreamErrorCondition? Condition { get; }

    public JabberStreamException(StreamErrorCondition? type) : this(string.Empty, type)
    {

    }

    public JabberStreamException(string? message, StreamErrorCondition? condition) : base(message)
    {
        Condition = condition;
    }

    public XmlElement GetTag()
    {
        var error = Namespace.Stream.CreateElement("stream:error");

        if (Condition.TryUnwrap(out var value))
            value.CreateElement(Message, document: error.OwnerDocument);

        return error;
    }
}
