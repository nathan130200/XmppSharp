using System.Xml.Linq;
using XmppSharp.Protocol;

namespace XmppSharp.Exceptions;

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

	public XElement GetTag()
	{
		var error = Namespaces.Stream.CreateElement("stream:error");

		if (!Condition.TryUnwrap(out var v))
			v = StreamErrorCondition.InternalServerError;

		error.Add(v.CreateElement(Message));

		return error;
	}
}
