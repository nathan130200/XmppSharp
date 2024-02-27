using System.Xml.Linq;
using XmppSharp.Protocol;

namespace XmppSharp.Exceptions;

public class JabberStreamException : Exception
{
	public StreamErrorCondition? Error { get; }

	public JabberStreamException(StreamErrorCondition? error) : this(string.Empty, error)
	{

	}

	public JabberStreamException(string? message, StreamErrorCondition? error) : base(message)
	{
		Error = error;
	}

	public XElement GetTag(string lang = "en", XElement? child = default)
	{
		if (!Error.TryUnwrap(out var self))
			self = StreamErrorCondition.InternalServerError;

		return self.CreateElement(Message, lang, child);
	}
}
